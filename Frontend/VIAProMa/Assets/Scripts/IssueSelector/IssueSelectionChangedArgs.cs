using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueSelectionChangedArgs : EventArgs
{
    public IssueSelectionChangedArgs(Issue issue, bool selected)
    {
        ChangedIssue = issue;
        Selected = selected;
    }

    public Issue ChangedIssue { get; private set; }

    public bool Selected { get; private set; }
}
