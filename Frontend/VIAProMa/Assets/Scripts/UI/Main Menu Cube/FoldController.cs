using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldController : MonoBehaviour
{
    [SerializeField] private Animator cubeAnimator;
    [SerializeField] private Transform cubePivot;
    [SerializeField] private Interactable onOffButton;

    public bool MenuOpen { get; private set; }

    private void Awake()
    {
        if(cubeAnimator == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(cubeAnimator));
        }
        if(cubePivot == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(cubePivot));
        }
        if (onOffButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(onOffButton));
        }
    }

    public void FoldCube()
    {
    }

    public void UnFoldCube()
    {
        Debug.Log("Unfolding cube");
        StartCoroutine(FadeSize(0.5f * Vector3.one, 0.5f, () =>
        {
            onOffButton.gameObject.SetActive(false);
            cubeAnimator.SetBool("MenuOpen", true);
        }));
        MenuOpen = true;
    }

    private IEnumerator FadeSize(Vector3 endSize, float fadeTime, Action OnFinished = null)
    {
        Vector3 startSize = cubePivot.localScale;
        float time = 0f;
        while(time < fadeTime)
        {
            cubePivot.localScale = Vector3.Lerp(startSize, endSize, time / fadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        cubePivot.localScale = endSize;
        OnFinished?.Invoke();
    }
}
