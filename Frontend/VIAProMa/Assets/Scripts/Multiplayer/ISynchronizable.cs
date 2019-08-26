using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISynchronizable
{
    bool SynchronizationInProgress { get; set; }
}
