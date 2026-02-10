using System;
using Enums;
using UnityEngine;

namespace Structs
{
    [Serializable]
    public struct BossAttack
    {
        public BossAttacksTypes AttacksType;
        public GameObject Prefab;
    }
}