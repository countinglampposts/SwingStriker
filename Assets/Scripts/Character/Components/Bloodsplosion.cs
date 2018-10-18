using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

namespace Swing.Character
{
    [RequireComponent(typeof(HingeJoint2D))]
    public class Bloodsplosion : MonoBehaviour
    {
        [Inject] CharacterSettings settings;

        void OnJointBreak2D(Joint2D brokenJoint)
        {
            var joint = GetComponent<HingeJoint2D>();
            var other = joint.connectedBody.transform;
            var otherJointPosition = other.TransformPoint(joint.connectedAnchor);

            AutoDestructionParticleSystem(Instantiate(settings.bloodsplosionEffect, transform.TransformPoint(joint.anchor),transform.rotation, transform));
            AutoDestructionParticleSystem(Instantiate(settings.bloodsplosionEffect, otherJointPosition, Quaternion.LookRotation(otherJointPosition - other.position), other));
        }

        void AutoDestructionParticleSystem(GameObject gameObject){
            var ps = gameObject.GetComponent<ParticleSystem>();
            Observable.EveryUpdate()
                      .TakeUntilDestroy(gameObject)
                      .Skip(5)
                      .Where(_ => ps.particleCount == 0)
                      .Subscribe(_ => Destroy(gameObject));
        }
    }
}