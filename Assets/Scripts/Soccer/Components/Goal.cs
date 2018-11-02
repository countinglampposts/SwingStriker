using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using Swing.Player;
using System.Linq;

namespace Swing.Game.Soccer
{
    [RequireComponent(typeof(Renderer))]
    public class Goal : MonoBehaviour
    {
        [SerializeField] int team;
        [Inject] SignalBus signalBus;
        [Inject] TeamsData teamsData;
        [Inject] Ball ball;

        private bool locked;

        private void Start()
        {
            signalBus.GetStream<GameEndsSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => locked = true);

            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .TakeUntil(signalBus.GetStream<GameEndsSignal>())
                     .Subscribe(_ => locked = true);

            signalBus.GetStream<BallResetSignal>()
                     .TakeUntilDestroy(this)
                     .TakeUntil(signalBus.GetStream<GameEndsSignal>())
                     .Subscribe(_ => locked = false);

            gameObject.OnTriggerEnter2DAsObservable()
                      .TakeUntilDestroy(this)
                      .Where(_ => !locked)
                      .Where(collision => collision.gameObject == ball.gameObject)
                      .Select(_ => teamsData.teams.FirstOrDefault(element => element.id != team).id)
                      .Subscribe(scoringTeamId => signalBus.Fire(new GoalScoredSignal() { team = scoringTeamId }));


            if (teamsData.teams.Length > 0)
            {
                Color color = teamsData.teams.First(element => element.id == team).color;
                color.a = .5f;
                GetComponent<Renderer>().material.color = color;
            }
        }
    }
}