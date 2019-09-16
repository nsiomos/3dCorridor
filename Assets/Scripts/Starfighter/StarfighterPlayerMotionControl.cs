using StarfighterDefinitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StarfighterPlayerMotionControl
{
    private Starfighter o;

    public StarfighterPlayerMotionControl(Starfighter o)
    {
        this.o = o;
    }

    public void UpdateAccelerationResetLocalPositionZ(float deltaTime)
    {
        Assert.AreEqual(AccelerationState.AccelerationResetting, o.AccelerationState);

        float forwardAccelerateSpeed = o.throttle * o.starfighterToLevelMoverRatioOfAcceleration;
        float accelerateResetSpeed = forwardAccelerateSpeed * o.accelerationResetSpeedFactor;
        o.transform.localPosition += -Mathf.Sign(o.transform.localPosition.z) * Mathf.Min(accelerateResetSpeed * deltaTime, Mathf.Abs(o.transform.localPosition.z))
            * Vector3.forward;
    }

    public void UpdateAccelerometer(float deltaTime)
    {
        if (o.AccelerationState != AccelerationState.Accelerating)
        {
            if (!o.accelerometer.IsAtMax())
            {
                o.accelerometer.Value += o.accelerometerRefillRate * deltaTime;
            }
        }
        else {
            if (!o.accelerometer.IsAtMin())
            {
                o.accelerometer.Value -= o.accelerometerDepletionRate * deltaTime;
            }
        }
    }

    public void UpdateAccelerationStateAfterAccelerometer(float accelerateAxis)
    {
        if (o.AccelerationState != AccelerationState.Accelerating)
        {
            if (o.accelerometer.IsAtMax() && accelerateAxis != 0)
            {
                o.AccelerationState = AccelerationState.Accelerating;
            }
        }
        else {
            if (o.accelerometer.IsAtMin() || accelerateAxis == 0)
            {
                o.AccelerationState = AccelerationState.AccelerationResetting;
            }
        }

    }

    public void UpdateAccelerationStateAfterAccelerationResetting()
    {
        Assert.AreEqual(AccelerationState.AccelerationResetting, o.AccelerationState);

        if (o.transform.localPosition.z == 0)
        {
            o.AccelerationState = AccelerationState.None;
        }
    }
    public Vector3 ClampToBorder(Vector3 deltaPosition)
    {
        float absTargetPositionX = Mathf.Abs(o.transform.position.x + deltaPosition.x);
        float softBorderX = (Section.AddedQuadrantsEachHorizontalDirection + 0.5f) * Quadrant.QuadrantSize - o.moveBorderSoftDistance;
        float hardBorderX = (Section.AddedQuadrantsEachHorizontalDirection + 0.5f) * Quadrant.QuadrantSize - o.moveBorderHardDistance;
        if (absTargetPositionX > hardBorderX)
        {
            deltaPosition.x -= Mathf.Sign(deltaPosition.x) * (absTargetPositionX - hardBorderX);
        }
        else if (absTargetPositionX > softBorderX
            && Mathf.Sign(deltaPosition.x) == Mathf.Sign(o.transform.position.x))
        {
            deltaPosition.x = Mathf.Lerp(deltaPosition.x, 0, 1 - (hardBorderX - absTargetPositionX) / (hardBorderX - softBorderX));
        }

        float absTargetPositionY = Mathf.Abs(o.transform.position.y + deltaPosition.y);
        float softBorderY = (Section.AddedQuadrantsEachVerticalDirection + 0.5f) * Quadrant.QuadrantSize - o.moveBorderSoftDistance;
        float hardBorderY = (Section.AddedQuadrantsEachVerticalDirection + 0.5f) * Quadrant.QuadrantSize - o.moveBorderHardDistance;
        if (absTargetPositionY > hardBorderY)
        {
            deltaPosition.y -= Mathf.Sign(deltaPosition.y) * (absTargetPositionY - hardBorderY);
        }
        else if (absTargetPositionY > softBorderY
            && Mathf.Sign(deltaPosition.y) == Mathf.Sign(o.transform.position.y))
        {
            deltaPosition.y = Mathf.Lerp(deltaPosition.y, 0, 1 - (hardBorderY - absTargetPositionY) / (hardBorderY - softBorderY));
        }

        return deltaPosition;
    }

    public void UpdateTranslateVector(float horizontalAxis, float verticalAxis, float strafeAxis, float accelerateAxis)
    {
        Vector3 translateVector = new Vector3(horizontalAxis, verticalAxis, 0);
        translateVector = Mathf.Max(Mathf.Abs(translateVector.x), Mathf.Abs(translateVector.y)) * translateVector.normalized;
        translateVector.z = 1;

        if (strafeAxis != 0)
        {
            if (Mathf.Sign(translateVector.x) == Mathf.Sign(strafeAxis))
            {
                translateVector.x *= o.strafeFactor * Mathf.Abs(strafeAxis);
            }
            else
            {
                translateVector.x *= 1 / o.strafeFactor * Mathf.Abs(strafeAxis);
            }
        }
        
        if (o.AccelerationState == AccelerationState.Accelerating)
        {
            float dampenFactor = Mathf.Lerp(0, 1, o.accelerometer.Value / o.accelerometer.maxValue);
            translateVector.z += (o.accelerateFactor - 1) * dampenFactor * accelerateAxis;
        }
        
        float forwardMoveSpeed = o.throttle;
        float sidewardMoveSpeed = o.manouverabilityAt100 * 100 * 100 / o.throttle;

        translateVector.x *= sidewardMoveSpeed;
        translateVector.y *= sidewardMoveSpeed;
        translateVector.z *= forwardMoveSpeed;

        o.TranslateVector = translateVector;
    }

    public float GetAcceleratePortionOfForwardTranslateVector()
    {
        return o.TranslateVector.z - (1 * o.throttle);
    }

    public void UpdatePosition(float deltaTime)
    {
        float starfighterPortionOfForwardTranslateVector = o.starfighterToLevelMoverRatioOfAcceleration * GetAcceleratePortionOfForwardTranslateVector();
        Vector3 deltaPosition = new Vector3(o.TranslateVector.x, o.TranslateVector.y, starfighterPortionOfForwardTranslateVector) * deltaTime;

        deltaPosition = ClampToBorder(deltaPosition);

        o.transform.position += deltaPosition;
    }

    public void UpdateRotation(float strafeAxis)
    {
        //TODO Look rotation viewing vector is zero
        //UnityEngine.Quaternion:LookRotation(Vector3)
        //StarfighterPlayerMotionControl: UpdateRotation(Single)(at Assets / Scripts / StarfighterPlayerMotionControl.cs:138)
        //Starfighter: UpdateRotation(Single)(at Assets / Scripts / Starfighter.cs:181)
        //Starfighter: Update()(at Assets / Scripts / Starfighter.cs:238)
        Quaternion lookRotation = Quaternion.LookRotation(o.TranslateVector);
        if (o.TranslateVector == Vector3.zero)
        {
            Debug.Log("****************************");
            Time.timeScale = 0;
        }

        float tiltAngle = Vector3.SignedAngle(o.TranslateVector.z * Vector3.forward,o.TranslateVector.z * Vector3.forward + o.TranslateVector.x * Vector3.right, Vector3.down);
        if (strafeAxis != 0)
        {
            tiltAngle = Mathf.Lerp(tiltAngle, -Mathf.Sign(strafeAxis) * 90, Mathf.Abs(strafeAxis));
        }

        Quaternion tiltRotation = Quaternion.AngleAxis(tiltAngle, Vector3.forward);

        o.transform.rotation = lookRotation * tiltRotation;
    }
}

