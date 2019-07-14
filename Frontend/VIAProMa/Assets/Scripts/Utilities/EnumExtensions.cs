using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Helper class which extends the functionality of enums
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the description of an enum item
    /// The description needs to be added to the enum item as a tag
    /// </summary>
    public static string GetDescription<T>(this T enumerationItem) where T : struct
    {
        // check if the function is called with a enum argument
        Type type = enumerationItem.GetType();
        if (!type.IsEnum)
        {
            Debug.LogError("Tried to get description of non-enum type");
            return "";
        }
        // get the members and attributes; they contain the description
        MemberInfo[] memberInfos = type.GetMember(enumerationItem.ToString());
        if (memberInfos.Length > 0)
        {
            object[] attributes = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return ((DescriptionAttribute)attributes[0]).Description;
            }
        }

        return "";
    }
}
