using System.ComponentModel;

namespace i5.VIAProMa.Multiplayer
{
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
}
