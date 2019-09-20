using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the load animation of a file
/// </summary>
public class LoadWidget : MonoBehaviour
{
    /// <summary>
    /// Available pages which can be animated
    /// </summary>
    [SerializeField] private GameObject[] pages;

    private Animator fileAnimator;
    private Animator[] pageAnimators;
    private bool fileOpen = false;
    private Particles3D particles3D;
    private HorizontalObjectArray objectArray;
    private RotateOnFocus rotateOnFocus;
    private File file;

    /// <summary>
    /// Gets the references to necessary components and checks the setup
    /// </summary>
    private void Awake()
    {
        fileAnimator = GetComponent<Animator>();
        pageAnimators = new Animator[pages.Length];
        particles3D = GetComponent<Particles3D>();
        rotateOnFocus = GetComponent<RotateOnFocus>();
        file = GetComponent<File>();

        if (pages.Length == 0)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pages));
        }
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(pages) + i);
            }
            else
            {
                pageAnimators[i] = pages[i].GetComponent<Animator>();
            }
        }
    }

    private void Start()
    {
        objectArray = transform.parent.gameObject.GetComponent<HorizontalObjectArray>();
    }

    /// <summary>
    /// True if the file is currently opened
    /// </summary>
    public bool FileOpen
    {
        get { return fileOpen; }
        set
        {
            fileOpen = value;
            fileAnimator.SetBool("File Open", value);
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i].SetActive(false);
            }
            particles3D.Spawning = fileOpen;
        }
    }

    /// <summary>
    /// Called if the folder has fully opened (i.e. after the animation has finished)
    /// The event is defined in the animation of the file
    /// </summary>
    private void OnFolderOpen()
    {
        // flag is set to open => means that the event is called when the folder has just opened
        if (fileOpen)
        {
            StartCoroutine(ActivatePages());
            Invoke("MoveBack", 4f);
        }
    }

    /// <summary>
    /// Called if the folder is fully closed (i.e. after the animation has finished)
    /// The event is defined in the animation of the file
    /// </summary>
    private void OnFolderClosed()
    {

    }

    /// <summary>
    /// Handles the activation and deactivation, as well as the animation of the pages
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivatePages()
    {
        int pageIndex = 0;
        while (fileOpen)
        {
            int secondPrevious = pageIndex - 2;
            if (secondPrevious < 0)
            {
                secondPrevious = pages.Length + secondPrevious;
            }
            pages[secondPrevious].SetActive(false);
            pageAnimators[secondPrevious].Play("Idle");
            pages[pageIndex].SetActive(true);
            pageAnimators[pageIndex].Play("Turn Page");
            pageIndex = (pageIndex + 1) % pages.Length;
            yield return new WaitForSeconds(2f);
        }
    }

    public void SelectFolder()
    {
        objectArray.enabled = false;
        rotateOnFocus.enabled = false;
        FileOpen = true;
        file.ProjectLoader.LockInteractables();
        StartCoroutine(Move(transform.localPosition, transform.localPosition + 0.5f * transform.up, 0.8f));
    }

    private IEnumerator Move(Vector3 startPosition, Vector3 endPosition, float targetTime, Action OnFinished = null)
    {
        float time = 0;
        while (time < targetTime)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time / targetTime);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endPosition;

        OnFinished?.Invoke();
    }

    private void MoveBack()
    {
        FileOpen = false;
        file.SelectFile();
        StartCoroutine(Move(transform.localPosition, transform.localPosition - 0.5f * transform.up, 0.8f, () =>
        {
            objectArray.enabled = true;
            rotateOnFocus.enabled = true;
            rotateOnFocus.ToStandardRotation();
            file.ProjectLoader.UnlockInteractables();
        }));
    }
}
