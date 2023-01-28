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
        private int lastCommandIndex = 0;
        private int selectedCubeIndex;
        private int currentTextMesh = 0;
        private int stoppedAtIndex = 0;
        private int currentCommandIndex;
        private int lowerRangeIndex = 0;
        private int upperRangeIndex = 0;
        private bool fillUI = false;
        //private CommandProcessor commandProcessor;

        public bool WindowEnabled { get; set; }

        public bool WindowOpen { get; private set; }

        public event EventHandler WindowClosed;

        void Update()
        {
            commands = UndoRedoManager.getCommandList();
            if(fillUI == false)
            {
                fillWithExistingCommands();
            }
            if (lastCommandIndex < commands.Count)
            {
                lastCommandIndex = commands.Count;
                addCommandToUI();
            }
        }


        void Awake()
        {
            UndoRedoManagerGameObject = GameObject.Find("UndoRedo Manager");
            UndoRedoManager = UndoRedoManagerGameObject.GetComponent<UndoRedoManager>();
            numberOfTextFields = commandItemsText.Length;
            //lastCommandIndex = 0;
            currentTextMesh = 0;
            fillUI = false;
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


        //Frage: wird currentPosition in CommandProcessor schon korrekt verschoben durch die Undo und Redo Aufrufe oder muss ich hier etwas manuell machen????


        //NICHT Löschen!!!!!!!! Nur CurrentPosition verschieben, Historie will man ja behalten außer bei RemoveRange
        public void Select(int index)
        {
            selectedCubeIndex = index;
            //NOTE when adding more textfields: TextFields in Editor prefilled with " " to check if they're empty
            if (upperRangeIndex >= numberOfTextFields && commandItemsText[selectedCubeIndex].GetComponent<TextMeshPro>().text != " ")
            {
                int indexInList = lowerRangeIndex + selectedCubeIndex;
                int stepsToUndo = lastCommandIndex - indexInList;
                for (int i = 0; i <= stepsToUndo; i++)
                {
                    
                    UndoRedoManager.Undo();
                }
               /* for (int i = selectedCubeIndex; i <= upperRangeIndex; i++)
                {
                    commandItemsText[i].GetComponent<TextMeshPro>().text = "";
                }*/
                upperRangeIndex = (lowerRangeIndex + selectedCubeIndex) - 1;
            }
            else if(upperRangeIndex <= numberOfTextFields && commandItemsText[selectedCubeIndex].GetComponent<TextMeshPro>().text != " ")
            {
                int stepsToUndo = upperRangeIndex - selectedCubeIndex;
                for (int i = 0; i <= stepsToUndo; i++)
                {
                    UndoRedoManager.Undo();
                }
            }

        }



        public void addCommandToUI()
        {
            //commands = UndoRedoManager.getCommandList();
            if (lastCommandIndex - 1 <= numberOfTextFields || upperRangeIndex <= numberOfTextFields)
            {
                commandItemsText[currentTextMesh].GetComponent<TextMeshPro>().text = commands[lastCommandIndex - 1].GetType().ToString();
                currentTextMesh++;
                stoppedAtIndex++;
                if (upperRangeIndex <= 8)
                {
                    upperRangeIndex++;
                }
            }
            else
            {
                //Debug.Log("lastCommandsIndex größer 9!!!!");

            }
            //Debug.Log("currentTextMesh");
            //Debug.Log(currentTextMesh);
        }


        public void addDirectlyToUI(ICommand command)
        {
            commandItemsText[currentTextMesh].GetComponent<TextMeshPro>().text = commands[lastCommandIndex - 1].GetType().ToString();
            currentTextMesh++;
        }


        //TODO
        //for keeping track of Historybefore UIHistory menu was instanciated
        public void fillWithExistingCommands()
        {
            fillUI = true;
            for (int i = 0; i <= numberOfTextFields; i++)
             {
                commandItemsText[i].GetComponent<TextMeshPro>().text = commands[i].GetType().ToString();
                if(currentTextMesh <= numberOfTextFields)
                {
                    currentTextMesh++;
                }
                lastCommandIndex++;
             }
        }

        public void scrollUp()
        {
            if (lowerRangeIndex >= 0)
            {
                lowerRangeIndex--;
                upperRangeIndex--;
                currentCommandIndex = lowerRangeIndex;
                for (int i = 0; i <= 8; i++)
                {
                    if (currentCommandIndex <= upperRangeIndex)
                    {
                        commandItemsText[i].GetComponent<TextMeshPro>().text = commands[currentCommandIndex].GetType().ToString();
                    }
                    currentCommandIndex++;
                }
            }
        }


        public void scrollDown()
        {
            if (upperRangeIndex <= lastCommandIndex)
            {
                lowerRangeIndex++;
                upperRangeIndex++;
                currentCommandIndex = lowerRangeIndex;
                for (int i = 0; i <= 8; i++)
                {
                    if (currentCommandIndex <= upperRangeIndex)
                    {
                        commandItemsText[i].GetComponent<TextMeshPro>().text = commands[currentCommandIndex].GetType().ToString();
                    }
                    currentCommandIndex++;
                }
            }
        }

    }
}