using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Interfaces
{
    public interface IBossAttack
    {
        void PreformAttack(Action onAttackFinished);
    }
}