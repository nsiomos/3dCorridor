
using StarfighterDefinitions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO event unsubscribing on starfighter dying?
public class Starfighter : ActiveObject
{
    private EventHandler<AccelerometerChangedEventArgs> accelerometerChanged;
    public event EventHandler<AccelerometerChangedEventArgs> AccelerometerChanged
    {
        add { accelerometerChanged += value; }
        remove { accelerometerChanged -= value; }
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

    private StarfighterPlayerMotionControl motionControl;
    private StarfighterPlayerFireControl fireControl;
    private Transform firePositionMain;
    private Transform firePositionLeft;
    private Transform firePositionRight;

    public bool IsPaused { get; set; } = false;
    public AccelerationState AccelerationState { get; set; } = AccelerationState.None;
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
    public float initialStarfighterToLevelMoverRatioOfAcceleration;
    public float accelerateResetFactor;
    public float fireRate;

    protected virtual void OnAccelerometerChanged(AccelerometerChangedEventArgs e)
    {
        accelerometerChanged(this, e);
    }

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

        motionControl = new StarfighterPlayerMotionControl(this);
        fireControl = new StarfighterPlayerFireControl(this);

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
        framework.TimeScale = (IsPaused) ? 0 : 1;
    }

    private void UpdateAccelerate(float accelerateAxis)
    {
        float prevAccelerometerValue = accelerometer.Value;
        motionControl.UpdateAccelerometer(framework.DeltaTime);
        if (accelerometer.Value != prevAccelerometerValue)
        {
            OnAccelerometerChanged(new AccelerometerChangedEventArgs() { prevValue = prevAccelerometerValue });
        }

        motionControl.UpdateAccelerationStateAfterAccelerometer(accelerateAxis);
        /*
        if (AccelerationState == AccelerationState.AccelerationResetting)
        {
            float prevLocalPositionZ = transform.localPosition.z;
            motionControl.UpdateAccelerationResetLocalPositionZ(framework.DeltaTime);
            if (transform.localPosition.z != prevLocalPositionZ)
            {
                OnAccelerationResetLocalPositionZChanged(new AccelerationResetLocalPositionZChangedEventArgs() { prevLocalPositionZ = prevLocalPositionZ });
            }

            motionControl.UpdateAccelerationStateAfterAccelerationResetting();
        }*/
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
        motionControl.UpdateTranslateVector(horizontalAxis, verticalAxis, strafeAxis, accelerateAxis);
    }

    private void UpdatePosition()
    {
        Vector3 prevPosition = transform.position;
        float prevLocalPositionZ = transform.localPosition.z;
        motionControl.UpdatePosition(framework.DeltaTime);
        if (prevPosition != transform.position)
        {
            OnPositionChanged(new PositionChangedEventArgs() { prevPosition = prevPosition });
        }
        if (transform.localPosition.z != prevLocalPositionZ)
        {
            OnAccelerationResetLocalPositionZChanged(new AccelerationResetLocalPositionZChangedEventArgs() { prevLocalPositionZ = prevLocalPositionZ });
        }

        if (AccelerationState == AccelerationState.AccelerationResetting)
        {
            motionControl.UpdateAccelerationStateAfterAccelerationResetting();
        }
    }

    private void UpdateRotation(float strafeAxis)
    {
        Quaternion prevRotation = transform.rotation;
        motionControl.UpdateRotation(strafeAxis);
        if (prevRotation != transform.rotation)
        {
            OnRotationChanged(new RotationChangedEventArgs() { prevRotation = prevRotation });
        }
    }

    private void Fire()
    {
        Projectile createdProjectile = framework.Instantiate(projectile, firePositionMain.position, firePositionMain.rotation);
        createdProjectile.owner = Owner.Player;
        createdProjectile = framework.Instantiate(projectile, firePositionLeft.position, firePositionLeft.rotation);
        createdProjectile.owner = Owner.Player;
        createdProjectile = framework.Instantiate(projectile, firePositionRight.position, firePositionRight.rotation);
        createdProjectile.owner = Owner.Player;
        
        fireControl.UpdateLastFiredTime(framework.Time);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (framework.GetInputButtonDown(Constants.ButtonPause))
        {
            TogglePause();
        }

        if (IsPaused)
        {
            return;
        }

        base.Update();

        bool fireButton = framework.GetInputButton(Constants.ButtonFire);
        float horizontalAxis = framework.GetInputAxis(Constants.AxisHorizontal);
        float verticalAxis = framework.GetInputAxis(Constants.AxisVertical);
        float strafeAxis = framework.GetInputAxis(Constants.AxisStrafe);
        float accelerateAxis = framework.GetInputAxis(Constants.AxisAccelerate);

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

        if (fireButton  && fireControl.CanFire(framework.Time))
        {
            Fire();
        }
    }

    public float GetAcceleratePortionOfForwardTranslateVector()
    {
        return motionControl.GetAccelerateComponentOfForwardTranslateVector();
    }
}
