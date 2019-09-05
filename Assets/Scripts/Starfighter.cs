﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO event unsubscribing on starfighter dying?
public class Starfighter : ActiveObject
{
    public class AccelerationResetLocalPositionZChangedEventArgs : EventArgs
    {
        public float prevLocalPositionZ;
    }

    public class PositionChangedEventArgs : EventArgs {
        public Vector3 prevPosition;
    }

    public class RotationChangedEventArgs : EventArgs
    {
        public Quaternion prevRotation;
    }

    private EventHandler<AccelerationResetLocalPositionZChangedEventArgs> accelerationResetLocalPositionZChanged;
    public event EventHandler<AccelerationResetLocalPositionZChangedEventArgs> AccelerationResetLocalPositionZChanged
    {
        add { accelerationResetLocalPositionZChanged += value; }
        remove { accelerationResetLocalPositionZChanged -= value; }
    }

    private EventHandler<PositionChangedEventArgs> positionChanged;
    public event EventHandler<PositionChangedEventArgs> PositionChanged
    {
        add { positionChanged += value; }
        remove { positionChanged -= value; }
    }

    private EventHandler<RotationChangedEventArgs> rotationChanged;
    public event EventHandler<RotationChangedEventArgs> RotationChanged
    {
        add { rotationChanged += value; }
        remove { rotationChanged -= value; }
    }

    private Transform firePositionMain;
    private Transform firePositionLeft;
    private Transform firePositionRight;

    public IFramework Framework { get; set; } = new UnityFramework();
    public bool IsPaused { get; set; } = false;
    public bool IsAccelerating { get; set; } = false;
    public Vector3 TranslateVector { get; set; }
    public float LastFiredTime { get; set; }


    public Projectile projectile;

    public float throttle;

    // ratio of sideward to forward movement at throttle 100, i.e. 1 means at throttle 100 we move 100 sidewards
    public float manouverabilityAt100;

    public float strafeFactor;
    public float moveBorderSoftDistance;
    public float moveBorderHardDistance;
    public float accelerateFactor;
    public ClampedValue accelerometer = new ClampedValue();
    public float accelerometerDepletionRate;
    public float accelerometerRefillRate;
    public float starfighterToLevelMoverRatioOfAcceleration;
    public float accelerationResetSpeedFactor;
    public float cameraFollowSpeedFactor;
    public float fireRate;

    protected virtual void OnAccelerationResetLocalPositionZChanged(AccelerationResetLocalPositionZChangedEventArgs e)
    {
        accelerationResetLocalPositionZChanged(this, e);
    }

    protected virtual void OnPositionChanged(PositionChangedEventArgs e)
    {
        positionChanged?.Invoke(this, e);
    }

    protected virtual void OnRotationChanged(RotationChangedEventArgs e)
    {
        rotationChanged?.Invoke(this, e);
    }

    protected override void Awake()
    {
        base.Awake();

        firePositionMain = transform.Find("FirePositionMain");
        firePositionLeft = transform.Find("FirePositionLeft");
        firePositionRight = transform.Find("FirePositionRight");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    private void TogglePause()
    {
        IsPaused = !IsPaused;
        Framework.TimeScale = (IsPaused) ? 0 : 1;
    }

    private void UpdateAccelerate(float accelerateAxis)
    {
        accelerometer = StarfighterLogic.GetAccelerometer(accelerometer, IsAccelerating, accelerometerDepletionRate, accelerometerRefillRate, Framework.DeltaTime);
        IsAccelerating = StarfighterLogic.GetIsAccelerating(IsAccelerating, accelerometer, accelerateAxis);
        if (!IsAccelerating && transform.localPosition.z != 0)
        {
            float prevLocalPositionZ = transform.localPosition.z;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,
                StarfighterLogic.GetAccelerationResetLocalPositionZ(transform.localPosition.z, throttle, starfighterToLevelMoverRatioOfAcceleration, accelerationResetSpeedFactor, Framework.DeltaTime));
            if (transform.localPosition.z != prevLocalPositionZ)
            {
                OnAccelerationResetLocalPositionZChanged(new AccelerationResetLocalPositionZChangedEventArgs() { prevLocalPositionZ = prevLocalPositionZ });
            }
        }
    }

