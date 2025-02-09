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
    [SerializeField] float throwVelocity = 20.0f;
    [SerializeField] float throwAngularVelocity = 6.0f;

    Interactable selectedItem;


    void Update()
    {
        if (interactionAction.action.WasPressedThisFrame())
        {
            bool valid = Physics.Raycast(interactionSource.position, interactionSource.forward, out RaycastHit rayInfo, maxDistance, interactionMask);
            Interactable item = valid ? rayInfo.transform.GetComponentInParent<Interactable>() : null;
            valid &= (item != null);
            if (valid) // Grabbing ground item
            {
                GrabItem(item);
            }
            else // Throwing hand item
            {
                if (selectedItem)
                {
                    TryGetComponent(out Rigidbody rb);
                    Vector3 velocity = (rb) ? ((interactionSource.forward * throwVelocity) + rb.velocity) : (interactionSource.forward * throwVelocity);
                    selectedItem.Throw(velocity, throwAngularVelocity);
                    selectedItem = null;
                }
            }
        }
    }

    public void GrabItem(Interactable item)
    {
        if (selectedItem)
        {
            selectedItem.Drop();
        }

        item.Grab(attachTransform);
        selectedItem = item;
    }
}
