using UnityEngine;

public class AnimateCube : Interactable
{
    Animator animator;
    private string startPrompt;

    void Start()
    {
        // TODO: fix  " error CS0103: The name 'startMessage' does not exist in the current context"
        // animator = GetComponent<Animator>();
        // startMessage = promptMessage;
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Default")) {
            promptMessage = startPrompt;
        } else {
            promptMessage = "Animating..";
        }
    }

    protected override void Interact()
    {
        animator.Play("Spin");
    }
}