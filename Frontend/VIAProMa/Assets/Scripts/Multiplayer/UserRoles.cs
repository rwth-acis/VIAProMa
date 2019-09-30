using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum UserRoles
{
    [Description("Product Owner")]
    PRODUCT_OWNER = 0,
    [Description("Scrum Master")]
    SCRUM_MASTER = 1,
    [Description("Developer")]
    DEVELOPER = 2,
    [Description("Stakeholder")]
    STAKEHOLDER = 3
}
