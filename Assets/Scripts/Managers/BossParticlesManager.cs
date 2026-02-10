using Enums;
using UnityEngine;

namespace Managers
{
    public class BossParticlesManager : MonoBehaviour
    {
        
        [SerializeField] private ParticleSystem _teleportParticles;
        [SerializeField] private ParticleSystem _spawnParticles;
        [SerializeField] private ParticleSystem _hitParticles;


        public void PlayParticles(BossParticles bossParticles)
        {
            switch (bossParticles)
            {
                case BossParticles.Teleport:
                    _teleportParticles.Play();
                    break;
                case BossParticles.Spawns:
                    _spawnParticles.Play();
                    break;
                case BossParticles.Hit:
                    _hitParticles.Play();
                    break;
                default:
                    _teleportParticles.Play();
                    break;
            }
        }
    }
}