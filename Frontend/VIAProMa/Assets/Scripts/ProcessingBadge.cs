using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingBadge : MonoBehaviour
{
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material processingMaterial;

    [SerializeField] float rotationSpeed = 0.5f;

    private Renderer rend;

    private bool isProcessing = true;

    public bool IsProcessing
    {
        get
        {
            return isProcessing;
        }
        set
        {
            isProcessing = value;
            if (isProcessing)
            {
                rend.material = processingMaterial;
            }
            else
            {
                rend.material = defaultMaterial;
            }
        }
    }

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (isProcessing)
        {
            rend.material.mainTextureOffset = new Vector2(0, (rend.material.mainTextureOffset.y - rotationSpeed * Time.deltaTime) % 1);
        }

        // TODO: remove debugging code
        if (Input.GetKeyDown(KeyCode.P))
        {
            IsProcessing = !IsProcessing;
        }
    }
}
