using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerometer : Speedometer
{
    public Vector3 acceleration = Vector3.zero;
    public Vector3 angularAcceleration = Vector3.zero;

    Vector3 lastVelocity = Vector3.zero;
    Vector3 lastAngularVelocity = Vector3.zero;

    protected override void FixedUpdate()
    {
        lastVelocity = velocity;
        lastAngularVelocity = angularVelocity;

        base.FixedUpdate();
    }

    protected override void UpdateValues()
    {
        base.UpdateValues();

        acceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;
        angularAcceleration = (angularVelocity - lastAngularVelocity) / Time.fixedDeltaTime;
    }
}
