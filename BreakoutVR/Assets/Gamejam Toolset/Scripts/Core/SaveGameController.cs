using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using GamejamToolset.Saving;

[Serializable]
public abstract class SaveData<T> where T : SaveData<T>
{
	public abstract string FileName { get; }

	public new Type GetType() { return typeof(T); }

	//------------------------------------------------------------------
	protected SaveData()
	{
		SaveGameController.OnLoad += Callback_OnLoad;
		SaveGameController.OnSave += Callback_OnSave;
	}

	//------------------------------------------------------------------
	~SaveData()
	{
		SaveGameController.OnLoad -= Callback_OnLoad;
		SaveGameController.OnSave -= Callback_OnSave;
	}

	//------------------------------------------------------------------
	private void Callback_OnSave()
	{
		SaveGameController.Instance.Save<T>((T)this);
	}

	//------------------------------------------------------------------
	private void Callback_OnLoad()
	{
		SaveGameController.Instance.Load<T>((T)this);
	}
}

namespace GamejamToolset.Saving
{
	public class SaveGameController : Singleton<SaveGameController>
	{
		private const string FILE_EXTENSION = "gg";

		//===========================================================================================================

		public static event Action OnSave;
		public static event Action OnLoad;

		//===========================================================================================================

		private bool m_saveRequested = false;
		private bool m_loadRequested = false;
		private bool m_saveInProgress = false;
		private bool m_loadInProgress = false;

		//===========================================================================================================
		//------------------------------------------------------------
		public void RequestSave()
		{
			m_saveRequested = true;
		}

		//------------------------------------------------------------
		public void RequestLoad()
		{
			m_loadRequested = true;
		}

		//===========================================================================================================
		//------------------------------------------------------------
		private void Update()
		{
			if (m_loadInProgress || m_saveInProgress)
				return;

			if (m_loadRequested)
			{
				m_loadRequested = false;
				m_loadInProgress = true;

				StartCoroutine(Coroutine_Load());
			}
			else if (m_saveRequested)
			{
				m_saveRequested = false;
				m_saveInProgress = true;

				StartCoroutine(Coroutine_Save());
			}
		}

		//------------------------------------------------------------
		private IEnumerator Coroutine_Save()
		{
			yield return new WaitForEndOfFrame();

			if (OnSave != null)
				OnSave();

			m_saveInProgress = false;
		}

		//------------------------------------------------------------
		private IEnumerator Coroutine_Load()
		{
			yield return new WaitForEndOfFrame();

			if (OnLoad != null)
				OnLoad();
			
			m_loadInProgress = false;
		}

		//------------------------------------------------------------
		public bool Save<T>(T saveData) where T : SaveData<T>
		{
			if (m_loadInProgress)
				return false;

			try
			{
				BinaryFormatter formatter = new BinaryFormatter();
				FileStream file = File.Open(
					string.Format
					(
						"{0}/{1}.{2}",
						Application.persistentDataPath,
						saveData.FileName,
						FILE_EXTENSION
					),
					FileMode.OpenOrCreate
				);

				formatter.Serialize(file, saveData);
				file.Close();
			}
			catch (SerializationException e)
			{
				Debug.LogErrorFormat
				(
					"{0} {1} {2} {3} {4} {5} {6}",
					"The save data class of type:",
					typeof(T).Name,
					"is not serializable.",
					"Use the attribute [System.Serializable] on the class and every class contained inside it.",
					"MonoBehaviour cannot be saved this way, for Monobehaviour use unity's serialization.",
					"Error Message: ",
					e.Message
				);
			}

			return true;
		}

		//------------------------------------------------------------
		public bool Load<T>(T saveData) where T : SaveData<T>
		{
			if (m_saveInProgress)
				return false;

			string dataPath = string.Format
			(
				"{0}/{1}.{2}",
				Application.persistentDataPath,
				saveData.FileName,
				FILE_EXTENSION
			);

			if (!File.Exists(dataPath))
			{
				return false;
			}

			BinaryFormatter formatter = new BinaryFormatter();
			FileStream file = File.Open(dataPath, FileMode.Open);

			saveData = (T)formatter.Deserialize(file);
			file.Close();

			return true;
		}
	}
}
