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
}
