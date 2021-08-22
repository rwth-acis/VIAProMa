using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGuide : MonoBehaviour
{

    private bool testStarted = false;
    public GameObject dialogPanel;
    private int testPhase = 0;
    public GameObject nextButton;
    
    public bool TestStarted { 
        get => testStarted;
        set
        {
            testStarted = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        testPhase = 0;
        testStarted = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartTest() {
        testStarted = true;
        testPhase = 0;
        Instantiate(nextButton);
        ShowGuideMessage(); 
    }

    public void NextTask() {
        testPhase++;
        ShowGuideMessage();
        Debug.Log(testPhase);
    }

    private void ShowDialog(string title, string content) {
        Component guidePanel = Dialog.Open(dialogPanel, DialogButtonType.OK, title, content, true);
        guidePanel.gameObject.GetComponent<Follow>().MinDistance = 0.4f;
        guidePanel.gameObject.GetComponent<Follow>().MaxDistance = 0.4f;
        guidePanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.4f;
        guidePanel.gameObject.transform.forward = CameraCache.Main.transform.forward;
    }

    private void ShowGuideMessage() {
        if (testStarted) {
            switch (testPhase) {
                case 0:
                    ShowDialog("Welcome", "Welcome to the evaluation process. In this evaluation, you have 12 simple tasks to be done. " +
                        "In each task, you need to freely interact with it to test its placement. " +
                        "There is no specific goals in a task." +
                        " You can click the 'Next' button on your left to move to the next task.");
                    break;
                case 1:
                    ShowDialog("Task 1/12", "Please freely move in a capacious space with the main menu and interact with it.");
                    break;
                case 2:
                    ShowDialog("Task 2/12", "Please move close to a wall at a distance about 60cm to let the main menu stick onto it.");
                    break;
                case 3:
                    ShowDialog("Task 3/12", "Please move even closer to the wall and let the menu be switch to a compact variant.");
                    break;
                case 4:
                    ShowDialog("Task 4/12", "You can now adjust the menu using the app bar below it to make it best suitable for you to use. You may not change it anymore when you find the best suitable transform " +
                        "because the last one will be saved.");
                    break;
                case 5:
                    ShowDialog("Task 5/12", "Please create an object using the 'create object' button on the main menu. Then you can close the main menu using the app bar below it.");
                    break;
                case 6:
                    ShowDialog("Task 6/12", "Please move far away from the created object and let the menu be placed in front of yourself.");
                    break;
                case 7:
                    ShowDialog("Task 7/12", "Please move closer to the object and let the menu be placed beside it.");
                    break;
                case 8:
                    ShowDialog("Task 8/12", "Please move even closer to the object and let the system switch to the compact variant.");
                    break;
                case 9:
                    ShowDialog("Task 9/12", "Please move closer to a wall with the object and try to let the menu collide the wall. For example, try to let the menu be at the right side of the object " +
                        "and move the object then closer and closer to the wall. The placement strategy of the object menu will change.");
                    break;
                case 10:
                    ShowDialog("Task 10/12", "Please try to move to some places so that the menu will be occluded by other things (e.g. by a wall) to test the occlusion detection function.");
                    break;
                case 11:
                    ShowDialog("Task 11/12", "Please adjust the menu's position, orientation and scale using the app bar below it. " +
                        "You may not change it anymore after reaching the best suitable transform because the last one will be saved. You can freely interact with the menus (object and main menus)" +
                        " and move to any places to test the placement algorithm.");
                    break;
                case 12:
                    ShowDialog("Task 12/12", "Please switch to the MANUAL MODE using the control panel displayed bottom right in your sight. " +
                        "You may also try the collision and occlusion detection functionalities in MANUAL MODE.");
                    break;
                default:
                    ShowDialog("Evaluation Finished!", "Thanks for your time and the participation of the evaluation! Please finish the evalaution questionnaire on LimeSurvey. " +
                        "Remember to quit the application using the button on the control panel on the left side in your sight.");
                    break;
            }


        }
    }

}
