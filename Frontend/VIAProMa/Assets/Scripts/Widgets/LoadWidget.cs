using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadWidget : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;

    private Animator fileAnimator;
    private Animator[] pageAnimators;
    private bool fileOpen = false;
    private Particles3D particles3D;

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

    private void OnFolderOpen()
    {
        // flag is set to open => means that the event is called when the folder has just opened
        if (fileOpen)
        {
            StartCoroutine(ActivatePages());
        }
    }

    private void OnFolderClosed()
    {

    }

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FileOpen = !FileOpen;
        }
    }
}
