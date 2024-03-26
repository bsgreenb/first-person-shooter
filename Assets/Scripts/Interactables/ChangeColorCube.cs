using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChangeColorCube : Interactable
{
    MeshRenderer mesh;
    public Color[] colors;
    private int colorIndex;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material.color = colorIndex.red;
    }

    protected override void Interact()
    {
        colorIndex++;
        if (colorIndex > colors.Length - 1) {
            colorIndex = 0;
        }

        mesh.material.color = colors[colorIndex];
    }

}