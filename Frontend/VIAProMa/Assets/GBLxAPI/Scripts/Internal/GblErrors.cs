using System;

namespace DIG.GBLXAPI
{
    public class VocabMissingException : Exception
    {
        public VocabMissingException(string termType, string term)
            : base("The " + termType + " \"" + term + "\"" + " does not exist in the GBLxAPI vocabulary. Either fix the typo or add the term to the vocabulary by editing Vocabulary/GBLxAPI_Vocab_User.xlsx, running GBLxAPI_Json_Parser.py, and moving the generated json files to Assets/Resources/Data. See the GBLxAPI documentation for help.")
        {

        }
    }
}
