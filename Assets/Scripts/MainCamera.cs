using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private void SceneBehaviour_StarfighterCreated(object sender, SceneBehaviour.StarfighterCreatedEventArgs e)
    {
        e.starfighter.PositionChanged += Starfighter_OnPositionChanged;
        e.starfighter.RotationChanged += Starfighter_OnRotationChanged;
    }

    private void Starfighter_OnPositionChanged(object sender, Starfighter.PositionChangedEventArgs e)
    {
        Starfighter o = (Starfighter)sender;
        transform.position = new Vector3(o.cameraFollowSpeedFactor * o.transform.position.x, 
            o.cameraFollowSpeedFactor * o.transform.position.y, 
            transform.position.z);
    }

    private void Starfighter_OnRotationChanged(object sender, Starfighter.RotationChangedEventArgs e)
    {
        Starfighter o = (Starfighter)sender;
        float tiltAngle = -Mathf.Sign(o.TranslateVector.x) * Mathf.Lerp(0, o.cameraFollowSpeedFactor * 30, Mathf.Abs(o.TranslateVector.x));
        transform.rotation = Quaternion.AngleAxis(tiltAngle, Vector3.forward);
    }

    void Awake()
    {
        SceneBehaviour.Instance.StarfighterCreated += SceneBehaviour_StarfighterCreated;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
