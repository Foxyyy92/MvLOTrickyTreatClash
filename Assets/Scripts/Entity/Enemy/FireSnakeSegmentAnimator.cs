using NSMB.Utilities.Extensions;
using Quantum;
using UnityEngine;

namespace NSMB.Entities.Enemies {
    public unsafe class FireSnakeSegmentAnimator : QuantumEntityViewComponent {

        //---Serialized Variables
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void OnValidate() {
            this.SetIfNull(ref spriteRenderer, UnityExtensions.GetComponentType.Children);
        }

        public override void OnActivate(Frame f) {
            spriteRenderer.sortingOrder -= (f.Unsafe.GetPointer<FireSnakeSegment>(EntityRef)->Index + 1);
        }
    }
}