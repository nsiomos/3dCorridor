using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClampedValue
{
    public float minValue = float.MinValue;
    public float value;
    public float Value {
        get { return Mathf.Clamp(value, minValue, maxValue); }
        set { this.value = Mathf.Clamp(value, minValue, maxValue); }
    }
    public float maxValue = float.MaxValue;

    public ClampedValue()
    {
    }

    public ClampedValue(float value, float minValue, float maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.Value = value;
    }

    public ClampedValue(ClampedValue o) : this(o.Value, o.minValue, o.maxValue)
    {
    }

    public bool IsAtMin()
    {
        return Value == minValue;
    }

    public bool IsAtMax()
    {
        return Value == maxValue;
    }
}
