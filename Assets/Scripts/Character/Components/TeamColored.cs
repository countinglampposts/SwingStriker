using System.Collections;
using System.Collections.Generic;
using Swing.Player;
using UnityEngine;
using Zenject;

namespace Swing.Character
{
    [RequireComponent(typeof(Renderer))]
    public class TeamColored : MonoBehaviour
    {
        [InjectOptional] TeamData teamData;

        private void Start()
        {
            GetComponent<Renderer>().material.color = teamData.color;
        }
    }
}