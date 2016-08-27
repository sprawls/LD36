using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SerializableDictionary<TI, TJ>
{
	public List<TI> Keys { get { return m_keys; } }
	public List<TJ> Values { get { return m_values; } }

	[SerializeField, HideInInspector]
	private List<TI> m_keys;
	[SerializeField, HideInInspector]
	private List<TJ> m_values;

	public SerializableDictionary()
	{
		m_keys = new List<TI>();
		m_values = new List<TJ>();
	}

	public SerializableDictionary(Dictionary<TI, TJ> dictionary)
	{
		m_keys = new List<TI>();
		foreach (TI key in dictionary.Keys)
		{
			m_keys.Add(key);
		}

		m_values = new List<TJ>();
		foreach (TJ value in dictionary.Values)
		{
			m_values.Add(value);
		}
	}

	public Dictionary<TI, TJ> GetDictionary()
	{
		Dictionary<TI, TJ> dictio = new Dictionary<TI, TJ>();

		for (int i = 0; i < Keys.Count; ++i)
		{
			TI key = Keys[i];

			if (!dictio.ContainsKey(key))
			{
				dictio.Add(key, Values[i]);
			}
		}

		return dictio;
	}
}
