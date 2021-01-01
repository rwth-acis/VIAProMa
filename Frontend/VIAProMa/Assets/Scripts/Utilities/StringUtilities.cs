using System.Text.RegularExpressions;

namespace i5.VIAProMa.Utilities
{
    public static class StringUtilities
    {
        public static string RemoveSpecialCharacters(string text)
        {
            return Regex.Replace(text, "[^a-zA-Z0-9 ]", "");
        }

        /// <summary>
        /// Determines if the given word contains at least one of the keywords
        /// </summary>
        /// <param name="word">The word to examine</param>
        /// <param name="keywords">The keywords which the should can contain</param>
        /// <returns>True if word contains at least one of the keywords</returns>
        public static bool ContainsAny(string word, string[] keywords)
        {
            foreach (string keyword in keywords)
            {
                if (word.Contains(keyword))
                {
                    return true;
                }
            }
            return false;
        }
    }
}