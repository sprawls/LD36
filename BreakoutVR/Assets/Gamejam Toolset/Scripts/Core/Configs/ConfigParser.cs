using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;

namespace GamejamToolset.Configs
{
	public class Config
	{
		private const int DISTANCE_TO_DESCRIPTION = 60;

		#region Generate
		public static void Generate()
		{
			List<ConfigFieldData> configs = GetConfigFields(typeof(ConfigHelper).GetFields());
			List<string> lines = new List<string>();
			var sortedConfigs = new Dictionary<ConfigCategory, List<ConfigFieldData>>();

			for (int i = 0; i < Enum.GetNames(typeof(ConfigCategory)).Length; ++i)
			{
				sortedConfigs.Add((ConfigCategory)i, new List<ConfigFieldData>());
			}

			foreach (ConfigFieldData config in configs)
			{
				if (ValidateConfig(config.Infos, config.Config.Type))
				{
					sortedConfigs[config.Config.Category].Add(config);
				} 
			}

			for (int i = 0; i < Enum.GetNames(typeof(ConfigCategory)).Length; ++i)
			{
				ConfigCategory category = (ConfigCategory) i;
				lines.Add(string.Format("||| {0} |||", category.ToString().ToUpper()));

				foreach (ConfigFieldData config in sortedConfigs[category])
				{
					object[] customAttributes = config.Infos.GetCustomAttributes(true);
					string description = "";

					foreach (var att in customAttributes)
					{
						DescriptionAttribute descAtt = att as DescriptionAttribute;
						if (descAtt != null)
						{
							description = descAtt.Description;
						}
					}

					string line = string.Format("{0} = {1}", config.Config.Name, config.DefaultValue.ToString());

					if (description != "")
					{
						line += AddSpace(DISTANCE_TO_DESCRIPTION - line.Length);
						line += string.Format("|Description: {0}|", description);
					}

					lines.Add(line);
				}

				lines.Add("");
			}

			File.WriteAllLines(ConfigFilePath, lines.ToArray());
		}

		private static string AddSpace(int count)
		{
			string result = "";
			for (int i = 0; i < Mathf.CeilToInt((float)count/9f); ++i)
			{
				result += '\t';
			}
			return result;
		}
		#endregion

		#region Load
		//-------------------------------------------------------------------------------------------
		public static void Init()
		{
			List<ConfigFieldData> configFields = GetConfigFields(typeof(ConfigHelper).GetFields());
			List<ConfigRawData> configRawData = ConfigParser.Parse();

			if (configFields == null || configRawData == null)
				return;

			for (int i = 0; i < configFields.Count; ++i)
			{
				ConfigFieldData fieldData = configFields[i];
				ConfigRawData rawData = ConfigRawData.FindByName(configRawData, fieldData.Config.Name);

				if (rawData == null)
					continue;

				if (ValidateConfig(fieldData.Infos, fieldData.Config.Type))
				{
					switch (fieldData.Config.Type)
					{
						case SupportedConfigType.Bool:
							RawDataToBool(fieldData.Infos, rawData.RawData);
							break;

						case SupportedConfigType.Int:
							RawDataToInt(fieldData.Infos, rawData.RawData);
							break;

						case SupportedConfigType.Float:
							RawDataToFloat(fieldData.Infos, rawData.RawData);
							break;

						case SupportedConfigType.String:
							RawDataToString(fieldData.Infos, rawData.RawData);
							break;
					}
				}
			}
		}

		//-------------------------------------------------------------------------------------------
		private static bool ValidateConfig(FieldInfo info, SupportedConfigType type)
		{
			if (!info.IsStatic)
			{
				Debug.LogError("Config Field {0} must be static");
				return false;
			}

			Type fieldType = info.FieldType;

			switch (type)
			{
				case SupportedConfigType.Bool:
					if (fieldType != typeof(bool)) 
					{
						Debug.LogError("Config Field {0} is tagged has bool but is not a boolean, fix it");
						return false;
					}
					break;

				case SupportedConfigType.Int:
					if (fieldType != typeof(int))
					{
						Debug.LogError("Config Field {0} is tagged has int but is not a boolean, fix it");
						return false;
					}
					break;

				case SupportedConfigType.Float:
					if (fieldType != typeof(float))
					{
						Debug.LogError("Config Field {0} is tagged has float but is not a boolean, fix it");
						return false;
					}
					break;

				case SupportedConfigType.String:
					if (fieldType != typeof(string))
					{
						Debug.LogError("Config Field {0} is tagged has string but is not a boolean, fix it");
						return false;
					}
					break;
			}

			return true;
		}

		//==================================================================================================================
		#region Raw Data to Type
		//-------------------------------------------------------------------------------------------
		private static void RawDataToBool(FieldInfo info, string RawData)
		{
			string rawData = RawData.ToLower();

			switch (rawData)
			{
				case "true":
					info.SetValue(null, true);
					break;

				case "false":
					info.SetValue(null, false);
					break;

				default:
					Debug.LogErrorFormat("The field {0} is tagged has a boolean but the config file data isn't correct, use \"true\" or \"false\"", info.Name);
					break;
			}
		}

