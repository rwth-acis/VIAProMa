using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringUtilities
{
    public static string RemoveSpecialCharacters(string text)
    {
        return Regex.Replace(text, "[^a-zA-Z0-9 ]", "");
    }
}
