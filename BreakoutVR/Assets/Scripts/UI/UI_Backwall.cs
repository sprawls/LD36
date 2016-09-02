using UnityEngine;
using System.Collections;
using GamejamToolset.LevelLoading;

public class UI_Backwall : MonoBehaviour {

    public GameObject ingameUI;
    public GameObject gameNotStartedUI;
    public GameObject gameOverUI;

	// Use this for initialization
	void Awake () {
        ActivateGameNotStarted();
    }

    void Start() {
        //LevelManager.OnLevelStart += OnLevelStart;
	}

    private void OnLevelStart(LevelName Level) {
        //DO STUFF
    }

    private void DeactivateAll() {
        ingameUI.SetActive(false);
        gameNotStartedUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    private void ActivateInGame() {
        DeactivateAll();
        ingameUI.SetActive(true);
    }
    private void ActivateGameNotStarted() {
        DeactivateAll();
        gameNotStartedUI.SetActive(true);
    }
    private void ActivateGameOver() {
        DeactivateAll();
        gameOverUI.SetActive(true);
    }

}
