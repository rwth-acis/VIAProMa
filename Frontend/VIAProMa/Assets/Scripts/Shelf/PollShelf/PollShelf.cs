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

            for(int board = 0; board < shelfBoards.Length; board++)
            {
                int numberOnBoard = Mathf.Max(0, Mathf.Min(pollsPerBoard, polls.Count - board * pollsPerBoard));
                GameObject[] instances = new GameObject[numberOnBoard];
                for(int i = 0; i <numberOnBoard; i++)
                {
                    instances[i] = Instantiate(pollPrefab, shelfBoards[board].transform);
                    PollObject poll = instances[i].GetComponent<PollObject>();
                    poll.PollIndex = i;
                    pollObjects.Add(poll);
                    interactables.Add(instances[i].GetComponent<Interactable>());
                }
                shelfBoards[board].Collection = instances;
            }
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
