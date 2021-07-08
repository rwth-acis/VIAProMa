using i5.VIAProMa.SaveLoadSystem.Core;
using i5.VIAProMa.Shelves.IssueShelf;
using i5.VIAProMa.Shelves.Widgets;
using i5.VIAProMa.UI.MessageBadge;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.WebConnection;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Shelves.ProjectLoadShelf
{
    public class ProjectLoader : Shelf, ILoadShelf
    {
        [SerializeField] GameObject filePrefab;
        [SerializeField] HorizontalObjectArray[] shelfBoards;

        [SerializeField] int filesPerBoard = 3;

        [SerializeField] private GameObject boundingBox;

        private string[] projects;
        private List<File> files;
        private List<Interactable> interactables;

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
            boundingBox.SetActive(false);
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
                messageBadge.TryAgainAction = LoadContent;
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
            await SaveLoadManager.Instance.LoadScene(name);
        }

        public void LockInteractables()
        {
            foreach (Interactable interactable in interactables)
            {
                interactable.Enabled = false;
            }
        }

        public void UnlockInteractables()
        {
            foreach (Interactable interactable in interactables)
            {
                interactable.Enabled = true;
            }
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void MoveShelf()
        {
            bool isActive = boundingBox.activeSelf;
            if (isActive)
            {
                boundingBox.SetActive(false);
            }
            else
            {
                boundingBox.SetActive(true);
            }
        }
    }
}