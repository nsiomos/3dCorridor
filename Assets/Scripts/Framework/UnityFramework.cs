using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityFramework : MonoBehaviour, IFramework
{
    public float TimeScale {
        get { return UnityEngine.Time.timeScale; }
        set { UnityEngine.Time.timeScale = value; }
    }

    public float Time
    {
        get { return UnityEngine.Time.time; }
    }

    public float DeltaTime
    {
        get { return UnityEngine.Time.deltaTime; }
    }


    public float GetInputAxis(string axisName)
    {
        return Input.GetAxis(axisName);
    }

    public bool GetInputButton(string buttonName)
    {
        return Input.GetButton(buttonName);
    }

    public bool GetInputButtonDown(string buttonName)
    {
        return Input.GetButtonDown(buttonName);
    }

    T IFramework.Instantiate<T>(T original, Vector3 position, Quaternion rotation)
    {
        return Instantiate<T>(original, position, rotation);
    }
}
