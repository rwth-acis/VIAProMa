using System;
using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.Multiplayer.Poll;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.Shelves.PollShelf
{
    public class PollShelf : Shelf
    
    {
        [SerializeField] GameObject pollPrefab;
        [SerializeField] HorizontalObjectArray[] shelfBoards;

        [SerializeField] int pollsPerBoard = 3;

        [SerializeField] private GameObject boundingBox;
        // private List<PollObject> polls;
        private List<Interactable> interactables;
        private List<PollObject> pollObjects;

        protected override void Awake()
        {
            base.Awake();
            if(pollPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(pollPrefab));
            }
            if(shelfBoards == null || shelfBoards.Length == 0)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(shelfBoards));
            }
            PageChanged += onPageChange;
        }

        void Start()
        {
            InstantiatePollRepresentations();
            boundingBox.SetActive(false);
        }


        private void InstantiatePollRepresentations()
        {
            List<SerializablePoll> polls = PollHandler.Instance.savedPolls;
            interactables = new List<Interactable>();
            pollObjects = new List<PollObject>();
            int offset = Page*pollsPerBoard;
            for(int board = 0; board < shelfBoards.Length; board++)
            {
                int numberOnBoard = Mathf.Max(0, Mathf.Min(pollsPerBoard, polls.Count -offset - board * pollsPerBoard));
                GameObject[] instances = new GameObject[numberOnBoard];
                for(int i = 0; i <numberOnBoard; i++)
                {   
                    instances[i] = Instantiate(pollPrefab);
                    PollObject poll = instances[i].GetComponent<PollObject>();
                    poll.PollIndex = i+board*pollsPerBoard+offset;
                    pollObjects.Add(poll);
                    interactables.Add(instances[i].GetComponent<Interactable>());
                }
                shelfBoards[shelfBoards.Length - board - 1].Collection = instances;
            }
        }

        public void onPageChange(object sender, EventArgs e)
        {
            for(int boardIndex = 0; boardIndex < shelfBoards.Length; boardIndex++)
            {
                var board = shelfBoards[boardIndex].Collection; 
                for(int i = 0; i < board.Length; i ++)
                {
                    Destroy(board[i]);
                }
            }
            InstantiatePollRepresentations();
        }

        public void LockInteractables()
        {
            foreach (Interactable interactable in interactables)
            {
                interactable.IsEnabled = false;
            }
        }

        public void UnlockInteractables()
        {
            foreach (Interactable interactable in interactables)
            {
                interactable.IsEnabled = true;
            }
        }

        public void Close()
        {
            Destroy(gameObject);
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
