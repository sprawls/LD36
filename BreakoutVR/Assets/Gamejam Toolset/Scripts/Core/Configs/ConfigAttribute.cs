using UnityEngine;

namespace GamejamToolset.Configs
{
	public enum SupportedConfigType
	{
		Float,
		Bool,
		Int,
		String,
	}

	public class ConfigAttribute : PropertyAttribute
	{
		public string Name { get; private set; }
		public ConfigCategory Category { get; private set; }
		public SupportedConfigType Type { get; private set; }

		public ConfigAttribute(string name, ConfigCategory category, SupportedConfigType type)
		{
			Name = name;
			Category = category;
			Type = type;

			//Validate Data
			if (name.Length == 0)
			{
				Debug.LogErrorFormat("Config item \"{0}\" cannot have a name of length 0", name);
			}

			for (int i = 0; i < name.Length; ++i)
			{
				if (name[i] == ' ')
				{
					Debug.LogErrorFormat("Config item \"{0}\" cannot contain spaces", name);
					break;
				}

				if (!char.IsLetterOrDigit(name[i]))
				{
					Debug.LogErrorFormat("Config item \"{0}\" must be only composed of letters and digits", name);
					break;
				}
			}
		}
	}

	public class DefaultValueAttribute : PropertyAttribute
	{
		public object Value { get; private set; }

		public DefaultValueAttribute(object value)
		{
			Value = value;
		}
	}

	public class DescriptionAttribute : PropertyAttribute
	{
		public string Description { get; private set; }

		public DescriptionAttribute(string description)
		{
			Description = description;
		}
	}
}