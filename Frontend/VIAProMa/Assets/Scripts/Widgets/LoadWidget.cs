using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadWidget : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;

    private Animator fileAnimator;
    private Animator[] pageAnimators;
    private bool fileOpen = false;

    private void Awake()
    {
        fileAnimator = GetComponent<Animator>();

        pageAnimators = new Animator[pages.Length];

        if (pages.Length == 0)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pages));
        }
        for (int i=0;i<pages.Length;i++)
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
            for(int i=0;i<pages.Length;i++)
            {
                pages[i].SetActive(false);
            }
        }
    }

    private void OnFolderClosed()
    {

    }

    private void OnFolderOpen()
    {
        // flag is set to open => means that the event is called when the folder has just opened
        if (fileOpen)
        {
            StartCoroutine(ActivatePages());
        }
    }
    
    private IEnumerator ActivatePages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            yield return new WaitForSeconds(i);
            pages[i].SetActive(true);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FileOpen = !FileOpen;
        }
    }
}
