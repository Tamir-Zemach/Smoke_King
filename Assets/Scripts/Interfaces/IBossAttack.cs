using System;

namespace Interfaces
{
    public interface IBossAttack
    {
        void PreformAttack(Action onAttackFinished);
    }
}