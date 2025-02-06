using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyShipNamesGenerator : Singlton<EnemyShipNamesGenerator>
    {
        [System.Serializable]
        public struct Name
        {
            public string NameText;
            public List<ShipRank> RanksAllowed;
        }

        public List<Name> FirstNames;
        public List<Name> LastNames;
        public List<Name> SoloNames;

        public string GetShipName(ShipRank _rank)
        {
            string name = string.Empty;
            List<Name> availableFirstNames = FirstNames.FindAll(n => n.RanksAllowed.Contains(_rank));
            List<Name> availableLastNames = LastNames.FindAll(n => n.RanksAllowed.Contains(_rank));
            List<Name> availableSoloNames = SoloNames.FindAll(n => n.RanksAllowed.Contains(_rank));

            // 20% chance of being a one name 
            if (Random.value < 0.3f)
            {
                name += availableSoloNames.RandomItem(true).NameText;
            }
            else
            {
                if (Random.value < 0.8f)
                {
                    name += $"{availableFirstNames.RandomItem(true).NameText} {availableLastNames.RandomItem(true).NameText}";
                }
                else
                {
                    name += $"{availableSoloNames.RandomItem(true).NameText}, the {availableFirstNames.RandomItem(true).NameText} {availableLastNames.RandomItem(true).NameText}";
                }
            }

            return $"{name}"; // ({_rank})";
        }
    }
}