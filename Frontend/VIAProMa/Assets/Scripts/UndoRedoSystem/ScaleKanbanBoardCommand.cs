using UnityEngine;

/// <summary>
/// Command which scales the Kanban Board.
/// </summary>
public class ScaleKanbanBoardCommand : ICommand
{
    public bool xAxis;
    public bool positiveEnd;

    private Vector3 kanbanBoardColumnStartPosition;
    private Vector3 kanbanBoardColumnEndPosition;
    private i5.VIAProMa.Visualizations.KanbanBoard.KanbanBoardColumnVisualController kanbanBoardController;

    private float oldWidth;
    private float oldHeight;
    private float newWidth;
    private float newHeight;

    public ScaleKanbanBoardCommand(Vector3 sKanbanBoardColumnStartPosition, bool sXAxis, i5.VIAProMa.Visualizations.KanbanBoard.KanbanBoardColumnVisualController sKanbanBoardController, float sOldWidth, float sOldHeight, float sNewWidth, float sNewHeight, Vector3 sKanbanBoardColumnEndPosition)
    {
        xAxis = sXAxis;
        kanbanBoardColumnStartPosition = sKanbanBoardColumnStartPosition;
        kanbanBoardColumnEndPosition = sKanbanBoardColumnEndPosition;
        kanbanBoardController = sKanbanBoardController;
        oldHeight = sOldHeight;
        oldWidth = sOldWidth;
        newHeight = sNewHeight;
        newWidth = sNewWidth;
    }

    /* -------------------------------------------------------------------------- */

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
    /// Reverses resizing of a Kanban Board.
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

    /* -------------------------------------------------------------------------- */

    public i5.VIAProMa.Visualizations.KanbanBoard.KanbanBoardColumnVisualController getKanbanBoardController()
    {
        return kanbanBoardController;
    }
}
