using Alteruna;
using UnityEngine;

public class HideLocalAvatar : MonoBehaviour
{
    private Alteruna.Avatar _avatar;

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        // Check if this is the local player's avatar
        if (_avatar.IsOwner)
        {
            HideAvatar();
        }
    }

    void HideAvatar()
    {
        // Disable all mesh renderers on the object
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }
}