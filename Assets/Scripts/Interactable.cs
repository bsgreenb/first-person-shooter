using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    
    // Add or remove an InteractionEvent component to this gameobject

    [SerializeField]
    public string promptMessage;

    public virtual string OnLook()
    {
        return promptMessage;
    }
    public void BaseInteract()
    {
        Interact();
    }
    protected virtual void Interact() {
        // to be overridden by subclasses
    }
}
