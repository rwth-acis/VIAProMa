using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class which handles the processing effect
/// </summary>
public class ProcessingEffect : MonoBehaviour
{
    /// <summary>
    /// Default material which should be applied if no processing indication is used
    /// </summary>
    [SerializeField] Material defaultMaterial;
    /// <summary>
    /// Special material which is set up to display the processing effect
    /// </summary>
    [SerializeField] Material processingMaterial;

    /// <summary>
    /// Rotation speed of the processing effect
    /// </summary>
    [SerializeField] float rotationSpeed = 0.5f;

    private Renderer rend;

    private bool isProcessing;

    /// <summary>
    /// If set to true, the processing effect will be active
    /// </summary>
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
                rend.material.mainTextureOffset = Vector2.zero;
            }
            else
            {
                rend.material = defaultMaterial;
            }
        }
    }

    /// <summary>
    /// Initialization: gets the required components
    /// </summary>
    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (defaultMaterial == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(defaultMaterial));
        }
        if (processingMaterial == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(processingMaterial));
        }
    }

    /// <summary>
    /// Called every frame; scrolls the texture of the processing material if processing indication should be displayed
    /// </summary>
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
