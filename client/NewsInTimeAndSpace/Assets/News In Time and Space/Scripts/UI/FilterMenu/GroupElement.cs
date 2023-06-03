using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements.Experimental;

/// <summary>
/// Group selection element for a FilterMenu3D. Can be used to visualize and interact with grouping of filters in that FilterMenu3D.
/// </summary>
public class GroupElement : UIButton
{
    public bool isSelected = false;
    public MeshRenderer boxRenderer;
    public FilterMenu3D filterMenu3DScript;
    public TextMeshPro textMeshPro;
    public Material selectedMaterial;
    private Material normalMaterial;

    // Start is called before the first frame update
    void Start()
    {
        normalMaterial = boxRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Checks if gameObject is selected by raycast and tells filterMenu.
    /// </summary>
    public override void onRaycastInteraction()
    {
        if (!isSelected)
        {
            isSelected = true;
            filterMenu3DScript.changeGroupSelection(gameObject);
        }
        else
        {
            isSelected = false;
            changeState(false);
            filterMenu3DScript.UpdateFiltersOnly();
        }

    }

    /// <summary>
    /// Outside call to change isSelected
    /// </summary>
    public void deselectSelf()
    {
        isSelected = false;
        changeState(false);
    }

    /// <summary>
    /// Change visible text.
    /// </summary>
    /// <param name="text"></param>
    public void changeText(string text)
    {
        //this.textMesh.text = text;
        this.textMeshPro.text = text;
    }

    /// <summary>
    /// Change isSelected state.
    /// </summary>
    /// <param name="isPressed"></param>
    public void changeState(bool isPressed)
    {
        this.isSelected = isPressed;

        if (isPressed)
            changeMaterial();
        else
            revertMaterial();
    }

    /// <summary>
    /// Change Material to selectedMaterial
    /// </summary>
    private void changeMaterial()
    {
        boxRenderer.material = selectedMaterial;
    }

    /// <summary>
    /// Revert Material to when gameObject was created.
    /// </summary>
    private void revertMaterial()
    {
        boxRenderer.material = normalMaterial;
    }
}
