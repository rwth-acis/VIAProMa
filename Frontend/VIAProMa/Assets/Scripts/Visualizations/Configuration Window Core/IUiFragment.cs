using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUiFragment
{
    bool UIEnabled { get; set; }

    void Setup(Visualization visualization);
}
