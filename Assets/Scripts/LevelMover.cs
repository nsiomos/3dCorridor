using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMover : MonoBehaviour
{
    private Starfighter starfighter;

    private void SceneBehaviour_StarfighterCreated(object sender, SceneBehaviour.StarfighterCreatedEventArgs e)
    {
        starfighter = e.starfighter;
        starfighter.AccelerationResetLocalPositionZChanged += Starfighter_OnAccelerationResetLocalPositionZChanged;
    }

    private void Starfighter_OnAccelerationResetLocalPositionZChanged(object sender, Starfighter.AccelerationResetLocalPositionZChangedEventArgs e)
    {
        Starfighter o = (Starfighter)sender;
        float starfighterDeltaZ = o.transform.position.z - e.prevLocalPositionZ;
        transform.position -= starfighterDeltaZ * Vector3.forward;
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
        if (starfighter != null)
        {
            float levelMoverPortionOfForwardTranslateVector = 1 * starfighter.throttle;

            if (starfighter.IsAccelerating)
            {
                // TODO starfighter logic used here?
                levelMoverPortionOfForwardTranslateVector += (1 - starfighter.starfighterToLevelMoverRatioOfAcceleration)
                    * StarfighterLogic.GetAcceleratePortionOfForwardTranslateVector(starfighter.TranslateVector, starfighter.throttle);
            }

            transform.position += levelMoverPortionOfForwardTranslateVector * Vector3.forward * Time.deltaTime;
        }
    }
}
