using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Minimap 
{
    public class MinimapHandle : MonoBehaviour, IMixedRealityPointerHandler
    {
        [SerializeField] private MinimapController minimap;
        private IMixedRealityPointer activePointer;
        public bool handleOnPositiveCap;

        private void Awake()
        {
            if (minimap is null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(minimap));
            }
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (activePointer == null && !eventData.used)
            {
                activePointer = eventData.Pointer;
                minimap.StartResizing(activePointer.Position, handleOnPositiveCap);
                eventData.Use();
            }

        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                minimap.SetHandles(activePointer.Position, handleOnPositiveCap);
                eventData.Use();
            }     

        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                activePointer = null;
                eventData.Use();
            }
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
