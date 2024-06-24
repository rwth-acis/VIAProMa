using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using System.Linq;

namespace i5.VIAProMa.UI.Chat
{
    /// <summary>
    /// Menu which contains a list of the commands. Used to skip between these.
    /// </summary>
    public class UIHistory : MonoBehaviour, IWindow
    {
        [SerializeField] private TextMeshPro[] commandItemsText;
        [SerializeField] private GameObject[] commandItemsCubes;
        [SerializeField] private Interactable pageUpButton;
        [SerializeField] private Interactable pageDownButton;

        [SerializeField] private GameObject Leiste;
        [SerializeField] private FollowMeToggle LeisteFollowMeToggle;

        private GameObject UndoRedoManagerGameObject;
        private UndoRedoManager UndoRedoManager;

        private List<ICommand> commands;
        private List<ICommand> tempCommands;
        private int currentPosition;
        // Used as a navigation window through the command list (max 9 at a time are able to be displayed in the UI).
        int lowerRangeIndex = 0;

        void Start()
        {
            UndoRedoManagerGameObject = GameObject.Find("UndoRedo Manager");
            UndoRedoManager = UndoRedoManagerGameObject.GetComponent<UndoRedoManager>();

            commands = UndoRedoManager.getCommandList();
        }

        void Update()
        {
            tempCommands = commands;
            commands = UndoRedoManager.getCommandList();
            // checks if a new command has been executed
            if (!commands.SequenceEqual(tempCommands))
            {
                if (commands.Count > 8)
                {
                    lowerRangeIndex = commands.Count - 9;
                }
                else
                {
                    lowerRangeIndex = 0;
                }
            }

            currentPosition = UndoRedoManager.getCurrentPosition();
            ShowRange(lowerRangeIndex);
            UpdateColor();
        }

        /* -------------------------------------------------------------------------- */

        /// <summary>
        /// Used to display the commands from the given Index in the list.
        /// </summary>
        /// <param name="pFromIndex">Index of the first command.</param>
        private void ShowRange(int pFromIndex)
        {
            for (int i = 0; i <= 8; i++)
            {
                if (pFromIndex < commands.Count)
                {
                    commandItemsText[i].GetComponent<TextMeshPro>().text = (pFromIndex + 1) + ": " + CreateDescription(pFromIndex);
                }
                else
                {
                    commandItemsText[i].GetComponent<TextMeshPro>().text = "";
                }
                pFromIndex++;
            }
        }

        /// <summary>
        /// Creates a description of the commandto be shown in the UI.
        /// </summary>
        /// <param name="i">
        /// Index of selected command in commands list.
        /// </param>
        /// <returns>
        /// The description of the command.
        /// </returns>
        private string CreateDescription(int i)
        {
            ICommand command = commands[i];
            Type type = command.GetType();

            if (type == typeof(AppBarTransformCommand))
            {
                return "Transformed App Bar";
            }
            else if (type == typeof(CreateMenuCommand))
            {
                return "Created Menu";
            }
            else if (type == typeof(ProgressBarHandleCommand))
            {
                return "Moved Progress Bar Handle";
            }
            else if (type == typeof(ScaleKanbanBoardCommand))
            {
                return "Scaled Kanban Board";
            }
            else if(type == typeof(DeleteObjectCommand))
            {
                return "Removed Object";
            }
            else if(type == typeof(MoveObjectCommand))
            {
                return "Moved Object";
            }
            else if(type == typeof(InitiateObjectCommand))
            {
                return "Initiated Object";
            }
            else
            {
                return "Executed " + commands[i].GetType().ToString();
            }
        }

        /// <summary>
        ///  Used by the "Down" button to scroll down the displayed command list in the UI.
        /// </summary>
        public void ScrollDown()
        {
            if (lowerRangeIndex + 8 < commands.Count - 1)
            {
                lowerRangeIndex++;
            }
        }

        /// <summary>
        ///  Used by the "Up" button to scroll up the displayed command list in the UI.
        /// </summary>
        public void ScrollUp()
        {
            if (lowerRangeIndex > 0)
            {
                lowerRangeIndex--;
            }
        }

        /// <summary>
        /// Called when a selection cube is pressed in order to calculate how many steps have to be undone.
        /// </summary>
        /// <param name="selectedCubeIndex">Selected Cube to which point commands need to be undo/redo.</param>
        public void Select(int selectedCubeIndex)
        {
            int amountOfSteps = currentPosition - (selectedCubeIndex + lowerRangeIndex);
            // if commands need to be reversed
            if (amountOfSteps > 0)
            {
                for (int i = 0; i < amountOfSteps; i++)
                {
                    UndoRedoManager.Undo();
                }
                UndoRedoManager.setCurrentPosition(currentPosition - amountOfSteps);
            }
            // if commands need to be repeated
            else if (amountOfSteps < 0)
            {
                for (int i = 0; i < -amountOfSteps; i++)
                {
                    UndoRedoManager.Redo();
                }
                UndoRedoManager.setCurrentPosition(currentPosition + -amountOfSteps);
            }
        }

        /// <summary>
        /// Sets the color of the TextMeshPro Components. Blue if current command, else white.
        /// </summary>
        private void UpdateColor()
        {
            for (int i = 0; i < commandItemsText.Length; i++)
            {
                if (i != currentPosition - lowerRangeIndex)
                {
                    commandItemsText[i].GetComponent<TextMeshPro>().color = Color.white;
                }
                else if (0 <= i && i <= 8)
                {
                    commandItemsText[i].GetComponent<TextMeshPro>().color = Color.blue;
                }
            }
        }

        /* -------------------------------------------------------------------------- */

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