using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

namespace i5.VIAProMa.UI.Chat
{
    public class UIHistory : MonoBehaviour, IWindow
    {
        [SerializeField] private TextMeshPro[] commandItemsText;
        [SerializeField] private GameObject[] commandItemsCubes;
        [SerializeField] private Interactable pageUpButton;
        [SerializeField] private Interactable pageDownButton;
        private UndoRedoManager UndoRedoManager;
        [SerializeField] private GameObject Leiste;
        [SerializeField] private FollowMeToggle LeisteFollowMeToggle;
        private GameObject UndoRedoManagerGameObject;
        private int numberOfTextFields;
        private List<ICommand> commands;
        private int lastCommandIndexChecker;
        private int lastCommandIndex;
        private int cubeIndex;
        private int currentTextMesh = 0;
        //private CommandProcessor commandProcessor;

        public bool WindowEnabled { get; set; }

        public bool WindowOpen { get; private set; }

        public event EventHandler WindowClosed;



        void Awake()
        { 
            UndoRedoManagerGameObject = GameObject.Find("UndoRedo Manager");
            UndoRedoManager = UndoRedoManagerGameObject.GetComponent<UndoRedoManager>();
            numberOfTextFields = commandItemsText.Length;
            lastCommandIndex = commands.Count;
        }


        void Update()
        {
            //lastCommandIndexChecker = commands.Count;
            commands = UndoRedoManager.getCommandList();
            //addCommandToUI();
            //lastCommandIndex = commands.Count;
            //addCommandToUI();
            /*if (lastCommandIndex < commands.Count)
            {
                lastCommandIndex = commands.Count;
                addCommandToUI();
            }*/
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

        public void ToggleFollowMeComponent()
        {
            LeisteFollowMeToggle.ToggleFollowMeBehavior();
        }

        public void Close()
        {
            WindowOpen = false;
            WindowClosed?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);
        }

        public void Undo()
        {
            UndoRedoManager.Undo();
        }

        public void Redo()
        {
            UndoRedoManager.Redo();
        }


        public void Select(int index)
        {
            //commandItemsText[index].GetComponent<TextMeshPro>().text = commands[0].GetType().ToString();
            cubeIndex = index;
            addCommandToUI();
        }


        public void scrollUp()
        {
            if (pageUpButton.IsEnabled)
            {
                UpdateView(true, false);
            }
        }

        public void addCommandToUI()
        {
            //commands = UndoRedoManager.getCommandList();
            commandItemsText[0].GetComponent<TextMeshPro>().text = commands[0].GetType().ToString();
            //currentTextMesh++;
        }


        public void scrollDown()
        {
            if (pageDownButton.IsEnabled)
            {
                UpdateView(false, true);
            }
        }

        public void UpdateView(bool up, bool down)
        {
            if (up)
            {

            }
            else if (down)
            {

            }
        }
    }
}