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
        //shows commands from commands list in this range in UI 
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
            showRange(lowerRangeIndex, upperRangeIndex);
        }

        void Awake()
        {
            UndoRedoManagerGameObject = GameObject.Find("UndoRedo Manager");
            UndoRedoManager = UndoRedoManagerGameObject.GetComponent<UndoRedoManager>();
            numberOfTextFields = commandItemsText.Length;
        }

       
        //-------------------------- for UIHistory display --------------------------

        //zeigt die von vonIndex bis bisIndex aus der commandsliste an
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

        void scrollDown()
        {
            //upperRangeIndex < commands.Count
            if (upperRangeIndex < commands.Count)
            {
                lowerRangeIndex++;
                upperRangeIndex++;
            }
        }

        void scrollUp()
        {
            lowerRangeIndex--;
            upperRangeIndex--;
        }



        void Select(int selectedCubeIndex)
        {
            int stepsToUndo = commands.Count - (lowerRangeIndex + selectedCubeIndex);
            for (int i = 0; i<= stepsToUndo; i++)
            {
                UndoRedoManager.Undo();
            }
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