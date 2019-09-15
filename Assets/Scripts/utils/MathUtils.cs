using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
    public static float To180RangeAngle(float angle)
    {
        angle %= 360;
        if (angle >= 180 || angle < -180)
        {
            angle -= Mathf.Sign(angle) * 360;
        }

        return angle;
    }

    public static float AbsMin(float value, float minValue)
    {
        return Mathf.Sign(value) * Mathf.Min(Mathf.Abs(value), minValue);
    }
    public static float AbsMax(float value, float maxValue)
    {
        return Mathf.Sign(value) * Mathf.Max(Mathf.Abs(value), maxValue);
    }
}
