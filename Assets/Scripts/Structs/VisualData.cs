using Enums;
using Interfaces;
using UnityEngine;

namespace Structs
{
    [System.Serializable]
    public struct VisualData : IStateTyped
    {
        public StateType Type;
        public Color SpriteColor;
        public Sprite IndexSprite;
        public Color IndexSpriteColor;
        public Gradient Gradient;

        StateType IStateTyped.Type => Type;
    }
}