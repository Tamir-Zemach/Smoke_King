using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    ///     helper generic method for getting specific data from a given dictionary
    /// </summary>
    public static class GenericVisualHelper
    {
        public static T Get<T>(Dictionary<StateType, T> dict, StateType type)
        {
            if (dict.TryGetValue(type, out var value))
                return value;

            Debug.LogError($"[GenericVisualHelper] No entry found for StateType: {type}");
            return default;
        }
    }
}