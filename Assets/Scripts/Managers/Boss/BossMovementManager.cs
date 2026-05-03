using System;
using System.Collections.Generic;
using Boss;
using UnityEngine;

namespace Managers.Boss
{
    public class BossMovementManager : MonoBehaviour
    {
        [SerializeField] private Transform _bossTransform;

        public List<BossTeleportSpot> TeleportSpots { get; private set; }
        public bool IsCurrentSpotLeft;

        private void Awake()
        {
            TeleportSpots = new List<BossTeleportSpot>(
                GetComponentsInChildren<BossTeleportSpot>()
            );
        }

        
        public void TeleportToSpot(int spotNumber)
        {
            var curSpot = TeleportSpots[spotNumber];
            FlipBoss(curSpot.LeftPoint);
            _bossTransform.position = curSpot.transform.position;
        }


        public void MoveTowards(Transform target)
        {
            //lerp into a transform
        }


        public void DashTowards(Transform target)
        {
            //lerp into a transform
        }


        private void FlipBoss(bool facingLeft)
        {
            IsCurrentSpotLeft = facingLeft;
            var scale = _bossTransform.localScale;
            scale.x = facingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            _bossTransform.localScale = scale;
        }

    }
}