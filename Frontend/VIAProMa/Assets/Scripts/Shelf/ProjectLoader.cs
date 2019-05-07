using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectLoader : MonoBehaviour
{
    [SerializeField] GameObject filePrefab;
    [SerializeField] ObjectArray[] shelfBoards;
    [SerializeField] MessageBadge messageBadge;

    [SerializeField] int filesPerBoard = 3;

    private string[] projects;
    private File[] files;

    private int pageOffset = 0;

    private void Start()
    {
        if (filePrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(filePrefab));
        }
        if (shelfBoards == null || shelfBoards.Length == 0)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(shelfBoards));
        }
        if (messageBadge == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(messageBadge));
        }
        LoadProjects();
    }

    public async void LoadProjects()
    {
        messageBadge.gameObject.SetActive(true);
        messageBadge.ShowProcessing();
        ApiResult<string[]> projectRes = await BackendConnector.GetProjects();
        messageBadge.DoneProcessing();
        if (projectRes.Successful)
        {
            messageBadge.gameObject.SetActive(false);
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
