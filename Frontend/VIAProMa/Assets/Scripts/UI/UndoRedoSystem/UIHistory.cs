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
        private int currentPosition;
        
        //new
        //used as a navigation window through the command list (always max 9 at a time are able to be displayed in the UI)
        int lowerRangeIndex = 0;
        int upperRangeIndex = 0;

        void Awake()
        {
            UndoRedoManagerGameObject = GameObject.Find("UndoRedo Manager");
            UndoRedoManager = UndoRedoManagerGameObject.GetComponent<UndoRedoManager>();
            numberOfTextFields = commandItemsText.Length;
        }

        void Update()
        {
            commands = UndoRedoManager.getCommandList();
            currentPosition = UndoRedoManager.getCurrentPosition();
            //until display is full with 9 commands
            if(commands.Count <= numberOfTextFields)
            {
                upperRangeIndex = commands.Count-1;
            }
            //updates what is currently shown, done by correctly setting the lower- & upperRangeIndex variable throughout the code
            showRange(lowerRangeIndex, upperRangeIndex);
            UpdateColor();
        }

        //-------------------------- for UIHistory display --------------------------

        // displays the commands in given range
        void showRange(int pFromIndex, int pTilIndex)
        {
                int i = 0;
                int j = pFromIndex;
                while (j <= pTilIndex)
                {
                    commandItemsText[i].GetComponent<TextMeshPro>().text = commands[j].GetType().ToString();
                    i++;
                    j++;
                }
        }

        // up button
        void scrollDown()
        {
            // upperRangeIndex < commands.Count
            if (upperRangeIndex < commands.Count - 1)
            {
                lowerRangeIndex++;
                upperRangeIndex++;
            }
        }

        // down button
        void scrollUp()
        {
            if (lowerRangeIndex > 0)
            {
                lowerRangeIndex--;
                upperRangeIndex--;
            }
        }

        // called when cube button is pressed in UI meaning that the steps until that point will be undone
        void Select(int selectedCubeIndex)
        {
            // calculates position in command list
            int stepsToUndo = commands.Count - (lowerRangeIndex + selectedCubeIndex);
            for (int i = 0; i<= stepsToUndo; i++)
            {
                UndoRedoManager.Undo();
            }
            // repositions currentPosition in CommandProcessor
            UndoRedoManager.setCurrentPosition(stepsToUndo);
        }

        /// <summary>
        /// Checks for all commands which are currently visible in the ItemsText list if they contain the currently selected command.
        /// If it is not currently selected, the text will be colored white, else it will be red.
        /// </summary>
        private void UpdateColor() {
            for (int i = 0; i < commandItemsText.Length; i++)
            {
                if (i != currentPosition - lowerRangeIndex)
                {
                    commandItemsText[i].GetComponent<TextMeshPro>().color = Color.white;
                }
                else if (0 <= i && i <= 8)
                {
                    commandItemsText[i].GetComponent<TextMeshPro>().color = Color.red;
                }
            }
        }

        //---------------------------- for window interface ---------------------
        public bool WindowEnabled { get; set; }

        public bool WindowOpen { get; private set; }

        public event EventHandler WindowClosed;

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
    }
}