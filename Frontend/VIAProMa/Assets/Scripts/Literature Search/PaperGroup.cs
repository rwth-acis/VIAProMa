using i5.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class PaperGroup : MonoBehaviour
    {
        [Tooltip("GameObject of the GridObjectCollection, displaying the papers.")]
        [SerializeField] private GameObject collection;
        [Tooltip("GameObject of the visual overlay for the group.")]
        [SerializeField] private GameObject overlay;
        [Tooltip("Prefab of the PaperGroupItem.")]
        [SerializeField] private GameObject itemPrefab;
        [Tooltip("TextMesh of the paper count text.")]
        [SerializeField] private TextMeshPro paperCountMesh;
        [Tooltip("TextMesh of the titles text.")]
        [SerializeField] private TextMeshPro titlesMesh;
        [Tooltip("Button to start the addition mode.")]
        [SerializeField] private Interactable additionModeButton;
        [Tooltip("Button to add the selected papers.")]
        [SerializeField] private Interactable addButton;
        [Tooltip("Button to cancel the addition mode.")]
        [SerializeField] private Interactable cancelButton;
        [Tooltip("Button to expand the papers.")]
        [SerializeField] private Interactable expandButton;
        [Tooltip("Button minimize the papers.")]
        [SerializeField] private Interactable minimizeButton;

        private GridObjectCollection _gridCollection;
        private List<GameObject> _children = new List<GameObject>();
        private int _paperCount = 0;
        private List<string> _titles = new List<string>();
        public int PaperCount 
        { 
            get 
            {
                return _paperCount;
            }
            private set 
            {
                _paperCount = value;
                paperCountMesh.text = value.ToString() + " papers";
            } 
        }
        /// <summary>
        /// Initializes the window and makes sure all UI Elements are referenced and the window is set up correctly.
        /// </summary>
        private void Awake()
        {
            if (collection == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(collection));
            }
            else
            {
                _gridCollection = collection.GetComponent<GridObjectCollection>();
            }
            if (overlay == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(overlay));
            }
            if (paperCountMesh == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(paperCountMesh));
            }
            if (titlesMesh == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(titlesMesh));
            }
            if (additionModeButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(additionModeButton));
            }
            if (addButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(addButton));
            }
            if (cancelButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(cancelButton));
            }
            if (itemPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(itemPrefab));
            }
        }

        /// <summary>
        /// Adds a paper to the collection if it is not contained already.
        /// </summary>
        /// <param name="paper">Paper to add.</param>
        public void AddPaper(Paper paper)
        {
            if (ContainsPaper(paper))
                return;
            GameObject paperItem = Instantiate(itemPrefab);
            paperItem.GetComponent<PaperDataDisplay>().Setup(paper);
            paperItem.transform.parent = collection.transform;
            _children.Add(paperItem);
            if(!(paper.Title?[0] is null))
            {
                AddTitle(paper.Title?[0]);
            }
            PaperCount++;
            _gridCollection.UpdateCollection();

        }

        /// <summary>
        /// Removes a paper from the collection;
        /// </summary>
        /// <param name="paper"></param>
        private void RemovePaper(Paper paper)
        {
            GameObject @object = RemoveFromChildren(paper);
            Destroy(@object);
            if (!(paper.Title?[0] is null))
            {
                RemoveTitle(paper.Title?[0]);
            }
            PaperCount--;
        }

        /// <summary>
        /// Removes a paper from the children.
        /// </summary>
        /// <param name="paper">Paper to remove.</param>
        private GameObject RemoveFromChildren(Paper paper)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i].GetComponent<PaperDataDisplay>().Content.Equals(paper))
                {
                    GameObject @object = _children[i];
                    _children.RemoveAt(i);
                    return @object;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if a paper is contained in the children.
        /// </summary>
        /// <param name="paper">Paper to check for.</param>
        /// <returns>true, if paper exists in children otherwise false.</returns>
        private bool ContainsPaper(Paper paper)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i].GetComponent<PaperDataDisplay>().Content.Equals(paper))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Expands the papers in the group.
        /// </summary>
        public void Expand()
        {
            collection.SetActive(true);
            foreach(Transform trans in collection.GetComponentInChildren<Transform>())
            {
                BoundsControl control = trans.gameObject.GetComponentInChildren<BoundsControl>();
                if(control != null)
                {
                    control.Active = false;
                }
            }
            expandButton.gameObject.SetActive(false);
            minimizeButton.gameObject.SetActive(true);
        }
        /// <summary>
        /// Minimizes the papers in the group.
        /// </summary>
        public void Minimize()
        {
            collection.SetActive(false);
            overlay.SetActive(true);
            BoundsControl control = GetComponentInChildren<BoundsControl>();
            control.Active = false;
            minimizeButton.gameObject.SetActive(false);
            expandButton.gameObject.SetActive(true);
        }
        /// <summary>
        /// Adds a title to the list.
        /// </summary>
        /// <param name="title">Title to add.</param>
        private void AddTitle(string title)
        {
            _titles.Add(title); 
            UpdateTitlesMesh();
        }

        /// <summary>
        /// Removes a title from the list.
        /// </summary>
        /// <param name="title">Title to remove.</param>
        private void RemoveTitle(string title)
        {
            _titles.Remove(title);
            UpdateTitlesMesh();
        }

        /// <summary>
        /// Updates the title list text.
        /// </summary>
        private void UpdateTitlesMesh()
        {
            if(_titles.Count == 0)
            {
                titlesMesh.text = "No papers added.";
                return;
            }
            string display = "";
            foreach (string t in _titles)
            {
                display += t + "\n";
            }
            titlesMesh.text = display;
        }

        /// <summary>
        /// Starts the addition mode.
        /// </summary>
        public void StartAdditionMode()
        {
            PaperSelectionManager.Instance.StartSelectionMode();
            addButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            additionModeButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Adds all selected papers not in the group and removes selected in the group from the group and ends selection mode.
        /// </summary>
        public void AddSelected()
        {
            PaperDataDisplay[] gameObjects = GameObject.FindObjectsOfType<PaperDataDisplay>();

            List<PaperDataDisplay> selectedDisplays = PaperSelectionManager.Instance.EndSelectionMode();
            foreach (PaperDataDisplay display in selectedDisplays)
            {
                GameObject @object = FindObjectWith(gameObjects, display);
                if(@object != null)
                {
                    if(@object.GetComponentInParent<GridObjectCollection>() == null)
                    {
                        AddPaper(display.Content);
                    }
                    else
                    {
                        RemovePaper(display.Content);
                    }
                }
                else
                {
                    Debug.Log(display.Content.DOI + " was null");
                }
            }
            EndAdditionMode();
        }

        /// <summary>
        /// Finds a specific data display in an array of display and returns its gameobject.
        /// </summary>
        /// <param name="array">Array to be searched.</param>
        /// <param name="display">Display to search for.</param>
        /// <returns>GameObject of the data display if it exists, null overwise.</returns>
        private GameObject FindObjectWith(PaperDataDisplay[] array, PaperDataDisplay display)
        {
            for(int i = 0; i < array.Length; i++)
            {
                if (array[i].Content.DOI.Equals(display.Content.DOI))
                {
                    return array[i].gameObject;
                }
            }
            return null;
        }

        /// <summary>
        /// Cancels the current selection and ends the addition mode.
        /// </summary> 
        public void CancelSelection()
        {
            PaperSelectionManager.Instance.EndSelectionMode();
            EndAdditionMode();
        }

        /// <summary>
        /// Ends the addition mode.
        /// </summary>
        private void EndAdditionMode()
        {
            addButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            additionModeButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// When the GameObject is destoried, all children are destroied too.
        /// </summary>
        private void OnDestroy()
        {
            for(int i = 0; i < _children.Count; i++)
            {
                Destroy(_children[i]);
            }
            Destroy(_gridCollection.gameObject);
        }
    }

}
