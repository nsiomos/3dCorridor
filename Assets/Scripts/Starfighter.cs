using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starfighter : ActiveObject
{
    private LevelMover levelMover;
    private Transform firePositionMain;
    private Transform firePositionLeft;
    private Transform firePositionRight;

    private bool isPaused = false;
    private StarfighterLogic logic = new StarfighterLogic();

    public Projectile projectile;

    public float throttle;

    // ratio of sideward to forward movement at throttle 100, i.e. 1 means at throttle 100 we move 100 sidewards
    public float manouverabilityAt100;

    public float strafeFactor;
    public float moveBorderSoftDistance;
    public float moveBorderHardDistance;
    public float accelerateFactor;
    public float maxAccelerometer;
    public float accelerometerDepletionRate;
    public float accelerometerRefillRate;
    public float starfighterToLevelMoverRatioOfAcceleration;
    public float accelerationResetSpeedFactor;
    public float cameraFollowSpeedFactor;
    public float fireRate;

    protected override void Awake()
    {
        base.Awake();

        levelMover = transform.parent.GetComponent<LevelMover>();
        firePositionMain = transform.Find("FirePositionMain");
        firePositionLeft = transform.Find("FirePositionLeft");
        firePositionRight = transform.Find("FirePositionRight");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        logic.InitAccelerate(maxAccelerometer);
    }

    private void Move(Vector3 moveAxis, float strafeAxis)
    {
        transform.position = logic.GetPosition(transform.position, moveAxis, throttle, manouverabilityAt100, 
            Section.AddedQuadrantsEachHorizontalDirection, Section.AddedQuadrantsEachVerticalDirection, Quadrant.QuadrantSize,
            moveBorderSoftDistance, moveBorderHardDistance, starfighterToLevelMoverRatioOfAcceleration, Time.deltaTime);
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

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = (isPaused) ? 0 : 1;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetButtonDown(Constants.ButtonPause))
        {
            TogglePause();
        }

        if (isPaused)
        {
            return;
        }

        transform.position = logic.UpdateAccelerate(transform.position, transform.localPosition, 
            Input.GetAxis(Constants.AxisAccelerate), throttle, starfighterToLevelMoverRatioOfAcceleration, accelerationResetSpeedFactor, 
            maxAccelerometer, accelerometerDepletionRate, accelerometerRefillRate, Time.deltaTime);

        Vector3 moveAxis = logic.GetMoveAxis(Input.GetAxis(Constants.AxisHorizontal), Input.GetAxis(Constants.AxisVertical), 
            Input.GetAxis(Constants.AxisStrafe), strafeFactor,
            Input.GetAxis(Constants.AxisAccelerate), accelerateFactor, 100);
        float strafeAxis = Input.GetAxis(Constants.AxisStrafe);
        float accelerateAxis = Input.GetAxis(Constants.AxisAccelerate);

        if (moveAxis == Vector3.forward && strafeAxis == 0 && accelerateAxis == 0)
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
        levelMover.transform.position = logic.GetLevelMoverPosition(levelMover.transform.position, moveAxis, throttle, starfighterToLevelMoverRatioOfAcceleration, Time.deltaTime);


        if (Input.GetButton(Constants.ButtonFire)
            && logic.CanFire(Time.time, fireRate))
        {
            Fire();
        }
    }
}
