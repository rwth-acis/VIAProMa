using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectLoader : Shelf, ILoadShelf
{
    [SerializeField] GameObject filePrefab;
    [SerializeField] HorizontalObjectArray[] shelfBoards;

    [SerializeField] int filesPerBoard = 3;

    private string[] projects;
    private List<File> files;
    private List<Interactable> interactables;

    private int pageOffset = 0;

    public MessageBadge MessageBadge { get => messageBadge; }

    protected override void Awake()
    {
        base.Awake();
        if (filePrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(filePrefab));
        }
        if (shelfBoards == null || shelfBoards.Length == 0)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(shelfBoards));
        }
    }

    private void Start()
    {
        LoadContent();
    }

    public async void LoadContent()
    {
        messageBadge.gameObject.SetActive(true);
        messageBadge.ShowProcessing();
        ApiResult<string[]> projectRes = await BackendConnector.GetProjects();
        messageBadge.DoneProcessing();
        if (projectRes.Successful)
        {
            messageBadge.Hide();
            projects = projectRes.Value;

            InstantiateProjectRepresentations();
        }
        else
        {
            messageBadge.ShowMessage(projectRes.ResponseCode);
        }
    }

    private void InstantiateProjectRepresentations()
    {
        files = new List<File>();
        interactables = new List<Interactable>();

        for (int board = 0; board < shelfBoards.Length; board++)
        {
            int numberOnBoard = Mathf.Max(0, Mathf.Min(filesPerBoard, projects.Length - board * filesPerBoard));
            GameObject[] instances = new GameObject[numberOnBoard];
            for (int i = 0; i < numberOnBoard; i++)
            {
                instances[i] = Instantiate(filePrefab, shelfBoards[board].transform);
                File file = instances[i].GetComponent<File>();
                file.ProjectTitle = projects[i + board * filesPerBoard];
                file.ProjectLoader = this;
                files.Add(file);
                interactables.Add(instances[i].GetComponent<Interactable>());
            }
            shelfBoards[board].Collection = instances;
        }
    }

    public async void LoadProject(string name)
    {
        await BackendConnector.Load(name);
    }

    public void LockInteractables()
    {
        foreach(Interactable interactable in interactables)
        {
            interactable.Enabled = false;
        }
    }

    public void UnlockInteractables()
    {
        foreach(Interactable interactable in interactables)
        {
            interactable.Enabled = true;
        }
    }
}
