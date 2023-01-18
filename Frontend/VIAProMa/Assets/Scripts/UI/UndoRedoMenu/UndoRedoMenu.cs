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
        private Color activeColor;
        private Color notActiveColor = Color.grey;

        public bool WindowEnabled { get; set; }

        public bool WindowOpen { get; private set; }

        public event EventHandler WindowClosed;

        public void Awake()
        {
            CommandController = GameObject.Find("CommandController");
            commandController = CommandController.GetComponent<CommandController>();
            undoButtonBG = GameObject.Find("Undo Button/BackPlate/Quad");
            redoButtonBG = GameObject.Find("Redo Button/BackPlate/Quad");
            /* ColorBlock colorBlock = bButton.GetComponent<Button>().colors;
             colorBlock.normalColor = yourColor;
             bButton.GetComponent<Button>().colors = colorBlock;*/
            activeColor = undoButtonBG.GetComponent<Renderer>().material.color;
            
        }


        public void changeColor(bool undoable, bool redoable)
        {
            if(!undoable)
            {
                undoButtonBG.GetComponent<Renderer>().material.color = notActiveColor;
            }
            if (!redoable)
            {
                redoButtonBG.GetComponent<Renderer>().material.color = notActiveColor;
            }
            if(undoable)
            {
                undoButtonBG.GetComponent<Renderer>().material.color = activeColor;
            }
            if (redoable)
            {
                redoButtonBG.GetComponent<Renderer>().material.color = activeColor;
            }
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
            changeColor(false, false);
            commandController.Redo();
        }
    }
}