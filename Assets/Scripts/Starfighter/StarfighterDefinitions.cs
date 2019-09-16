using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarfighterDefinitions
{
    public enum AccelerationState
    {
        None,
        Accelerating,
        AccelerationResetting
    }

    public class AccelerometerChangedEventArgs : EventArgs
    {
        public float prevValue;
    }
    public class AccelerationResetLocalPositionZChangedEventArgs : EventArgs
    {
        public float prevLocalPositionZ;
    }

    public class PositionChangedEventArgs : EventArgs
    {
        public Vector3 prevPosition;
    }

    public class RotationChangedEventArgs : EventArgs
    {
        public Quaternion prevRotation;
    }
}
