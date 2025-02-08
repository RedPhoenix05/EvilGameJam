using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] LayerMask interactionMask;
    [SerializeField] InputActionReference interactionAction;
    [SerializeField] Transform interactionSource;
    [SerializeField] float maxDistance = 3.0f;
    [SerializeField] Transform attachTransform;


    void Update()
    {
        if (interactionAction.action.WasPressedThisFrame())
        {
            bool valid = Physics.Raycast(interactionSource.position, interactionSource.forward, out RaycastHit rayInfo, maxDistance, interactionMask);
            Interactable item = valid ? rayInfo.transform.GetComponentInParent<Interactable>() : null;
            valid &= (bool) item;
            if (valid)
            {
                GrabItem(item);
            }
        }
    }

    public void GrabItem(Interactable item)
    {
        item.transform.parent = attachTransform;
        item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
