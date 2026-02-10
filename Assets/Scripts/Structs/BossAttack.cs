using Enums;
using UnityEngine;

namespace Structs
{
    [System.Serializable]
    public struct BossAttack
    {
        public BossAttacksTypes AttacksType;
        public GameObject Prefab;
    }
}