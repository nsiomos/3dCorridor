using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampedValue
{
    private float value;
    public float Value { get { return value; } set { this.value = Mathf.Clamp(value, MinValue, MaxValue); } }
    public float MinValue { get; }
    public float MaxValue { get; }

    public ClampedValue(float initialValue, float minValue, float maxValue)
    {        
        this.MinValue = minValue;
        this.MaxValue = maxValue;
        Value = initialValue;
    }
}
