using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using i5.VIAProMa.UI;
using i5.VIAProMa.Visualizations.KanbanBoard;

public class ScaleKanbanBoardCommand : ICommand
{
    public bool xAxis;
    public bool positiveEnd;

    private Vector3 pointerStartPosition;
    private Vector3 kanbanBoardColumnStartPosition;
    private i5.VIAProMa.Visualizations.KanbanBoard.KanbanBoardColumnVisualController kanbanBoardController;
    float oldWidth;
    float oldHeight;

    public ScaleKanbanBoardCommand(Vector3 skanbanBoardColumnStartPosition, bool sxAxis, i5.VIAProMa.Visualizations.KanbanBoard.KanbanBoardColumnVisualController skanbanBoardController, float soldWidth, float soldHeight)
    {
        xAxis = sxAxis;
        kanbanBoardColumnStartPosition = skanbanBoardColumnStartPosition;
        kanbanBoardController = skanbanBoardController;
        oldHeight = soldHeight;
        oldWidth = soldWidth;
    }

    public void Execute()
    {


    }



    // Setzt Breite/Höhe und Position zurück
        public void Undo()
    {
            if (xAxis)
            {
            kanbanBoardController.Width = oldWidth;
            kanbanBoardController.transform.localPosition = kanbanBoardColumnStartPosition;
            }
            else
            {
            kanbanBoardController.Height = oldHeight;
            kanbanBoardController.transform.localPosition = kanbanBoardColumnStartPosition;
            }
    }
}
