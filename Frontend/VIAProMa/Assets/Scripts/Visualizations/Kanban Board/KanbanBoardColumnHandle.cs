using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.KanbanBoard
{
    public class KanbanBoardColumnHandle : MonoBehaviour, IMixedRealityPointerHandler
    {
        [SerializeField] private KanbanBoardColumnVisualController kanbanBoardController;

        public bool xAxis;
        public bool positiveEnd;

        private IMixedRealityPointer activePointer;
        private Vector3 pointerStartPosition;
        private Vector3 kanbanBoardColumnStartPosition;
        private float startLength;
        private GameObject CommandController;
        private CommandController commandController;


        private void Awake()
        {
            CommandController = GameObject.Find("CommandController");
            commandController = CommandController.GetComponent<CommandController>();

            if (kanbanBoardController == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(kanbanBoardController));
            }
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (activePointer == null && !eventData.used)
            {
                activePointer = eventData.Pointer;
                pointerStartPosition = activePointer.Position;
                kanbanBoardColumnStartPosition = kanbanBoardController.transform.localPosition;
                if (xAxis)
                {
                    startLength = kanbanBoardController.Width;
                }
                else
                {
                    startLength = kanbanBoardController.Height;
                }

                // Mark pointer data as used
                eventData.Use();
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            ICommand drag = new ScaleKanbanBoardCommand(eventData, startLength, kanbanBoardColumnStartPosition, pointerStartPosition, activePointer, xAxis, positiveEnd, kanbanBoardController);
            commandController.Execute(drag);
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                activePointer = null;
                eventData.Use();
            }
        }

        private void MoveForPivotScaling()
        {
            Vector3 objToPivot;
            if (xAxis)
            {
                objToPivot = new Vector3(kanbanBoardController.Width, 0, 0);
            }
            else
            {
                objToPivot = new Vector3(0, kanbanBoardController.Height, 0);
            }
            if (!positiveEnd)
            {
                objToPivot *= -1;
            }
        }
    }
}