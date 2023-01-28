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
        }

        void Awake()
        {
            UndoRedoManagerGameObject = GameObject.Find("UndoRedo Manager");
            UndoRedoManager = UndoRedoManagerGameObject.GetComponent<UndoRedoManager>();
            numberOfTextFields = commandItemsText.Length;
        }

       
        //-------------------------- for UIHistory display --------------------------

        //displays the commands in given range
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

        //up button
        void scrollDown()
        {
            //upperRangeIndex < commands.Count
            if (upperRangeIndex < commands.Count)
            {
                lowerRangeIndex++;
                upperRangeIndex++;
            }
        }

        /down button
        void scrollUp()
        {
            lowerRangeIndex--;
            upperRangeIndex--;
        }


        //called when cube button is pressed in UI meaning that the steps until that point will be undone
        void Select(int selectedCubeIndex)
        {
            //calculates position in command list
            int stepsToUndo = commands.Count - (lowerRangeIndex + selectedCubeIndex);
            for (int i = 0; i<= stepsToUndo; i++)
            {
                UndoRedoManager.Undo();
            }
            //repositions currentPosition in CommandProcessor
            UndoRedoManager.setCurrentPosition(stepsToUndo);
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