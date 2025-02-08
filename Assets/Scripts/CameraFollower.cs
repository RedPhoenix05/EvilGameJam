using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    Camera cam;
    [SerializeField] Transform root;
    void Update()
    {
        if (!root)
        {
            root = transform;
        }

        cam = root.GetComponentInChildren<Camera>();
        transform.parent = cam.transform;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
