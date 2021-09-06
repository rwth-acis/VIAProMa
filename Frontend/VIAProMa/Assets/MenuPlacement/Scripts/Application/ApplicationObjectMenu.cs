using i5.VIAProMa.Utilities;
using MenuPlacement;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class ApplicationObjectMenu : MenuBase
{

    
    [SerializeField] private GameObject slider;
    

    private ObjectManipulator manipulator;
    private float initialZ;
    private GameObject targetObject;

    // Start is called before the first frame update
    void Awake()
    {
        if (slider == null) {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(slider));
        }
    }

    private void OnDisable() {

    }

    public void MoveWithSlider() {
        targetObject.transform.position = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, initialZ + slider.GetComponent<PinchSlider>().SliderValue * 3);
    }

    public void AllowMoveOperation() {
        if (targetObject.GetComponent<MoveAxisConstraint>().enabled == true) {
            initialZ = targetObject.transform.position.z;
            slider.SetActive(true);
            slider.GetComponent<PinchSlider>().SliderValue = 0;
            targetObject.GetComponent<MinMaxScaleConstraint>().enabled = true;
            targetObject.GetComponent<RotationAxisConstraint>().enabled = true;
            targetObject.GetComponent<MoveAxisConstraint>().enabled = false;
            manipulator.ManipulationType = ManipulationHandFlags.OneHanded | ManipulationHandFlags.TwoHanded;
        }
    }



    public void AllowRotateOperation() {
        if (targetObject.GetComponent<RotationAxisConstraint>().enabled == true) {
            initialZ = targetObject.transform.position.z;
            slider.SetActive(false);
            targetObject.GetComponent<MinMaxScaleConstraint>().enabled = true;
            targetObject.GetComponent<MoveAxisConstraint>().enabled = true;
            targetObject.GetComponent<RotationAxisConstraint>().enabled = false;
            manipulator.ManipulationType = ManipulationHandFlags.TwoHanded | ManipulationHandFlags.OneHanded;
            manipulator.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;
            manipulator.OneHandRotationModeNear = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;
            manipulator.TwoHandedManipulationType = TransformFlags.Rotate;
        }
    }

    public void AllowScaleOperation() {        
        if (targetObject.GetComponent<MinMaxScaleConstraint>().enabled == true) {
            initialZ = targetObject.transform.position.z;
            slider.SetActive(false);
            targetObject.GetComponent<RotationAxisConstraint>().enabled = true;
            targetObject.GetComponent<MoveAxisConstraint>().enabled = true;
            targetObject.GetComponent<MinMaxScaleConstraint>().enabled = false;
            manipulator.ManipulationType = ManipulationHandFlags.TwoHanded;
            manipulator.TwoHandedManipulationType = TransformFlags.Scale;
        }

    }


    public override void Initialize() {
        targetObject = GetComponent<MenuHandler>().TargetObject;
        manipulator = targetObject.GetComponent<ObjectManipulator>();
    }

    public override void OnClose() {
        targetObject = GetComponent<MenuHandler>().TargetObject;
        manipulator = targetObject.GetComponent<ObjectManipulator>();
        targetObject.GetComponent<PointerHandler>().enabled = true;
        initialZ = targetObject.transform.position.z;
        slider.GetComponent<PinchSlider>().SliderValue = 0;
        slider.SetActive(false);
        targetObject.GetComponent<MinMaxScaleConstraint>().enabled = true;
        targetObject.GetComponent<MoveAxisConstraint>().enabled = true;
        targetObject.GetComponent<RotationAxisConstraint>().enabled = true;
        manipulator.ManipulationType = 0;
        manipulator.TwoHandedManipulationType = 0;
    }
}
