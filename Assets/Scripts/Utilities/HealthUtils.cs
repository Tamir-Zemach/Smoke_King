using System.Collections;
using Interfaces;
using UnityEngine;

namespace Utilities
{
    public static class HealthUtils
    {
        public static bool IsBelowHalf(int current, int max)
        {
            return (float)current / max < 0.5f;
        }

        public static bool IsAboveHalf(int current, int max)
        {
            return (float)current / max > 0.5f;
        }

        public static void DisplayHealthInConsole(int current, int max, string label = "Health")
        {
            Debug.Log($"{label} — Current: {current}, Max: {max}");
        }

        public static IEnumerator Invisibility(IInvincible target, float time)
        {
            target.IsInvincible = true;
            target.OnInvincibleStart();

            yield return new WaitForSeconds(time);

            target.IsInvincible = false;
            target.OnInvincibleEnd();
        }
    }
}