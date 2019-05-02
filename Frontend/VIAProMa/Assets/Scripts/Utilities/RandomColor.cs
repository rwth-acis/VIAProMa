using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    [SerializeField] private bool updateOnEnable = true;

    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        rend.material.color = Random.ColorHSV();
    }

    private void OnEnable()
    {
        if (updateOnEnable)
        {
            rend.material.color = Random.ColorHSV();
        }
    }
}
