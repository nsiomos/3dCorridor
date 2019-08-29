using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarfighterLogic
{
    private float lastFiredTime = -float.MaxValue;

    private Vector3 ClampToBorder(Vector3 position, Vector3 deltaPosition, 
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

    public Vector3 GetMoveAxis(float inputAxisHorizontal, float inputAxisVertical, float inputAxisStrafe, float strafeFactor)
    {
        Vector3 moveAxis = new Vector3(inputAxisHorizontal, inputAxisVertical, 0);
        if (moveAxis == Vector3.zero && inputAxisStrafe == 0)
        {
            return Vector3.zero;
        }

        moveAxis = Mathf.Max(Mathf.Abs(moveAxis.x), Mathf.Abs(moveAxis.y)) * moveAxis.normalized;
        if (inputAxisStrafe != 0)
        {
            if (Mathf.Sign(moveAxis.x) == Mathf.Sign(inputAxisStrafe))
            {
                moveAxis.x *= strafeFactor * Mathf.Abs(inputAxisStrafe);
            }
            else
            {
                moveAxis.x /= strafeFactor * Mathf.Abs(inputAxisStrafe);
            }
        }

        return moveAxis;
    }

    public Vector3 GetPosition(Vector3 position, Vector3 moveAxis, float throttle, float manouverabilityAt100,
        int addedQuadrantsEachHorizontalDirection, int addedQuadrantsEachVerticalDirection, int quadrantSize, 
        float moveBorderSoftDistance, float moveBorderHardDistance, float deltaTime)
    {
        float sidewardsMoveSpeed = manouverabilityAt100 * 100 * 100 / throttle;

        Vector3 deltaPosition = moveAxis * sidewardsMoveSpeed * deltaTime;
        deltaPosition = ClampToBorder(position, deltaPosition, 
            addedQuadrantsEachHorizontalDirection, addedQuadrantsEachVerticalDirection, quadrantSize, 
            moveBorderSoftDistance, moveBorderHardDistance);
        deltaPosition.z = 0;

        return position + deltaPosition;
    }

    public Quaternion GetLookRotation(Vector3 moveAxis, float throttle, float manouverabilityAt100)
    {
        float sidewardsMoveSpeed = manouverabilityAt100 * 100 * 100 / throttle;
        return Quaternion.LookRotation(throttle * Vector3.forward + sidewardsMoveSpeed * moveAxis);
    }

    public Quaternion GetTiltRotation(Vector3 moveAxis, float inputAxisStrafe, float throttle, float manouverabilityAt100)
    {
        Vector3 sidewardsMove = manouverabilityAt100 * 100 * 100 / throttle * moveAxis.x * Vector3.right;
        Vector3 forwardMove = throttle * Vector3.forward;
        float tiltAngle = Vector3.SignedAngle(forwardMove, forwardMove + sidewardsMove, Vector3.down);

        //float tiltAngle = -Mathf.Sign(moveAxis.x) * Mathf.Lerp(0, maxTiltAngle, Mathf.Abs(moveAxis.x));
        if (inputAxisStrafe != 0)
        {
            tiltAngle = Mathf.Lerp(tiltAngle, -Mathf.Sign(inputAxisStrafe) * 90, Mathf.Abs(inputAxisStrafe));
        }

        return Quaternion.AngleAxis(tiltAngle, Vector3.forward);
    }

    public Vector3 GetCameraPosition(Vector3 position, float cameraFollowSpeedFactor)
    {
        return new Vector3(cameraFollowSpeedFactor * position.x, cameraFollowSpeedFactor * position.y, Camera.main.transform.position.z);
    }

    public Quaternion GetCameraTiltRotation(Vector3 moveAxis, float cameraFollowSpeedFactor)
    {
        float cameraTiltAngle = -Mathf.Sign(moveAxis.x) * Mathf.Lerp(0, cameraFollowSpeedFactor * 30, Mathf.Abs(moveAxis.x));
        return Quaternion.AngleAxis(cameraTiltAngle, Vector3.forward);
    }

    public bool CanFire(float time, float fireRate)
    {
        return time - lastFiredTime >= 1 / fireRate;
    }

    public void Fire(float time)
    {
        lastFiredTime = time;
    }

}
