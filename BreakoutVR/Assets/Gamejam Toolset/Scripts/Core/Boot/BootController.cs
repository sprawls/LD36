using UnityEngine;
using UnityEngine.SceneManagement;
using GamejamToolset.Configs;
using GamejamToolset.LevelLoading;

public class BootController : MonoBehaviour 
{
	public const string PERSISTENT_SCENE_NAME = "Static Scene";
	public const string PERSISTENT_SCENE_PATH = "Assets/Leblond Gamejam Toolset/{0}.unity";
	public const string BOOT_SCENE_NAME = "Boot Scene";

    public static bool WasBooted = false;

	[Header("Persistent"), SerializeField]
	private PersistentObjectsManager persistentSpawnerObject;

	[Header("Component"), SerializeField]
	private BootLogoUI uiController;

	void Awake()
	{
		if (uiController == null)
		{
			CustomLogger.BootLogError("The boot UI controller component was not set properly");
			return;
		}
	}

	void Start()
	{
		CustomLogger.BootLog("Loading Configs");
#if !SUBMISSION_BUILD
        Config.Init();
#endif

        CustomLogger.BootLog("Starting Bootstrap");
		if (SceneManager.sceneCount > 0)
		{
			for (int i = SceneManager.sceneCount - 1; i >= 0; --i)
			{
				if (SceneManager.GetSceneAt(i).name != "Boot Scene")
				{
					SceneManager.UnloadScene(SceneManager.GetSceneAt(i).name);
				}
			}
		}

		CustomLogger.BootLog("Persistent Objects Initialized");
		Instantiate(persistentSpawnerObject.gameObject);

		//Access Init
		Access_LevelManager.Init();
		Access_SaveGameController.Init();
		//Access_AudioController.Init();

		if (ConfigHelper.BootSkipIntro)
		{
			CustomLogger.BootLog("Intro Screen Skiped");
			Callback_OnLogoEnd();
		}
		else
		{
			CustomLogger.BootLog("Intro Screen Starting");
			uiController.StartLogoUI(Callback_OnLogoEnd);
		}
	}

	private void Callback_OnLogoEnd()
	{
		CustomLogger.BootLog("Intro Screen Ended");

		UIController.Instance.Show(true);
		if (persistentSpawnerObject == null)
		{
			CustomLogger.BootLogError("The persistent Object spawner prefab reference in BootController is null");
			return;
		}

	    WasBooted = true;

#if !SUBMISSION_BUILD && false
		Access_LevelManager.RequestLoadLevel(ConfigHelper.StartingLevel);
#else
		LevelManager.Instance.RequestLoadLevel(LevelName.Main);
#endif
	}
}
