using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectLoader : MonoBehaviour
{
    [SerializeField] GameObject filePrefab;
    [SerializeField] ObjectArray[] shelfBoards;

    [SerializeField] int filesPerBoard = 3;

    private string[] projects;
    private File[] files;

    private int pageOffset = 0;

    private void OnEnable()
    {
        if (filePrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(filePrefab));
        }
        LoadProjects();
    }

    private async void LoadProjects()
    {
        ApiResult<string[]> projectRes = await BackendConnector.GetProjects();
        if (projectRes.Successful)
        {
            projects = projectRes.Value;
            files = new File[projects.Length];

            InstantiateProjectRepresentations();
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
