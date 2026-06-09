using System;
using System.Collections.Generic;
using Audio;
using Cameras;
using UnityEngine;
using DG.Tweening;
using Enums;
using Particles;

namespace Ui
{
    public class UiSmokeTransition : MonoBehaviour
    {
        [Header("Spawn Settings")] 
        public GameObject SmokePrefab;
        public Transform SpawnPoint;
        public int SmokeCount = 5;
        public float VerticalOffset = 0.2f;

        [Header("Circle Movement Settings")]
        public float CircleRadius = 0.5f;
        public float CircleSpeedDegPerSec = 180f;
        public float CircleDuration = 1.5f;

        [Header("Fly Settings")]
        public float FlyDistance = 3f;
        public float FlyDuration = 1f;

        [Header("Black Curtain (Sprite)")]
        public Transform BlackCurtain;
        public float CurtainDelay = 0.1f;
        public float CurtainMoveDistance = 10f;

        private readonly List<Transform> _spawnedSmokes = new();
        private bool _soundPlayed;

        // -------------------------
        // PUBLIC API
        // -------------------------

        public void PlayTransitionToRight(Action onComplete = null)
        {
            PlayTransitionInternal(Vector2.right, Vector3.right, onComplete);
        }

        public void PlayTransitionToLeft(Action onComplete = null)
        {
            PlayTransitionInternal(Vector2.left, Vector3.left, onComplete);
        }

        // -------------------------
        // INTERNAL LOGIC
        // -------------------------

        private void PlayTransitionInternal(Vector2 flyDir, Vector3 curtainDir, Action onComplete)
        {
            if (SmokePrefab == null || SpawnPoint == null)
            {
                Debug.LogError("UiSmokeTransition: Missing prefab or spawnPoint");
                return;
            }

            // Clear previous smokes
            foreach (var s in _spawnedSmokes)
                if (s != null)
                    Destroy(s.gameObject);

            _spawnedSmokes.Clear();

            // Instantiate stacked smokes
            for (int i = 0; i < SmokeCount; i++)
            {
                Vector3 pos = SpawnPoint.position + new Vector3(0, i * VerticalOffset, 0);
                GameObject obj = Instantiate(SmokePrefab, pos, Quaternion.identity, SpawnPoint);
                _spawnedSmokes.Add(obj.transform);
            }

            AnimateSmokes(flyDir, curtainDir, onComplete);
        }

        private void AnimateSmokes(Vector2 flyDir, Vector3 curtainDir, Action onComplete)
        {
            int completed = 0;

            foreach (var smoke in _spawnedSmokes)
            {
                Tween circleTween = ParticleMovementUtility.MoveInCircle(
                    smoke,
                    CircleRadius,
                    CircleSpeedDegPerSec,
                    CircleDuration
                );

                circleTween.OnComplete(() =>
                {
                    // Trigger curtain + sound only once
                    if (!_soundPlayed)
                    {
                        _soundPlayed = true;

                        AudioManager.Instance.PlaySfx(SfxType.SmokeTransition);

                        if (BlackCurtain != null)
                        {
                            Vector3 startPos = BlackCurtain.position;
                            Vector3 targetPos = startPos + curtainDir * CurtainMoveDistance;

                            BlackCurtain.DOMove(targetPos, FlyDuration)
                                .SetEase(Ease.OutQuad)
                                .SetDelay(CurtainDelay);
                        }
                    }

                    // Fly smoke in chosen direction
                    Tween flyTween = ParticleMovementUtility.FlyTo(
                        smoke,
                        flyDir,
                        FlyDistance,
                        FlyDuration
                    );

                    flyTween.OnComplete(() =>
                    {
                        completed++;

                        if (completed == _spawnedSmokes.Count)
                        {
                            onComplete?.Invoke();
                        }
                    });
                });

            }
        }
    }
}
