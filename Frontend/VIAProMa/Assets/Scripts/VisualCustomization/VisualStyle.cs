using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VisualStyle : MonoBehaviour
{
    public string key;
    public List<VisualStyleVariant> variants;

    public void CreateVariant()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        var newVariant = new VisualStyleVariant();

        foreach (var renderer in renderers)
        {
            var materialAssigment = new VisualStyleVariant.MaterialAssignment(renderer);

            newVariant.AddMaterialAssignment(materialAssigment);
        }

        variants ??= new List<VisualStyleVariant>();
        variants.Add(newVariant);

        EditorUtility.SetDirty(this);
    }
}

[Serializable]
public class VisualStyleVariant
{
    [Serializable]
    public struct MaterialAssignment
    {
        public string name;
        public MeshRenderer renderer;
        public Material[] materials;

        public MaterialAssignment(MeshRenderer meshRenderer)
        {
            this.name = meshRenderer.name;
            renderer = meshRenderer;
            materials = meshRenderer.sharedMaterials;
        }
    }
    
    public string key;
    [SerializeField] private List<MaterialAssignment> assignments;

    public void AddMaterialAssignment(MaterialAssignment materialAssignment)
    {
        assignments ??= new List<MaterialAssignment>();
        assignments.Add(materialAssignment);
    }

    public void ApplyVariant()
    {
        foreach (var assignment in assignments)
        {
            assignment.renderer.materials = assignment.materials;
        }
    }
}

[CustomEditor(typeof(VisualStyle))]
public class VisualStyleEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VisualStyle myScript = (VisualStyle)target;
        if(GUILayout.Button("Create Variant"))
        {
            myScript.CreateVariant();
        }
    }
}

