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
                    selectedItem.Throw(interactionSource.forward * throwVelocity);
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
