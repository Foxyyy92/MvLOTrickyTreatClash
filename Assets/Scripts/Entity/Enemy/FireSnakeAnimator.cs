using NSMB.Quantum;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace NSMB.Entities.Enemies {
    public unsafe class FireSnakeAnimator : QuantumEntityViewComponent<StageContext> {

        //---Serialized Variables
        [SerializeField] private Transform eyesParent;
        [SerializeField] private float maxEyesRotation = 20f, marioDistance = 7;
        [SerializeField] private float slerpSpeed = 8f;

        public override void OnUpdateView() {
            Frame f = PredictedFrame;
            EntityRef closestPlayer = QuantumUtils.FindClosestAliveMario(f, transform.position.ToFPVector2(), out FPVector2 closestMarioPosition, ViewContext.Stage);
            Vector3 effectiveMarioPosition;
            if (closestPlayer != EntityRef.None) {
                effectiveMarioPosition = closestMarioPosition.ToUnityVector3();
                effectiveMarioPosition.z = -marioDistance;
            } else {
                effectiveMarioPosition = transform.position + (f.Unsafe.GetPointer<PhysicsObject>(EntityRef)->Velocity.ToUnityVector3() * 1);
                effectiveMarioPosition.z = -marioDistance;
            }

            Quaternion forward = Quaternion.LookRotation(-Vector3.forward);
            Quaternion lookAtMario = Quaternion.LookRotation((effectiveMarioPosition - transform.position).normalized);
            float angle = Quaternion.Angle(lookAtMario, forward);
            if (angle > maxEyesRotation) {
                lookAtMario = Quaternion.RotateTowards(lookAtMario, forward, angle - maxEyesRotation);
            }
            eyesParent.forward = Vector3.Slerp(eyesParent.forward, lookAtMario * -Vector3.forward, Time.deltaTime * slerpSpeed);
        }
    }
}