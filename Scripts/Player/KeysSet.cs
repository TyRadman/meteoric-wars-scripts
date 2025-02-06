using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(menuName = "Others/Keys Set")]
    public class KeysSet : ScriptableObject
    {
        public List<KeySet> Keys = new List<KeySet> { new KeySet {Tag = KeyTag.Up },
    new KeySet {Tag = KeyTag.Down }, new KeySet {Tag = KeyTag.Left}, new KeySet {Tag = KeyTag.Right},
    new KeySet {Tag = KeyTag.Ability1}, new KeySet {Tag = KeyTag.Ability2}, new KeySet {Tag = KeyTag.Ability3},
    new KeySet {Tag = KeyTag.Ability4}, new KeySet {Tag = KeyTag.NormalShot}};
    }
}
