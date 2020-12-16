using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assigns a random color to the object's material
/// </summary>
[RequireComponent(typeof(Renderer))]
public class RandomColor : MonoBehaviour
{
    [SerializeField] private bool updateOnEnable = true;

    private Renderer rend;

    /// <summary>
    /// Gets the reference to the renderer and changes the material's color randomly
    /// </summary>
    private void Awake()
    {
        rend = GetComponent<Renderer>();

        rend.material.color = Random.ColorHSV();
    }

    /// <summary>
    /// If updateOnEnable is on, the color changed again randomly
    /// </summary>
    private void OnEnable()
    {
        if (updateOnEnable)
        {
            rend.material.color = Random.ColorHSV();
        }
    }
}
