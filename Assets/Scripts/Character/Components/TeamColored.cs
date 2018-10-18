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
        [Inject] PlayerData playerData;

        private void Start()
        {
            GetComponent<Renderer>().material.color = playerData.team.color;
        }
    }
}