using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFramework
{
    float TimeScale { get; set; }
    float Time { get; }
    float DeltaTime { get; }
    float GetInputAxis(string axisName);
    bool GetInputButton(string buttonName);
    bool GetInputButtonDown(string buttonName);


    T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object;
}
