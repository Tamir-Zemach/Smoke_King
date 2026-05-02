using UnityEngine;

namespace Data
{
    [CreateAssetMenu]
    public class PlayerData : ScriptableObject
    {
        [Header("Movement Parameters")] public float Speed = 5;

        public float JumpForce = 200;
        public float JumpForceFromWallY = 200;
        public float JumpForceFromWallX = 200;
        public LayerMask GroundMask;
        public LayerMask WallMask;


        [Header("Attack Parameters")] 

        public float AttackDuration = 0.2f;

        [Header("Health Parameters")] public int MaxHealth = 3;

        [Header("Getting Hit Parameters")] public float KnockBackForce = 15;

        public float EaseControlBackDur = 0.3f;
    }
}