using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpaceWar.UI.HUD
{
    public class HUDManager : MonoBehaviour
    {
        [field: SerializeField] public List<PlayerHUD> PlayerUIReferences { get; private set; }
        [SerializeField] private Gradient m_HealthGradient;

        public void SetUp()
        {
            // start with all the HUDs disabled
            PlayerUIReferences.ForEach(h => h.Enable(false));
        }

        public void EnablePlayerHUD(int playerIndex, bool enable)
        {
            PlayerUIReferences[playerIndex].Enable(enable);
        }

        public void UpdatePlayerHealth(float _amount, int _playerIndex, string _healthAmountText)
        {
            PlayerUIReferences[_playerIndex].HealthBarImage.fillAmount = _amount;
            Color healthColor = m_HealthGradient.Evaluate(_amount);
            PlayerUIReferences[_playerIndex].HealthBarImage.color = healthColor;
            PlayerUIReferences[_playerIndex].HealthIconImage.color = healthColor;
            PlayerUIReferences[_playerIndex].HealthText.text = _healthAmountText;
        }

        public void UpdatePlayerRegeneratedHealthBar(float _amount, int _playerIndex)
        {
            PlayerUIReferences[_playerIndex].RegenerationBarImage.fillAmount = _amount;
        }

        public void UpdatePlayerShootingPoints(float _amount, int _playerIndex, string _amountText, string _level)
        {
            PlayerUIReferences[_playerIndex].ShooterPointsImage.fillAmount = _amount;
            PlayerUIReferences[_playerIndex].ShooterPointsAmountText.text = _amountText;
            PlayerUIReferences[_playerIndex].ShooterLevelText.text = _level;
        }

        public void UpdateScoreText(float _amount, int _playerIndex)
        {
            PlayerUIReferences[_playerIndex].ScoreText.text = _amount.ToString();
        }

        public void SetLivesCount(int _lives, int _playerIndex)
        {
            PlayerUIReferences[_playerIndex].LivesText.text = $"x {_lives}";
        }

        public void UpdateCoins(int _coins, int _playerIndex)
        {
            PlayerUIReferences[_playerIndex].CoinsText.text = _coins.ToString();
        }
    }
}
