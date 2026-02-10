using Data;
using Enums;
using UnityEngine;
using Utilities;

namespace Boss.BossAttacks.DangerZoneAttack
{
    public class DangerZone : MonoBehaviour
    {
        [SerializeField] private DangerZoneAttackData _data;
        [SerializeField] private SpriteRenderer _spriteRendererIndex;
        private Collider2D _collider;

        private DamageGiver _damageGiver;
        private SpriteRenderer _spriteRenderer;

        private void GetRelevantComponents()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _damageGiver = GetComponent<DamageGiver>();
            _collider = GetComponent<Collider2D>();
        }

        public void SetState(StateType state)
        {
            if (_spriteRenderer == null || _damageGiver == null) GetRelevantComponents();
            var visual = _data.GetVisual(state);

            _damageGiver.StateType = state;
            _spriteRenderer.color = visual.SpriteColor;
            _spriteRendererIndex.sprite = visual.IndexSprite;
            ;
            _spriteRendererIndex.color = visual.IndexSpriteColor;
            ;
        }


        public void ShowIndex(bool show)
        {
            _spriteRendererIndex.gameObject.SetActive(show);
        }

        public void ActiveZone(bool active)
        {
            _collider.enabled = active;
            _damageGiver.enabled = active;
            _spriteRenderer.enabled = active;
        }
    }
}