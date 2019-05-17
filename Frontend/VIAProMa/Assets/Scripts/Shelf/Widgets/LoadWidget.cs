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

    /// <summary>
    /// Gets the references to necessary components and checks the setup
    /// </summary>
    private void Awake()
    {
        fileAnimator = GetComponent<Animator>();
        pageAnimators = new Animator[pages.Length];
        particles3D = GetComponent<Particles3D>();

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
            yield return new WaitForSeconds(2f);
            if (fileOpen) // situation could have changed while waiting
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
            }
        }
    }

    private void Update()
    {
        // TODO: remove debug code
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FileOpen = !FileOpen;
        }
    }
}
