using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LevelController : MonoBehaviour
    {
        private void Update()
        {
            if(Input.GetKey(KeyCode.R)){
                Application.LoadLevel(Application.loadedLevel);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Press R to restart");
        }
    }
}