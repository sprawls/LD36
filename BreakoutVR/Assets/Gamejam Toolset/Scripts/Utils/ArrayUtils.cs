using UnityEngine;
using System.Collections.Generic;

public static class ArrayUtils
{
	public static bool HasOnlyLetterAndDigit(List<string> stringArray)
	{
		for (int i = 0; i < stringArray.Count; ++i)
		{
			if (!stringArray[i].HasOnlyLetterAndDigit())
			{
				return false;
			}
		}

		return true;
	}

	public static bool HasOnlyLetterAndDigit(string[] stringArray)
	{
		for (int i = 0; i < stringArray.Length; ++i)
		{
			if (!stringArray[i].HasOnlyLetterAndDigit())
			{
				return false;
			}
		}

		return true;
	}
}
