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
        [InjectOptional] TeamData[] teamDatas;

        private bool locked;

        private void Start()
        {
            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => locked = true);

            signalBus.GetStream<BallResetSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => locked = false);

            if (teamDatas.Length > 0)
            {
                Color color = teamDatas.First(element => element.id == team).color;
                color.a = .5f;
                GetComponent<Renderer>().material.color = color;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(!locked && collision.gameObject == gameBall.gameObject)
            {
                var scoringTeamId = (teamDatas.Length > 0)? teamDatas.First(element => element.id != team).id : 0;
                signalBus.Fire(new GoalScoredSignal() { team = scoringTeamId });
            }
        }
    }
}