		//-------------------------------------------------------------------------------------------
		private static void RawDataToFloat(FieldInfo info, string RawData)
		{
			for (int i = 0; i < RawData.Length; ++i)
			{
				if (RawData[i] != '.' && !char.IsDigit(RawData[i])) 
				{
					Debug.LogErrorFormat("The field {0} is tagged has a float but the config file data isn't correct", info.Name);
					return;
				}
			}

			info.SetValue(null, float.Parse(RawData));
		}

		//-------------------------------------------------------------------------------------------
		private static void RawDataToInt(FieldInfo info, string RawData)
		{
			for (int i = 0; i < RawData.Length; ++i)
			{
				if (!char.IsDigit(RawData[i]))
				{
					Debug.LogErrorFormat("The field {0} is tagged has a int but the config file data isn't correct", info.Name);
					return;
				}
			}

			info.SetValue(null, int.Parse(RawData));
		}

		//-------------------------------------------------------------------------------------------
		private static void RawDataToString(FieldInfo info, string RawData)
		{
			info.SetValue(null, RawData);
		}
		#endregion
		#endregion

		//-------------------------------------------------------------------------------------------
		private static List<ConfigFieldData> GetConfigFields(FieldInfo[] infos)
		{
			List<ConfigFieldData> configs = new List<ConfigFieldData>();

			foreach (FieldInfo info in infos)
			{
				object[] attributes = info.GetCustomAttributes(true);
				DefaultValueAttribute defaultAtt = null;
				ConfigAttribute configAtt = null;
				foreach (object attribute in attributes)
				{
					if (configAtt == null)
					{
						configAtt = attribute as ConfigAttribute;
					}
					if (defaultAtt == null)
					{
						defaultAtt = attribute as DefaultValueAttribute;
					}
				}

				if (configAtt != null)
				{
					if (defaultAtt != null)
					{
						configs.Add(new ConfigFieldData(info, configAtt, defaultAtt.Value));
					}
					else
					{
						configs.Add(new ConfigFieldData(info, configAtt, info.GetValue(null)));
					}
				}
			}

			return configs;
		}

		//-------------------------------------------------------------------------------------------
		public static string ConfigFilePath 
		{
			get
			{
				return Application.dataPath + "/config.ini";
			}
		}
	}

	//==================================================================================================================
	public class ConfigRawData
	{
		public string Name { get; private set; }
		public string RawData { get; private set; }

		public ConfigRawData(string name, string rawData)
		{
			Name = name;
			RawData = rawData;
		}

		public static ConfigRawData FindByName(List<ConfigRawData> data, string name)
		{
			for (int i = 0; i < data.Count; ++i)
			{
				if (data[i].Name == name)
				{
					return data[i];
				}
			}

			return null;
		}
	}

	//==================================================================================================================
	public class ConfigFieldData
	{

		public FieldInfo Infos { get; private set; }
		public ConfigAttribute Config { get; private set; }
		public object DefaultValue { get; private set; }

		public ConfigFieldData(FieldInfo info, ConfigAttribute config, object defaultValue)
		{
			Infos = info;
			Config = config;
			DefaultValue = defaultValue;
		}
	}

	//==================================================================================================================
	public class ConfigParser
	{
		public static List<ConfigRawData> Parse()
		{
			List<ConfigRawData> data = new List<ConfigRawData>();
			
			//In submission only use default value
			#if !SUBMISSION_BUILD
			if (!File.Exists(Config.ConfigFilePath))
			{
				CustomLogger.BootLog("Could not find Config.ini file, reverting to fallback values");
				return null;
			}

			string[] lines = File.ReadAllLines(Config.ConfigFilePath);
			for (int i = 0; i < lines.Length; ++i)
			{
				ConfigRawData config = LineParser(lines[i]);
				if (config != null)
				{
					data.Add(config);
				}
			}
			#endif

			return data;
		}

		enum ParseState
		{
			ParsingName,
			ParsingData,
			ParsingIdle
		}

		private static ConfigRawData LineParser(string line)
		{
			string Name = "";
			string RawData = "";

			ParseState state = ParseState.ParsingIdle;
			bool foundEqualSymbol = false;
			bool foundDot = false;

			for (int i = 0; i < line.Length; ++i)
			{
				char current = line[i];
				bool skipCheck = false;

				switch (current)
				{
					case '=':
						foundEqualSymbol = true;
						break;

					case '|':
					case ';':
						return DataToReturn(Name, RawData);

					case '.':
						if (state == ParseState.ParsingData && !foundDot)
						{
							foundDot = true;
							skipCheck = true;
						}
						break;

					case '_':
						skipCheck = true;
						break;
				}

				if (!skipCheck && !char.IsLetterOrDigit(current))
				{
					state = ParseState.ParsingIdle;
					continue;
				}

				if (state == ParseState.ParsingIdle)
				{
					if (Name == "")
						state = ParseState.ParsingName;

					if (RawData == "" && foundEqualSymbol)
						state = ParseState.ParsingData;
				}

				switch (state)
				{
					case ParseState.ParsingName:
					{
						Name += current;
						break;
					}

					case ParseState.ParsingData:
					{
						RawData += current;
						break;
					}
				}
			}

			return DataToReturn(Name, RawData);
		}

		private static ConfigRawData DataToReturn(string Name, string RawData)
		{
			if (Name != "" && RawData != "")
				return new ConfigRawData(Name, RawData);

			return null;
		}
	}
}