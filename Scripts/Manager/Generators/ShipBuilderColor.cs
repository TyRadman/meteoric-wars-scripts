using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShipBuilderColor : Singlton<ShipBuilderColor>
    {
        [System.Serializable]
        public class ColorProfile
        {
            public Color MainColor;
            public List<Color> ExtraColors;

            public Color GetRandomComplementaryColor()
            {
                return ExtraColors.RandomItem();
            }
        }

        //[SerializeField] private List<ColorProfile> m_Colors;
        [SerializeField] private List<ShipColorProfile> m_Colors;

        public ShipColorProfile GetRandomColorProfile()
        {
            return m_Colors.RandomItem();
        }

        public ShipColorProfile GetRandomColorPalette()
        {
            ShipColorProfile tempColorProfile = GetRandomColorProfile();

            List<Color> colors = new List<Color>();
            List<Color> sideColors = tempColorProfile.ExtraColors.Duplicate();
            int colorsNumber = Random.Range(1, 3);

            for (int i = 0; i < colorsNumber; i++)
            {
                colors.Add(sideColors.RandomItem(true));
            }

            ShipColorProfile newColor = new ShipColorProfile
            {
                MainColor = tempColorProfile.MainColor,
                ExtraColors = colors
            };

            return newColor;
        }
    }
}