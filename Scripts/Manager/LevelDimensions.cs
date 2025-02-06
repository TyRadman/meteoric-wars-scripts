using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDimensions : MonoBehaviour
{
    public static LevelDimensions Instance;
    public float LevelWidth;
    public float LevelHeight;
    [HideInInspector] public float HalfWidth;
    [HideInInspector] public float HalfHeight;
    public Vector2 HorizontalDimensions;
    public Vector2 VerticalDimensions;

    private void Awake()
    {
        Instance = this;
        HalfWidth = LevelWidth / 2;
        HalfHeight = LevelHeight / 2;
    }

    public Vector2 GetRandomPointDownwards()
    {
        return new Vector2(Random.Range(-HalfWidth, HalfWidth), -HalfHeight);
    }

    public float GetRandomWidth()
    {
        return Random.Range(-HalfWidth, HalfWidth);
    }
}
