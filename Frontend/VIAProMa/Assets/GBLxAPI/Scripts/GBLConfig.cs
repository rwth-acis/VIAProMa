namespace DIG.GBLXAPI
{
    public class GBLConfig
    {
        public const string StandardsDefaultPath = "data/GBLxAPI_Vocab_Default";
        public const string StandardsUserPath = "data/GBLxAPI_Vocab_User";

        public const string LrsURL = "https://lrs.gblxapi.org/data/xAPI";

        // Fill in these fields for GBLxAPI setup.
        public string lrsUser = "[Enter user here]";
        public string lrsPassword = "[Enter password here]";

        public string companyURI = "https://company.com/";
        public string gameURI = "https://company.com/example-game";
        public string gameName = "Example Game";

        public GBLConfig()
        {

        }

        public GBLConfig(string lrsUser, string lrsPassword)
        {
            this.lrsUser = lrsUser;
            this.lrsPassword = lrsPassword;
        }
    }
}
