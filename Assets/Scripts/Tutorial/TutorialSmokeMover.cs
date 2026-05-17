using UnityEngine;

namespace Tutorial
{
    public class TutorialSmokeMover : MonoBehaviour
    {
        private Transform _player;
        public float Speed = 4f;

        private bool _frozen = false;

        private void Start()
        {
            _player = FindAnyObjectByType<Player.PlayerMovementManager>().transform;
        }

        private void Update()
        {
            if (_player == null)
                return;

            if (_frozen)
                return;

            transform.position = Vector2.MoveTowards(
                transform.position,
                _player.position,
                Speed * Time.deltaTime
            );
        }

        public void Freeze()
        {
            _frozen = true;
        }

        public void Unfreeze()
        {
            _frozen = false;
        }
    }
}