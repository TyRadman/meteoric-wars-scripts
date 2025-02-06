using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonCreator : MonoBehaviour
{
    [SerializeField] private GameObject OriginalObject;
    [SerializeField] private GameObject ObjectToAddPolygonTo;
    [SerializeField] private float m_Scale;

    private void Awake()
    {
        createPolygon();
    }

    private void createPolygon()
    {
        PolygonCollider2D collider = ObjectToAddPolygonTo.AddComponent<PolygonCollider2D>();
        collider.pathCount = 1;

        // collider.SetPath(0, OriginalObject.GetComponent<PolygonCollider2D>().GetPath(0));
        // collider.SetPath(0, getScaledPolygon(OriginalObject.GetComponent<PolygonCollider2D>().GetPath(0), OriginalObject.transform.localScale.x));
        collider.SetPath(0, scalePath(OriginalObject.GetComponent<PolygonCollider2D>().GetPath(0), OriginalObject.transform.localScale));
    }
    
    private Vector2[] getScaledPolygon(Vector2[] _path, float _scale)
    {
        var scaledPath = new List<Vector2>();

        for (int i = 0; i < _path.Length; i++)
        {
            var p1 = _path[i];
            var p2 = _path[(i + 1) % _path.Length];
            var dx = p2.x - p1.x;
            var dy = p2.y - p1.y;
            var n = new Vector2(dy, -dx).normalized;
            scaledPath.Add(p1 + n * _scale / 2);
        }

        return scaledPath.ToArray();
    }

    private Vector2[] scalePath(Vector2[] _path, Vector2 _scale)
    {
        var scaledPath = new List<Vector2>();

        for (int i = 0; i < _path.Length; i++)
        {
            var point = _path[i];
            point.x *= _scale.x;
            point.y *= _scale.y;
            scaledPath.Add(point);
        }

        return scaledPath.ToArray();
    }
}
