using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopAnimation : MonoBehaviour
{
    public enum Animations
    {
        ShowInteraction, HideInteraction, OpenShop, CloseShop
    }

    [SerializeField] private Animator m_Anim;
    private List<string> m_ShowWeaponAnimationKeys = new List<string>();
    private List<string> m_HideWeaponAnimationKeys = new List<string>();

    public void PlayAnimation(Animations _animation)
    {
        m_Anim.SetTrigger(Animator.StringToHash(_animation.ToString()));
    }

    public void PlayWeaponAnimation(bool _show)
    {
        StartCoroutine(PlayWeaponShowingAnimationsProcess(_show ? m_ShowWeaponAnimationKeys : m_HideWeaponAnimationKeys));
    }

    private IEnumerator PlayWeaponShowingAnimationsProcess(List<string> _animationKeys)
    {
        int startingLayer = 2;

        for (int i = 0; i < _animationKeys.Count; i++)
        {
            m_Anim.SetTrigger(_animationKeys[i]);
            yield return new WaitForSeconds(m_Anim.GetCurrentAnimatorStateInfo(startingLayer + i).length);
        }
    }

    public void AddWeaponAnimations(string _showWeaponKey, string _hideWeaponKey)
    {
        m_ShowWeaponAnimationKeys.Add(_showWeaponKey);
        m_HideWeaponAnimationKeys.Add(_hideWeaponKey);
    }

    public float GetAnimationLength(int _layer)
    {
        return m_Anim.GetCurrentAnimatorStateInfo(_layer).length;
    }

    public float GetWeaponsAnimationLength(bool _show)
    {
        float length = 0f;
        List<string> animations = _show ? m_ShowWeaponAnimationKeys : m_HideWeaponAnimationKeys;
        int startingLayer = 2;

        for (int i = 0; i < animations.Count; i++)
        {
            length += m_Anim.GetCurrentAnimatorStateInfo(startingLayer + i).length;
        }

        return length;
    }
}
