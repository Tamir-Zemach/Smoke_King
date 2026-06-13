using Enums;
using UnityEngine;
using Audio;

namespace Boss
{
    public class BossSoundManager : MonoBehaviour
    {
        private AudioManager _audio;

        private void Awake()
        {
            _audio = AudioManager.Instance;
        }

        public void PlaySfx(SfxType type)
        {
            _audio.PlaySfx(type);
        }
        
        public void PlaySfxByParticles(BossParticles bossParticles)
        {
            switch (bossParticles)
            {
                case BossParticles.Spawns:
                    _audio.PlaySfx(SfxType.BossSpawn);
                    break;
                
                case BossParticles.Snap:
                    _audio.PlaySfx(SfxType.BossSnap);
                    break;
                
                case BossParticles.Teleport:
                    _audio.PlaySfx(SfxType.BossPortalSpawn);
                    break;


                default:
                    break;
            }
        }
    }
}