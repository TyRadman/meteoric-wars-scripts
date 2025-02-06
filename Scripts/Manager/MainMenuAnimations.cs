using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace SpaceWar.MainMenu
{
    public class MainMenuAnimations : Singlton<MainMenuAnimations>
    {
        public enum AnimationType
        {
            MainMenu, Modes, Settings, PlayerSelectionWindow, PlayerSelectionProcess, PlayerConfirmationSelection, CountDown
        }

        [System.Serializable]
        public struct AnimationInfo
        {
            public AnimationType Type;
            public Animation Anim;
            public AnimationClip[] Clips;
        }

        [SerializeField] private List<AnimationInfo> m_Animations;
        [SerializeField] private TextMeshProUGUI m_CountDownText;

        #region Buttons
        #region First Layer
        public void StartButton()
        {
            // hide the main menu buttons and show the level selection panel
            var animationInfo = m_Animations.Find(a => a.Type == AnimationType.MainMenu);
            PerformAnimation(animationInfo.Anim, animationInfo.Clips[0]);
            animationInfo = m_Animations.Find(a => a.Type == AnimationType.Modes);
            PerformAnimation(animationInfo.Anim, animationInfo.Clips[1]);

            // switch controls 
            MenuController.i.SwitchInput(AnimationType.Modes);
        }

        public void ExitButton()
        {
            Application.Quit();
        }
        #endregion

        #region Second Layer
        public void PlayerSelectionMenuShow()
        {
            var animationInfo = m_Animations.Find(a => a.Type == AnimationType.Modes);
            PerformAnimation(animationInfo.Anim, animationInfo.Clips[0]);
            animationInfo = m_Animations.Find(a => a.Type == AnimationType.PlayerSelectionWindow);
            PerformAnimation(animationInfo.Anim, animationInfo.Clips[1]);

            MenuController.i.SwitchInput(AnimationType.PlayerSelectionWindow);
        }

        public void BackSecondLayerButton()
        {
            var animationInfo = m_Animations.Find(a => a.Type == AnimationType.MainMenu);
            PerformAnimation(animationInfo.Anim, animationInfo.Clips[1]);
            animationInfo = m_Animations.Find(a => a.Type == AnimationType.Modes);
            PerformAnimation(animationInfo.Anim, animationInfo.Clips[0]);

            MenuController.i.SwitchInput(AnimationType.MainMenu, -1, false);
        }
        #endregion

        #region Third Layer
        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }

        public void ReturnToModesMenu()
        {
            var animationInfo = m_Animations.Find(a => a.Type == AnimationType.Modes);
            PerformAnimation(animationInfo.Anim, animationInfo.Clips[1]);
            animationInfo = m_Animations.Find(a => a.Type == AnimationType.PlayerSelectionWindow);
            PerformAnimation(animationInfo.Anim, animationInfo.Clips[0]);
        }
        #endregion
        #endregion

        private void PerformAnimation(Animation _animation, AnimationClip _clip)
        {
            // 1 is close, 0 is open
            _animation.clip = _clip;
            _animation.Play();
        }

        private void PerformAnimation(AnimationType _type, int _animationIndex)
        {
            var anim = m_Animations.Find(a => a.Type == _type);
            anim.Anim.clip = anim.Clips[_animationIndex];
            anim.Anim.Play();
        }

        public void StartCountDown()
        {
            // start animation
            PerformAnimation(AnimationType.CountDown, 0);
            // start a coroutine to display numbers
            StartCoroutine(CountDownProcess());
        }

        private IEnumerator CountDownProcess()
        {
            // play the count down with the animation
            for (int i = 5; i > 0; i--)
            {
                m_CountDownText.text = (i).ToString("0");
                yield return new WaitForSeconds(1f);
            }

            // disable main menu inputs
            GameObject.FindObjectOfType<PlayerMainMenuController>().DisableInputs();
            // load the next scene
            SceneManager.LoadScene(1);
        }

        public void StopCountDown()
        {
            StopAllCoroutines();
            m_Animations.Find(a => a.Type == AnimationType.CountDown).Anim.Stop();
            m_CountDownText.enabled = false;
        }
    }
}