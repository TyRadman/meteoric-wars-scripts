using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class PlayerGraphicsHolderMainMenu : MonoBehaviour
    {
        [field: SerializeField] public List<PlayerCharacterMenuController> Graphics { private set; get; } = new List<PlayerCharacterMenuController>();

        public void AddGraphics(PlayerCharacterMenuController character)
        {
            Graphics.Add(character);
        }
    }
}