    private void ResetTranslateVector()
    {
        TranslateVector = throttle * Vector3.forward;
    }

    private void ResetRotation()
    {
        Quaternion prevRotation = transform.rotation;
        transform.rotation = Quaternion.identity;
        OnRotationChanged(new RotationChangedEventArgs() { prevRotation = prevRotation });
    }

    private void UpdateTranslateVector(float horizontalAxis, float verticalAxis, float strafeAxis, float accelerateAxis)
    {
        TranslateVector = StarfighterLogic.GetTranslateVector(horizontalAxis, verticalAxis, strafeAxis, accelerateAxis,
            throttle, manouverabilityAt100, strafeFactor,
            IsAccelerating, accelerateFactor, accelerometer);
    }

    private void UpdatePosition()
    {
        Vector3 prevPosition = transform.position;
        transform.position = StarfighterLogic.GetPosition(transform.position, TranslateVector, throttle, starfighterToLevelMoverRatioOfAcceleration,
            Section.AddedQuadrantsEachHorizontalDirection, Section.AddedQuadrantsEachVerticalDirection, Quadrant.QuadrantSize,
            moveBorderSoftDistance, moveBorderHardDistance, Framework.DeltaTime);
        if (prevPosition != transform.position)
        {
            OnPositionChanged(new PositionChangedEventArgs() { prevPosition = prevPosition });
        }
    }

    private void UpdateRotation(float strafeAxis)
    {
        Quaternion prevRotation = transform.rotation;
        transform.rotation = StarfighterLogic.GetRotation(TranslateVector, strafeAxis);
        if (prevRotation != transform.rotation)
        {
            OnRotationChanged(new RotationChangedEventArgs() { prevRotation = prevRotation });
        }
    }

    private void Fire()
    {
        Projectile createdProjectile = Framework.Instantiate(projectile, firePositionMain.position, firePositionMain.rotation);
        createdProjectile.owner = Owner.Player;
        createdProjectile = Framework.Instantiate(projectile, firePositionLeft.position, firePositionLeft.rotation);
        createdProjectile.owner = Owner.Player;
        createdProjectile = Framework.Instantiate(projectile, firePositionRight.position, firePositionRight.rotation);
        createdProjectile.owner = Owner.Player;

        LastFiredTime = Framework.Time;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Framework.GetInputButtonDown(Constants.ButtonPause))
        {
            TogglePause();
        }

        if (IsPaused)
        {
            return;
        }

        base.Update();

        bool fireButton = Framework.GetInputButton(Constants.ButtonFire);
        float horizontalAxis = Framework.GetInputAxis(Constants.AxisHorizontal);
        float verticalAxis = Framework.GetInputAxis(Constants.AxisVertical);
        float strafeAxis = Framework.GetInputAxis(Constants.AxisStrafe);
        float accelerateAxis = Framework.GetInputAxis(Constants.AxisAccelerate);

        UpdateAccelerate(accelerateAxis);

        if (horizontalAxis == 0 && verticalAxis == 0 && strafeAxis == 0 && accelerateAxis == 0)
        {
            if (TranslateVector != throttle * Vector3.forward)
            {
                ResetTranslateVector();
            }
            if (transform.rotation != Quaternion.identity)
            {
                ResetRotation();
            }
        }
        else
        {
            UpdateTranslateVector(horizontalAxis, verticalAxis, strafeAxis, accelerateAxis);
            UpdatePosition();
            UpdateRotation(strafeAxis);
        }

        if (fireButton  && StarfighterLogic.CanFire(fireRate, LastFiredTime, Framework.Time))
        {
            Fire();
        }
    }
}
