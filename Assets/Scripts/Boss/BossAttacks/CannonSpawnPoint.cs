using UnityEngine;

namespace Boss.BossAttacks
{
    public class CannonSpawnPoint : MonoBehaviour
    {
        public Color Color = Color.yellow;
        public float Size = 1;
        public float SidesSize = 0.7f;
        public float MaxDistance = 5;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color;


            // Local directions
            var tip = transform.position + transform.up * Size;

            var left = transform.position + Quaternion.Euler(0, 0, 140) * transform.up * Size * SidesSize;
            var right = transform.position + Quaternion.Euler(0, 0, -140) * transform.up * Size * SidesSize;

            // Draw triangle edges
            Gizmos.DrawLine(tip, left);
            Gizmos.DrawLine(tip, right);
            Gizmos.DrawLine(left, right);


            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.up * MaxDistance);
        }
    }
}