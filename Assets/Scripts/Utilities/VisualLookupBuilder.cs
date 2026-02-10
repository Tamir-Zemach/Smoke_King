using System.Collections.Generic;
using Enums;
using Interfaces;

namespace Utilities
{
    /// <summary>
    ///     Build a dictionary for getting a visual data - O(1) look time
    /// </summary>
    public static class VisualLookupBuilder
    {
        public static Dictionary<StateType, T> Build<T>(List<T> list) where T : IStateTyped
        {
            var dict = new Dictionary<StateType, T>();

            foreach (var item in list) dict.TryAdd(item.Type, item);

            return dict;
        }
    }
}