using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.UI.Chat
{
    public class UndoRedoMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject Leiste;
        [SerializeField] private FollowMeToggle LeisteFollowMeToggle;
        private GameObject CommandController;
        private CommandController commandController;
        private GameObject undoButtonBG;
        private GameObject redoButtonBG;
        private Color notActiveColor = Color.grey;

        public bool WindowEnabled { get; set; }

        public bool WindowOpen { get; private set; }

        public event EventHandler WindowClosed;

        public void Awake()
        {
            CommandController = GameObject.Find("CommandController");
            commandController = CommandController.GetComponent<CommandController>();
            undoButtonBG = GameObject.Find("UndoRedoMenu/Leiste/Backdrop/Undo Button/BackPlate/Quad");
            redoButtonBG = GameObject.Find("UndoRedoMenu/Leiste/Backdrop/Redo Button/BackPlate/Quad");
            undoButtonBG.GetComponent<Renderer>().material.color = notActiveColor;
            redoButtonBG.GetComponent<Renderer>().material.color = notActiveColor;
        }


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

        public void Undo()
        {
            commandController.Undo();
        }

        public void Redo()
        {

            commandController.Redo();
        }
    }
}