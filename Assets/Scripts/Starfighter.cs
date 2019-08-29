﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starfighter : ActiveObject
{
    private Transform firePositionMain;
    private Transform firePositionLeft;
    private Transform firePositionRight;

    private StarfighterLogic logic = new StarfighterLogic();

    public Projectile projectile;

    public float throttle = 100;

    // ratio of sideward to forward movement at throttle 100, i.e. 1 means at throttle 100 we move 100 sidewards
    public float manouverabilityAt100 = 0.33f;

    public float strafeFactor = 4;
    public float moveBorderSoftDistance = 30;
    public float moveBorderHardDistance = 10;
    public float cameraFollowSpeedFactor = 0.5f;
    public float fireRate;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        firePositionMain = transform.Find("FirePositionMain");
        firePositionLeft = transform.Find("FirePositionLeft");
        firePositionRight = transform.Find("FirePositionRight");
    }

    private void Move(Vector3 moveAxis, float strafeAxis)
    {
        transform.position = logic.GetPosition(transform.position, moveAxis, throttle, manouverabilityAt100, 
            Section.AddedQuadrantsEachHorizontalDirection, Section.AddedQuadrantsEachVerticalDirection, Quadrant.QuadrantSize,
            moveBorderSoftDistance, moveBorderHardDistance, Time.deltaTime);
        transform.rotation = logic.GetLookRotation(moveAxis, throttle, manouverabilityAt100) * logic.GetTiltRotation(moveAxis, strafeAxis, throttle, manouverabilityAt100);
        Camera.main.transform.position = logic.GetCameraPosition(transform.position, cameraFollowSpeedFactor);
        Camera.main.transform.rotation = logic.GetCameraTiltRotation(moveAxis, cameraFollowSpeedFactor);
    }

    private void Fire()
    {
        Projectile createdProjectile = Instantiate(projectile, firePositionMain.position, firePositionMain.rotation);
        createdProjectile.owner = Owner.Player;
        createdProjectile = Instantiate(projectile, firePositionLeft.position, firePositionLeft.rotation);
        createdProjectile.owner = Owner.Player;
        createdProjectile = Instantiate(projectile, firePositionRight.position, firePositionRight.rotation);
        createdProjectile.owner = Owner.Player;
        logic.Fire(Time.time);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Vector3 moveAxis = logic.GetMoveAxis(Input.GetAxis(Constants.AxisHorizontal), Input.GetAxis(Constants.AxisVertical), Input.GetAxis(Constants.AxisStrafe), strafeFactor);
        float strafeAxis = Input.GetAxis(Constants.AxisStrafe);
        if (moveAxis == Vector3.zero && strafeAxis == 0)
        {
            if (transform.rotation != Quaternion.identity)
            {
                transform.rotation = Quaternion.identity;
                Camera.main.transform.rotation = Quaternion.identity;
            }
        }
        else {
            Move(moveAxis, strafeAxis);
        }

        if (Input.GetButton(Constants.ButtonFire)
            && logic.CanFire(Time.time, fireRate))
        {
            Fire();
        }
    }
}