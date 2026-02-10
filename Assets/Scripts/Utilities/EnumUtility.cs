using System;

namespace Utilities
{
    public static class EnumUtility
    {
        private static readonly Random Random = new();

        public static int GetRandomIndex<T>() where T : Enum
        {
            var length = Enum.GetNames(typeof(T)).Length;
            return Random.Next(0, length);
        }

        public static T GetRandomValue<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            var index = Random.Next(values.Length);
            return (T)values.GetValue(index);
        }

        public static T GetNextValueInEnum<T>(T current) where T : Enum
        {
            var values = (T[])Enum.GetValues(typeof(T));
            var index = Array.IndexOf(values, current);

            // Move to next index, wrap around using modulo
            var nextIndex = (index + 1) % values.Length;

            return values[nextIndex];
        }

        public static T GetPreviousValueInEnum<T>(T current) where T : Enum
        {
            var values = (T[])Enum.GetValues(typeof(T));
            var index = Array.IndexOf(values, current);

            var prevIndex = (index - 1 + values.Length) % values.Length;

            return values[prevIndex];
        }
    }
}