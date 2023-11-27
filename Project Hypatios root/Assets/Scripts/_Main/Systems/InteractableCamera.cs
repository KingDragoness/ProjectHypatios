using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCamera : MonoBehaviour
{

    public InteractableObject currentInteractable;

    public static InteractableCamera instance;

    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        bool noInteract = true;
        RaycastHit hit;
        Ray ray = new Ray(Hypatios.MainCamera.transform.position, Hypatios.MainCamera.transform.forward); //Camera.main.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f,0));

        if (Physics.Raycast(ray, out hit, 6f))
        {
            var interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInParent<InteractableObject>();
            }

            if (interactable != null)
            {
                if (interactable.enabled == true)
                {
                    currentInteractable = interactable;
                    noInteract = false;
                }
            }
        }

        if (noInteract)
        {
            currentInteractable = null;
        }

  
    }

    public bool IsInteractContainer()
    {

        if (currentInteractable != null)
        {
            if (currentInteractable is Interact_Container)
                return true;
        }

        return false;
    }

    private void Update()
    {
        if (Time.timeScale <= 0) return;

        if (currentInteractable != null)
        {
            if (Hypatios.Input.Interact.triggered)
            {
                currentInteractable.Interact();
            }

        }
    }

}
