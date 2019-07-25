using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The status of the issue
/// The order/index of the entries is important and should be synchronized with the corresponding enum on the backend
/// </summary>
public enum IssueStatus
{
    OPEN, IN_PROGRESS, CLOSED
}
