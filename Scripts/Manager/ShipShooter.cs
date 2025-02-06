using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShipShooter : ShipPart
{
    public Transform ShootingPoint;
    [SerializeField] private SpriteRenderer m_Light;

    public override void SetColor(Color _col)
    {
        bool isGray = _col.r == _col.g && _col.r == _col.g;

        if (!isGray)
        {
            m_Light.color = _col;
        }

        m_Light.sortingOrder = transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder + 1;
        //m_Light.material.SetColor("_MainColor", _col);
    }

    public void SetMaterialColor(Color _col, Material _shooterMaterial)
    {
        _shooterMaterial.SetColor("_MainColor", _col);
        m_Light.material = _shooterMaterial;
    }
}
