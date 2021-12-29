using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;

namespace GuidedTour
{
    [UnityEngine.AddComponentMenu("Scripts/MRTK/SDK/InteractableReceiver")]
    public class KeyboardTaskEventHandler : ReceiverBaseMonoBehavior
    {

        public UnityEvent m_MyEvent;

        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = default) 
        {
            NotifyTask();
        }

        public void NotifyTask() 
        {
            Debug.Log("The Keyboard done has been pressed by Luk");
            m_MyEvent.Invoke();
        }

    }
}
