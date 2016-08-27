using UnityEngine;
using System.Collections;

public static class TextUtils
{
    public static string FillInsideCounter(this string stringRef, int counterLength)
    {
        string result = "";
        for (int i = counterLength - 1; i >= 0; --i)
        {
            if (i < stringRef.Length)
            {
                result += stringRef[i];
            }
            else
            {
                result += '0';
            }
        }
        return result;
    }
}
