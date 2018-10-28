using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.Audio;
using Swing.Sound;
using System.Linq;

namespace Swing.Character
{
    public class JointBrokenSignal{}

    [RequireComponent(typeof(AnchoredJoint2D))]
    public class Bloodsplosion : MonoBehaviour
    {
        [Inject] CharacterSettings settings;
        [Inject] SignalBus signalBus;
        [Inject] SoundPlayer soundPlayer;

        private bool dead = false;

        private void Start()
        {
            signalBus.GetStream<SuicideSignal>()
               .TakeUntilDestroy(this)
               .Subscribe(__ => BreakEffect(true));
        }

        void OnJointBreak2D(Joint2D brokenJoint)
        {
            BreakEffect(false);
        }

        private void BreakEffect(bool destroyJoint){
            if (dead) return;

            signalBus.Fire<JointBrokenSignal>();
            signalBus.Fire(new RumbleTriggeredSignal { magnitude = 3f });

            var joint = GetComponent<AnchoredJoint2D>();
            var jointPosition = transform.TransformPoint(joint.anchor);
            var other = joint.connectedBody.transform;
            var otherJointPosition = other.TransformPoint(joint.connectedAnchor);
            var otherJointDirection = (otherJointPosition - other.position).normalized;

            AutoDestructionParticleSystem(Instantiate(settings.bloodsplosionEffect, jointPosition, transform.rotation, transform));
            AutoDestructionParticleSystem(Instantiate(settings.bloodsplosionEffect, otherJointPosition, Quaternion.LookRotation(otherJointDirection), other));

            if (destroyJoint)
            {
                joint.enabled = false;
                float suicideExplosionForce = 250f;
                joint.GetComponent<Rigidbody2D>().AddForceAtPosition(transform.forward * suicideExplosionForce, jointPosition);
                other.GetComponent<Rigidbody2D>().AddForceAtPosition(otherJointDirection * suicideExplosionForce, otherJointPosition);
            }

            soundPlayer.PlaySound("Bloodsplosion", transform);

            dead = true;
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