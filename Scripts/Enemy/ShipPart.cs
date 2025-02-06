using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipPart : MonoBehaviour
{
    public ShipPartTag Tag;

    public virtual void SetColor(Color _col)
    {
        SpriteRenderer sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        sprite.color = _col;
    }

    public void SetOrderLayer(int _layer)
    {

    }

    public virtual void ClearPoints()
    {

    }

    public enum ShipPartTag
    {
        Body, Wing, Tail, Weapon
    }
}
