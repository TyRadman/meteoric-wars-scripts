using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWing : ShipPart
{
    [SerializeField] private List<Transform> m_WeaponPoints;

    public override void ClearPoints()
    {
        base.ClearPoints();

        m_WeaponPoints.ForEach(w => Destroy(w.gameObject));
    }

    public Vector3 GetRandomWeaponPoint()
    {
        return m_WeaponPoints[Random.Range(0, m_WeaponPoints.Count)].position;
    }

    public Vector3 GetWeaponPoint(int _index)
    {
        return m_WeaponPoints[_index].position;
    }

    public int WeaponPointsNumber()
    {
        return m_WeaponPoints.Count;
    }
}
