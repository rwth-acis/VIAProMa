using System;
using UnityEngine;

namespace i5.VIAProMa.UI
{
    public class Checkbox : MonoBehaviour
    {
        public event EventHandler OnValueChanged;

        public bool IsChecked { get; private set; }

        public void OnClick()
        {
            IsChecked = !IsChecked;
            OnValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}