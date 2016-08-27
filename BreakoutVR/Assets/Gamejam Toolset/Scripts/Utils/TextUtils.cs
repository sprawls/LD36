using UnityEngine;
using System.Collections;

public static class TextUtils
{
    public static string FillInsideCounter(this string stringRef, int counterLength)
    {
        string result = "";
        int numberOfZero = counterLength - stringRef.Length;

        for (int i = 0; i < counterLength; ++i)
        {
            if (i >= numberOfZero)
            {
                result += stringRef[i - numberOfZero];
            }
            else
            {
                result += '0';
            }
        }
        return result;
    }
}
