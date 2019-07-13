using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class CustomizationController : MonoBehaviour
{
    public AvatarConfigurationOption[] options;

    private SkinnedMeshRenderer skinnedRenderer;

    int index = 0;

    private void Awake()
    {
        skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            skinnedRenderer.sharedMesh = options[index].Mesh;
            skinnedRenderer.material = options[index].Material;
            index++;
            index %= options.Length;
        }
    }
}
