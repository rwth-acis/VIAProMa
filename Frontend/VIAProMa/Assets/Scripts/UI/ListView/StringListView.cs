using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringListView : ListViewController<StringData, StringListViewItem>
{
    private void Awake()
    {
        base.CreateInstances();
    }
}
