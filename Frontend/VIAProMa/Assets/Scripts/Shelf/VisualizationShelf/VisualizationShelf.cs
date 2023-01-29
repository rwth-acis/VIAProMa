using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Shelves.Visualizations
{
    public class VisualizationShelf : Shelf
    {
        public List<GameObject> visualizationWidgets;
        public int objectsPerBoard = 3;
        public int boards = 5;
        public float boardLength = 0.8f;

        [SerializeField] private GridObjectCollection shelfGrid;
        [SerializeField] private GameObject boundingBox;

        private GameObject UndoRedoManagerGameObject;
        private UndoRedoManager UndoRedoManager;
        private Vector3 startPosition;

        private List<GameObject> widgetInstances;

        protected override void Awake()
        {
            base.Awake();
            if (shelfGrid == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(shelfGrid));
            }

            widgetInstances = new List<GameObject>();

            for (int i = 0; i < visualizationWidgets.Count; i++)
            {
                GameObject instance = Instantiate(visualizationWidgets[i], shelfGrid.transform);
                widgetInstances.Add(instance);
            }

            DisplayWidgets();
            boundingBox.SetActive(false);
            UndoRedoManagerGameObject = GameObject.Find("UndoRedo Manager");
            UndoRedoManager = UndoRedoManagerGameObject.GetComponent<UndoRedoManager>();
        }

        /* -------------------------------------------------------------------------- */

        private void DisplayWidgets()
        {
            shelfGrid.Rows = boards;
            shelfGrid.CellWidth = boardLength / objectsPerBoard;

            int startIndex = page * boards * objectsPerBoard;
            int endIndex = startIndex + boards * objectsPerBoard - 1;
            ActivateRange(startIndex, endIndex);

            shelfGrid.UpdateCollection();
        }

        private void ActivateRange(int startIndex, int endIndex)
        {
            for (int i = 0; i < widgetInstances.Count; i++)
            {
                bool isActive = (i >= startIndex && i <= endIndex);
                widgetInstances[i].SetActive(isActive);
            }
        }

        public override void ScrollDown()
        {
            base.ScrollDown();
            DisplayWidgets();
        }

        public override void ScrollUp()
        {
            base.ScrollUp();
            DisplayWidgets();
        }

        public void Close()
        {
            // gameObject.SetActive(false);
            ICommand close = new DeleteObjectCommand(gameObject, null);
            UndoRedoManager.Execute(close);
        }

        public void MoveShelf()
        {
            bool isActive = boundingBox.activeSelf;
            if (isActive)
            {
                ICommand move = new MoveObjectCommand(startPosition, gameObject.transform.localPosition, gameObject);
                UndoRedoManager.Execute(move);
                boundingBox.SetActive(false);
            }
            else
            {
                startPosition = gameObject.transform.localPosition;
                boundingBox.SetActive(true);
            }
        }
    }
}