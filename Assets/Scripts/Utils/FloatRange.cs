using System;
using UnityEngine;

[Serializable]
public struct FloatRange
{
    [Min(1f)]
    public float min;

    [Min(1f)]
    public float max;

    public float GetRandom()
    {
        float realMin = Mathf.Min(min, max);
        float realMax = Mathf.Max(min, max);

        return UnityEngine.Random.Range(realMin, realMax);
    }
}