using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestingScript : MonoBehaviour
{
    [SerializeField] private int maxNumber;

    public Vector2 GetPolygonBox(PolygonCollider2D _col)
    {
        List<Vector2> allPoints = new List<Vector2>();

        for (int i = 0; i < _col.points.Length; i++)
        {
            allPoints.Add(_col.points[i]);
        }

        var xMax = allPoints.OrderBy(p => p.x).FirstOrDefault().x;
        var yMax = allPoints.OrderBy(p => p.y).FirstOrDefault().y;

        return new Vector2(xMax * 2f, yMax * 2f);
    }

    public void DrawPointInPath()
    {

    }
}
