namespace Quantum {

    public unsafe partial struct FireSnake {
        public void Respawn(Frame f, EntityRef entity) {
            JumpTimer = 0;
            f.Unsafe.GetPointer<Interactable>(entity)->ColliderDisabled = false;

            if (f.Exists(Segments[0])) {
                for (int i = 0; i < Segments.Length; i++) {
                    EntityRef segment = Segments[i];
                    f.Unsafe.GetPointer<FireSnakeSegment>(segment)->Respawn(f, segment);
                }
            }
        }

        public void Kill(Frame f, EntityRef entity) {

        }
    }

    public unsafe partial struct FireSnakeSegment {
        public void Respawn(Frame f, EntityRef entity) {
            f.Unsafe.GetPointer<Interactable>(entity)->ColliderDisabled = false;
            Reset(f, entity);
        }

        public void Reset(Frame f, EntityRef entity) {
            f.Unsafe.GetPointer<Transform2D>(entity)->Teleport(f, f.Unsafe.GetPointer<Transform2D>(Parent)->Position);
            SpawnTick = 0;
        }
    }
}