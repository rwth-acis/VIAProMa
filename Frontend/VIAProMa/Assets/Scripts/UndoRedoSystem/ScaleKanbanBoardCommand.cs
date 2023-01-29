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
    private Vector3 kanbanBoardColumnEndPosition;
    private i5.VIAProMa.Visualizations.KanbanBoard.KanbanBoardColumnVisualController kanbanBoardController;
    
    private float oldWidth;
    private float oldHeight;
    private float newWidth;
    private float newHeight;

    /// <summary>
    /// Creates a command for the action of resizing the Kanban Board.
    /// </summary>
    /// <param name="skanbanBoardColumnStartPosition"></param>
    /// <param name="sxAxis"></param>
    /// <param name="skanbanBoardController"></param>
    /// <param name="soldWidth"></param>
    /// <param name="soldHeight"></param>
    /// <param name="snewWidth"></param>
    /// <param name="snewHeight"></param>
    /// <param name="skanbanBoardColumnEndPosition"></param>
    public ScaleKanbanBoardCommand(Vector3 skanbanBoardColumnStartPosition, bool sxAxis, i5.VIAProMa.Visualizations.KanbanBoard.KanbanBoardColumnVisualController skanbanBoardController, float soldWidth, float soldHeight, float snewWidth, float snewHeight, Vector3 skanbanBoardColumnEndPosition)
    {
        xAxis = sxAxis;
        kanbanBoardColumnStartPosition = skanbanBoardColumnStartPosition;
        kanbanBoardColumnEndPosition = skanbanBoardColumnEndPosition;
        kanbanBoardController = skanbanBoardController;
        oldHeight = soldHeight;
        oldWidth = soldWidth;
        newHeight = snewHeight;
        newWidth = snewWidth;
    }

    public i5.VIAProMa.Visualizations.KanbanBoard.KanbanBoardColumnVisualController getKanbanBoardController()
    {
        return kanbanBoardController;
    }

    /// <summary>
    /// Resizes a Kanban Board by dragging the handles.
    /// </summary>
    public void Execute()
    {
        if (xAxis)
        {
            kanbanBoardController.Width = newWidth;
            kanbanBoardController.transform.localPosition = kanbanBoardColumnEndPosition;
        }
        else
        {
            kanbanBoardController.Height = newHeight;
            kanbanBoardController.transform.localPosition = kanbanBoardColumnEndPosition;
        }
    }

    /// <summary>
    /// Undos resizing of a Kanban Board.
    /// </summary>
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
