using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    public Vector3 angularVelocity = Vector3.zero;

    Vector3 lastPosition = Vector3.zero;
    Quaternion lastRotation = Quaternion.identity;

    protected virtual void FixedUpdate()
    {
        UpdateValues();

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    protected virtual void UpdateValues()
    {
        velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        Vector3 deltaRotation = (transform.rotation * Quaternion.Inverse(lastRotation)).eulerAngles;
        deltaRotation.x = Mathf.DeltaAngle(0f, deltaRotation.x);
        deltaRotation.y = Mathf.DeltaAngle(0f, deltaRotation.y);
        deltaRotation.z = Mathf.DeltaAngle(0f, deltaRotation.z);
        angularVelocity = (deltaRotation / Time.deltaTime) * Mathf.Deg2Rad;
    }

    public void Calculate()
    {
        UpdateValues();
    }
}
