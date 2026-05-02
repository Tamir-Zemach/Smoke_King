using System.Collections.Generic;
using UnityEngine;

namespace Managers.Boss
{
    public class BossMovementManager : MonoBehaviour
    {
        [SerializeField] private Transform _bossTransform;
        [SerializeField] private List<Transform> _teleportSpots;

        public List<Transform> TeleportSpots => _teleportSpots;

        public void TeleportToSpot(int spotNumber)
        {
            _bossTransform.position = _teleportSpots[spotNumber].position;
        }


        public void MoveTowards(Transform target)
        {
            //lerp into a transform
        }


        public void DashTowards(Transform target)
        {
            //lerp into a transform
        }
    }
}