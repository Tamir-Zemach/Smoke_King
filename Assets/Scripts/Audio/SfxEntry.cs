using System;
using Enums;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public struct SfxEntry
    {
        public SfxType Type;
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume;   // Per‑SFX volume
        public float Cooldown;

    }
}