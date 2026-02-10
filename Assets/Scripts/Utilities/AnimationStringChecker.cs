using System.Linq;
using UnityEngine;

namespace Utilities
{
    public static class AnimationStringChecker
    {
        
        public static bool AnimatorHasString(Animator animatorToCheck, string triggerName)
        {
            if (HasTrigger(animatorToCheck, triggerName)) return true;
            Debug.LogError($"Animator missing trigger: {triggerName}");
            return false;

        }
        
        private static bool HasTrigger(Animator animatorToCheck, string triggerName)
        {
            return animatorToCheck.parameters.Any(param => param.name == triggerName);
        }

    }
}