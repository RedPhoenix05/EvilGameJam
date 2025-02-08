using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    Camera cam;
    void Awake()
    {
        cam = GetComponentInParent<Camera>();
    }
    void Update()
    {
        transform.parent = cam.transform;
    }
}
