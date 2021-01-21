using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.UI.KeyboardInput
{
    [RequireComponent(typeof(Interactable))]
    public class Key : MonoBehaviour
    {
        protected string value;

        protected Keyboard keyboard;

        protected virtual void Awake()
        {
            Interactable interactable = GetComponent<Interactable>();
            interactable.OnClick.AddListener(KeyPressed);

            keyboard = transform.parent.parent.GetComponent<Keyboard>();
            if (keyboard == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Keyboard), transform.parent.gameObject);
            }
        }

        protected virtual void KeyPressed()
        {
        }
    }
}