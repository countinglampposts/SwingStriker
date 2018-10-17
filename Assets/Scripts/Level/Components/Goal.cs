using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using Swing.Player;
using System.Linq;

namespace Swing.Level
{
    public class GoalScoredSignal{
        public int team; 
    }
    public class BallResetSignal { }

    [RequireComponent(typeof(Renderer))]
    public class Goal : MonoBehaviour
    {
        [SerializeField] int team;
        [Inject] GameBall gameBall;
        [Inject] SignalBus signalBus;
        [Inject] TeamsData teamsData;

        private bool locked;

        private void Start()
        {
            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => locked = true);

            signalBus.GetStream<BallResetSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => locked = false);

            if (teamsData.teams.Length > 0)
            {
                Color color = teamsData.teams.First(element => element.id == team).color;
                color.a = .5f;
                GetComponent<Renderer>().material.color = color;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(!locked && collision.gameObject == gameBall.gameObject)
            {
                var scoringTeamId = (teamsData.teams.Length > 0)? teamsData.teams.First(element => element.id != team).id : 0;
                signalBus.Fire(new GoalScoredSignal() { team = scoringTeamId });
            }
        }
    }
}