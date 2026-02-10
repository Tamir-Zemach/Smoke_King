using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class BossMovementManager: MonoBehaviour
    {
        [SerializeField] private GameObject _boss;
        [SerializeField] private List<Transform> _teleportSpots;
        
        public List<Transform> TeleportSpots => _teleportSpots;
        
        public void TeleportToSpot(int spotNumber)
        {
            _boss.transform.position = _teleportSpots[spotNumber].position;
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