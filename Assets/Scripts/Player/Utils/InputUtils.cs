using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swing.Player
{
    public static class InputUtils
    {
        private static Vector3 ScreenPositionToWorldPosition(this Camera camera, Vector3 position)
        {
            var ray = camera.ScreenPointToRay(position);
            float point;
            if ((new Plane(Vector3.zero, Vector3.up, Vector3.right)).Raycast(ray, out point))
            {
                return ray.GetPoint(point);
            }
            Debug.LogError("Position not colliding with plane");
            return Vector3.zero;
        }
    }
}