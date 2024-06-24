using Microsoft.MixedReality.Toolkit.UI;
using System;
using UnityEngine;

namespace i5.VIAProMa.UI.Chat
{
    /// <summary>
    /// Menu used to interact with the Undo-Redo System
    /// </summary>
    public class UndoRedoMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject Leiste;
        [SerializeField] private FollowMeToggle LeisteFollowMeToggle;
        [SerializeField] private Interactable uiHistoryButton;
        private GameObject UndoRedoManagerGameObject;
        private UndoRedoManager UndoRedoManager;


        public bool WindowEnabled { get; set; }

        public bool WindowOpen { get; private set; }

        public event EventHandler WindowClosed;

        public void Awake()
        {
            UndoRedoManagerGameObject = GameObject.Find("UndoRedo Manager");
            UndoRedoManager = UndoRedoManagerGameObject.GetComponent<UndoRedoManager>();
        }

        /* -------------------------------------------------------------------------- */

        public void Open()
        {
            gameObject.SetActive(true);
            WindowOpen = true;
            LeisteFollowMeToggle.SetFollowMeBehavior(false);
        }

        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.localPosition = position;
            transform.localEulerAngles = eulerAngles;
        }

        public void Close()
        {
            WindowOpen = false;
            WindowClosed?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);
        }

        public void ToggleFollowMeComponent()
        {
            LeisteFollowMeToggle.ToggleFollowMeBehavior();
        }

        public void ShowUIHistory()
        {
            ICommand createMenu = new CreateMenuCommand(MenuType.UIHistory, transform.position - 0.5f * transform.up, transform.localEulerAngles);
            UndoRedoManager.Execute(createMenu);
        }

        /// <summary>
        /// Reverses the current command
        /// </summary>
        public void Undo()
        {
            UndoRedoManager.Undo();
        }

        /// <summary>
        /// Repeats the next command
        /// </summary>
        public void Redo()
        {
            UndoRedoManager.Redo();
        }
    }
}