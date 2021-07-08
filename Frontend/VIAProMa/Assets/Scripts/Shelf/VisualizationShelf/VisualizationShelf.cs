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
        }

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