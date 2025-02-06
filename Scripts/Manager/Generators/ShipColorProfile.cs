using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    [CreateAssetMenu(fileName = "Color_NAME", menuName = Directories.MAIN + "Ship Color Palette")]
    public class ShipColorProfile : ScriptableObject
    {
        public Color MainColor;
        public List<Color> ExtraColors;

        public Color GetRandomComplementaryColor()
        {
            return ExtraColors.RandomItem();
        }
    }
}
