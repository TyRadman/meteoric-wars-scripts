using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceWar
{
    public class KeyBend : MonoBehaviour
    {
        public Button KeyButton;
        public Text ActionText;
        public Text KeyText;
        [HideInInspector] public KeyTag Tag;

        public void FillInfo(KeySet _key)
        {
            Tag = _key.Tag;
            ActionText.text = _key.Tag.ToString();
            KeyText.text = _key.TheButton.ToString();
        }

        public void OnSelected()
        {
            KeyButton.Select();
        }
    }
}