using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectLoader : Shelf, ILoadShelf
{
    [SerializeField] GameObject filePrefab;
    [SerializeField] HorizontalObjectArray[] shelfBoards;

    [SerializeField] int filesPerBoard = 3;

    private string[] projects;
    private File[] files;

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
            files = new File[projects.Length];

            InstantiateProjectRepresentations();
        }
        else
        {
            messageBadge.ShowMessage(projectRes.ResponseCode);
        }
    }

    private void InstantiateProjectRepresentations()
    {
        for (int board = 0; board < shelfBoards.Length; board++)
        {
            int numberOnBoard = Mathf.Max(0, Mathf.Min(filesPerBoard, projects.Length - board * filesPerBoard));
            GameObject[] instances = new GameObject[numberOnBoard];
            for (int i = 0; i < numberOnBoard; i++)
            {
                instances[i] = Instantiate(filePrefab);
                File file = instances[i].GetComponent<File>();
                file.ProjectTitle = projects[i + board * filesPerBoard];
            }
            shelfBoards[board].Collection = instances;
        }
    }
}
