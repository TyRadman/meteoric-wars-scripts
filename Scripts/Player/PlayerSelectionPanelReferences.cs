using SpaceWar.MainMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceWar.MainMenu
{
    public class PlayerSelectionPanelReferences : MonoBehaviour
    {
        public ShipUIReferencesMainMenu References;
        public Text PressButtonText;
        public GameObject ContentToHide;
        public CanvasGroup Content;
        public Animation Anim;
        [SerializeField] private AnimationClip m_ShowReadiness;
        [SerializeField] private AnimationClip m_HideReadiness;
        [HideInInspector] public KeyCode PlayerSelectionKey;
        [HideInInspector] public KeyCode PlayerDeselectionKey;
        [HideInInspector] public int Index = 0;
        // detemines whether a play is going to join the game in the first place
        [HideInInspector] public bool Active = false;
        // determines whether a ship of an active player has been selected
        [HideInInspector] public bool Ready = false;
        [SerializeField] private GameObject m_Cover;
        [field: SerializeField] public Transform StartPoint { private set; get; }
        [field: SerializeField] public Transform EndPoint { private set; get; }

        public void ShowCover(bool _show)
        {
            m_Cover.SetActive(_show);
        }

        public void ShowReadiness(bool _show)
        {
            Anim.clip = _show ? m_ShowReadiness : m_HideReadiness;
            Anim.Play();
        }
    }
}