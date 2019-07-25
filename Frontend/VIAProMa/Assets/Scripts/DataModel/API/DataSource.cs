using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// The data source of the issues
/// The order/indices of the entries is important and should be synchronized with the corresponding enum of the backend
/// </summary>
public enum DataSource
{
    [Description("Requirements Bazaar")] REQUIREMENTS_BAZAAR,
    [Description("GitHub")] GITHUB
}
