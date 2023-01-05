using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.UI.InputFields;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.UI.Chat
{
    public class UndoRedoMenu : MonoBehaviour, IWindow
    {
        public bool WindowEnabled { get; set; }

        public bool WindowOpen { get; private set; }

        public event EventHandler WindowClosed;

        public void Close()
        {
            WindowOpen = false;
            WindowClosed?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            WindowOpen = true;
        }

        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.localPosition = position;
            transform.localEulerAngles = eulerAngles;
        }
    }
}