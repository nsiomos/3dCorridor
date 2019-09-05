using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StarfighterLogic
{

    public static float GetAccelerationResetLocalPositionZ(float localPositionZ, float throttle, float starfighterToLevelMoverRatioOfAcceleration, float accelerationResetSpeedFactor, float deltaTime)
    {
        float forwardAccelerateSpeed = throttle * starfighterToLevelMoverRatioOfAcceleration;
        float accelerateResetSpeed = forwardAccelerateSpeed * accelerationResetSpeedFactor;
        localPositionZ += -Mathf.Sign(localPositionZ) * Mathf.Min(accelerateResetSpeed * deltaTime, Mathf.Abs(localPositionZ));

        return localPositionZ;
    }

    public static ClampedValue GetAccelerometer(ClampedValue accelerometer, bool isAccelerating, float accelerometerDepletionRate, float accelerometerRefillRate, float deltaTime)
    {
        accelerometer = new ClampedValue(accelerometer);
        if (isAccelerating)
        {
            if (!accelerometer.IsAtMax())
            {
                accelerometer.Value += accelerometerRefillRate * deltaTime;
            }
        }
        else {
            if (!accelerometer.IsAtMin())
            {
                accelerometer.Value -= accelerometerDepletionRate * deltaTime;
            }
        }

        return accelerometer;
    }

    public static bool GetIsAccelerating(bool isAccelerating, ClampedValue accelerometer, float accelerateAxis)
    {
        if (!isAccelerating)
        {
            if (accelerometer.IsAtMax() && accelerateAxis != 0)
            {
                isAccelerating = true;
            }
        }
        else {
            if (accelerometer.IsAtMin() || accelerateAxis == 0)
            {
                isAccelerating = false;
            }
        }

        return isAccelerating;
    }

    private static Vector3 ClampToBorder(Vector3 position, Vector3 deltaPosition,
        int addedQuadrantsEachHorizontalDirection, int addedQuadrantsEachVerticalDirection, int quadrantSize,
        float moveBorderSoftDistance, float moveBorderHardDistance)
    {
        float absTargetPositionX = Mathf.Abs(position.x + deltaPosition.x);
        float softBorderX = (addedQuadrantsEachHorizontalDirection + 0.5f) * quadrantSize - moveBorderSoftDistance;
        float hardBorderX = (addedQuadrantsEachHorizontalDirection + 0.5f) * quadrantSize - moveBorderHardDistance;
        if (absTargetPositionX > hardBorderX)
        {
            deltaPosition.x -= Mathf.Sign(deltaPosition.x) * (absTargetPositionX - hardBorderX);
        }
        else if (absTargetPositionX > softBorderX
            && Mathf.Sign(deltaPosition.x) == Mathf.Sign(position.x))
        {
            deltaPosition.x = Mathf.Lerp(deltaPosition.x, 0, 1 - (hardBorderX - absTargetPositionX) / (hardBorderX - softBorderX));
        }

        float absTargetPositionY = Mathf.Abs(position.y + deltaPosition.y);
        float softBorderY = (addedQuadrantsEachVerticalDirection + 0.5f) * quadrantSize - moveBorderSoftDistance;
        float hardBorderY = (addedQuadrantsEachVerticalDirection + 0.5f) * quadrantSize - moveBorderHardDistance;
        if (absTargetPositionY > hardBorderY)
        {
            deltaPosition.y -= Mathf.Sign(deltaPosition.y) * (absTargetPositionY - hardBorderY);
        }
        else if (absTargetPositionY > softBorderY
            && Mathf.Sign(deltaPosition.y) == Mathf.Sign(position.y))
        {
            deltaPosition.y = Mathf.Lerp(deltaPosition.y, 0, 1 - (hardBorderY - absTargetPositionY) / (hardBorderY - softBorderY));
        }

        return deltaPosition;
    }

    public static Vector3 GetTranslateVector(float orizontalAxis, float verticalAxis, float strafeAxis, float accelerateAxis,
        float throttle, float manouverabilityAt100, float strafeFactor,
        bool isAccelerating, float accelerateFactor, ClampedValue accelerometer)
    {
        Vector3 translateVector = new Vector3(orizontalAxis, verticalAxis, 0);
        translateVector = Mathf.Max(Mathf.Abs(translateVector.x), Mathf.Abs(translateVector.y)) * translateVector.normalized;
        translateVector.z = 1;

        if (strafeAxis != 0)
        {
            if (Mathf.Sign(translateVector.x) == Mathf.Sign(strafeAxis))
            {
                translateVector.x *= strafeFactor * Mathf.Abs(strafeAxis);
            }
            else
            {
                translateVector.x /= strafeFactor * Mathf.Abs(strafeAxis);
            }
        }

        if (isAccelerating)
        {
            float dampenerFactor = Mathf.Lerp(0, 1, accelerometer.Value / accelerometer.maxValue);
            translateVector.z += (accelerateFactor - 1) * dampenerFactor * accelerateAxis;
        }

        float forwardMoveSpeed = throttle;
        float sidewardMoveSpeed = manouverabilityAt100 * 100 * 100 / throttle;

        translateVector.x *= sidewardMoveSpeed;
        translateVector.y *= sidewardMoveSpeed;
        translateVector.z *= forwardMoveSpeed;

        return translateVector;
    }

    public static float GetAcceleratePortionOfForwardTranslateVector(Vector3 translateVector, float throttle)
    {
        return translateVector.z - (1 * throttle);
    }

    public static Vector3 GetPosition(Vector3 position, Vector3 translateVector, float throttle, float starfighterToLevelMoverRatioOfAcceleration,
        int addedQuadrantsEachHorizontalDirection, int addedQuadrantsEachVerticalDirection, int quadrantSize, 
        float moveBorderSoftDistance, float moveBorderHardDistance, float deltaTime)
    {
        float starfighterPortionOfForwardTranslateVector = starfighterToLevelMoverRatioOfAcceleration * GetAcceleratePortionOfForwardTranslateVector(translateVector, throttle);
        Vector3 deltaPosition = new Vector3(translateVector.x, translateVector.y, starfighterPortionOfForwardTranslateVector) * deltaTime;

        deltaPosition = ClampToBorder(position, deltaPosition,
            addedQuadrantsEachHorizontalDirection, addedQuadrantsEachVerticalDirection, quadrantSize,
            moveBorderSoftDistance, moveBorderHardDistance);

        return position + deltaPosition;
    }

    public static Quaternion GetRotation(Vector3 translateVector, float strafeAxis)
    {
        Quaternion lookRotation = Quaternion.LookRotation(translateVector);

        float tiltAngle = Vector3.SignedAngle(translateVector.z * Vector3.forward,translateVector.z * Vector3.forward + translateVector.x * Vector3.right, Vector3.down);
        if (strafeAxis != 0)
        {
            tiltAngle = Mathf.Lerp(tiltAngle, -Mathf.Sign(strafeAxis) * 90, Mathf.Abs(strafeAxis));
        }

        Quaternion tiltRotation = Quaternion.AngleAxis(tiltAngle, Vector3.forward);

        return lookRotation * tiltRotation;
    }

    public static bool CanFire(float lastFiredTime, float fireRate, float time)
    {
        return time - lastFiredTime >= 1 / fireRate;
    }
}

