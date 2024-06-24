using i5.VIAProMa.UI;
using UnityEngine;

/// <summary>
/// Command which opens a menu.
/// </summary>
public class CreateMenuCommand : ICommand
{
    private MenuType menuType;
    private Vector3 position;
    private Vector3 eulerAngles;

    public CreateMenuCommand(MenuType pMenuType, Vector3 pPosition, Vector3 pEulerAngles)
    {
        menuType = pMenuType;
        position = pPosition;
        eulerAngles = pEulerAngles;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Creates a Menu depending on the type of the menu. The menus are instanciated using the Windowmanager.
    /// </summary>
    public void Execute()
    {
        switch (menuType)
        {
            case MenuType.RoomMenu:
                WindowManager.Instance.RoomMenu.Open(position, eulerAngles);
                break;
            case MenuType.ServerStatusMenu:
                WindowManager.Instance.ServerStatusMenu.Open(position, eulerAngles);
                break;
            case MenuType.ChatMenu:
                WindowManager.Instance.ChatMenu.Open(position, eulerAngles);
                break;
            case MenuType.SaveProjectWindow:
                WindowManager.Instance.SaveProjectWindow.Open(position, eulerAngles);
                break;
            case MenuType.LoginMenu:
                WindowManager.Instance.LoginMenu.Open(position, eulerAngles);
                break;
            case MenuType.AnchorMenu:
                WindowManager.Instance.AnchorMenu.Open(position, eulerAngles);
                break;
            case MenuType.UndoRedoMenu:
                WindowManager.Instance.UndoRedoMenu.Open(position, eulerAngles);
                break;
            case MenuType.UIHistory:
                WindowManager.Instance.UIHistory.Open(position, eulerAngles);
                break;
        }
    }

    /// <summary>
    /// Closes the window.
    /// </summary>
    public void Undo()
    {
        switch (menuType)
        {
            case MenuType.RoomMenu:
                WindowManager.Instance.RoomMenu.Close();
                break;
            case MenuType.ServerStatusMenu:
                WindowManager.Instance.ServerStatusMenu.Close();
                break;
            case MenuType.ChatMenu:
                WindowManager.Instance.ChatMenu.Close();
                break;
            case MenuType.SaveProjectWindow:
                WindowManager.Instance.SaveProjectWindow.Close();
                break;
            case MenuType.LoginMenu:
                WindowManager.Instance.LoginMenu.Close();
                break;
            case MenuType.AnchorMenu:
                WindowManager.Instance.AnchorMenu.Close();
                break;
            case MenuType.UndoRedoMenu:
                WindowManager.Instance.UndoRedoMenu.Close();
                break;
            case MenuType.UIHistory:
                WindowManager.Instance.UIHistory.Close();
                break;
        }
    }
}