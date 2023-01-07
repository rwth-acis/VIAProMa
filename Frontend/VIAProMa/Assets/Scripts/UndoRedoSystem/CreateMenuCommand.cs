using i5.VIAProMa.UI;
using UnityEngine;

public class CreateMenuCommand : ICommand
{
    private IWindow window;
    private Vector3 position;
    private Vector3 eulerAngles;

    public CreateMenuCommand(Vector3 pPosition, Vector3 pEulerAngles)
    {
        //window = pWindow;
        position = pPosition;
        eulerAngles = pEulerAngles;
    } 

    public void Execute()
    {
        WindowManager.Instance.UndoRedoMenu.Open(position, eulerAngles);
    }

    public void Undo()
    {
        WindowManager.Instance.UndoRedoMenu.Close();
    }
}
