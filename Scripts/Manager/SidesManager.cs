using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class SidesManager : Singlton<SidesManager>
    {
        [System.Serializable]
        public class SideInfo
        {
            public string Tag;
            public int Layer;
            public LayerMask LayerMask;
            public Quaternion Rotation;
        }

        [SerializeField] private List<SideInfo> m_Sides;

        public void ConvertToSide(ShipComponenets _components, bool _blue)
        {
            SideInfo side = m_Sides[_blue ? 0 : 1];
            // make the ship change sides
            _components.tag = side.Tag;
            _components.gameObject.layer = side.Layer;
            _components.transform.rotation = side.Rotation;
        }
    }
}