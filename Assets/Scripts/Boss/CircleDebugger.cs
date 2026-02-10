using UnityEngine;

namespace Boss
{
    public class CircleDebugger : MonoBehaviour
    {
        [SerializeField] private float _radius = 0.15f;
        [SerializeField] private int _segments = 12;
        [SerializeField] private Color _color = Color.yellow;

        private void OnDrawGizmos()
        {
            Gizmos.color = _color;

            var center = transform.position;
            var angleStep = 360f / _segments;

            var prevPoint = center + new Vector3(_radius, 0f, 0f);

            for (var i = 1; i <= _segments; i++)
            {
                var angle = angleStep * i * Mathf.Deg2Rad;
                var nextPoint = center + new Vector3(Mathf.Cos(angle) * _radius, Mathf.Sin(angle) * _radius, 0f);

                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }
}