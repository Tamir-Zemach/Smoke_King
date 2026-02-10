using Player;
using TMPro;
using UnityEngine;

namespace Ui
{
    public class PlayerHealthUi : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private PlayerHealthManager _playerHealthManager;

        private void Awake()
        {
            _playerHealthManager.OnHealthChanged += UpdateText;
        }

        private void Start()
        {
            UpdateText();
        }

        private void OnDestroy()
        {
            _playerHealthManager.OnHealthChanged -= UpdateText;
        }

        private void UpdateText()
        {
            _text.text = $"Player Health: {_playerHealthManager.CurrentHealth}/{_playerHealthManager.MaxHealth}";
        }
    }
}