using UnityEngine;
using System.Collections;

public static class StringExtension
{
	public static bool HasOnlySpace(this string s)
	{
		for (int i = 0; i < s.Length; ++i)
		{
			if (s[i] != ' ')
				return false;
		}

		return true;
	}

	public static bool HasOnlyLetterAndDigit(this string s)
	{
		for (int i = 0; i < s.Length; ++i)
		{
			if (!char.IsDigit(s[i]) && !char.IsLetter(s[i]))
				return false;
		}

		return true;
	}
}
