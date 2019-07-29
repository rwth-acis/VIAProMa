using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShelfConfiguration
{
    bool IsValidConfiguration { get; }

    DataSource SelectedSource { get; }
}
