using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpaceWar.UI.HUD
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private GameObject m_Content;
        [Header("Image References")]
        public Image HealthBarImage;
        public Image HealthIconImage;
        public Image ShooterPointsImage;
        public Image RegenerationBarImage;
        [Header("Text References")]
        public TextMeshProUGUI ShooterPointsAmountText;
        public TextMeshProUGUI ScoreText;
        public TextMeshProUGUI LivesText;
        public TextMeshProUGUI HealthText;
        public TextMeshProUGUI ShooterLevelText;
        public TextMeshProUGUI CoinsText;
        

        public void Enable(bool enable)
        {
            m_Content.SetActive(enable);
        }
    }
}
