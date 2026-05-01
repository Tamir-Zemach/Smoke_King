using System;
using Enums;
using Interfaces;
using UnityEngine;

namespace Structs
{
    [Serializable]
    public struct VisualData : IStateTyped
    {
        public StateType Type;

        public Material Material;
        public Color Color;

        StateType IStateTyped.Type => Type;
    }
}