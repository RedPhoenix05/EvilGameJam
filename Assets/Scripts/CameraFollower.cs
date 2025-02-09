using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    Camera cam;
    [SerializeField] Transform root;
    void Start()
    {
        if (!root)
        {
            root = transform;
        }

        cam = root.GetComponentInChildren<Camera>();
        if (cam)
        {
            transform.parent = cam.transform;
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
