using UnityEngine;
using System;

namespace Swing.Player
{
    [CreateAssetMenu(menuName = "Swing/SplitScreenLayouts")]
    public class SplitScreenLayouts : ScriptableObject
    {
        public SplitScreenLayout[] layouts;
    }

    [Serializable]
    public class SplitScreenLayout
    {
        public CameraSettings[] settings;
    }

    [Serializable]
    public class CameraSettings
    {
        public Rect viewportRect;
    }
}