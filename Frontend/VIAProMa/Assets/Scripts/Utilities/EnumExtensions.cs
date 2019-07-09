using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public static class EnumExtensions
{
    public static string GetDescription<T>(this T enumerationItem) where T : struct
    {
        Type type = enumerationItem.GetType();
        if (!type.IsEnum)
        {
            Debug.LogError("Tried to get description of non-enum type");
            return "";
        }
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
