using Player;

namespace Tutorial
{
    public class PlayerInputBlocker
    {
        private readonly PlayerInput _input;

        public bool BlockMovement { get; set; }
        public bool BlockJump { get; set; }
        public bool BlockAttack { get; set; }
        public bool BlockStateSwitch { get; set; }

        public PlayerInputBlocker(PlayerInput input)
        {
            _input = input;
        }

        public bool CanJump()        => !BlockJump;
        public bool CanAttack()      => !BlockAttack;
        public bool CanStateSwitch() => !BlockStateSwitch;
        public bool CanMove()        => !BlockMovement;
    }
}