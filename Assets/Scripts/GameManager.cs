using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {
	public GUIManager guiManager;
	public Family family;

	public static GameManager Instance = null;

	void Awake() {
		if (Instance == null) {
			Instance = this;

			Debug.Assert(guiManager != null, "Game Manager: GUI Manager is null.");
			Debug.Assert(family != null, "Game Manager: Family is null.");
		}
		else {
			Destroy(this.gameObject);
		}
	}

	void Start() {
		Time.timeScale = 1f;
		GameManager.Instance.GetGUIManager().OnCollectibleUpdate(GameObject.FindObjectsOfType<Collectible>().Length);
	}

	public GUIManager GetGUIManager() {
		return guiManager;
	}

	public Family GetFamily() {
		return family;
	}

	public void OnGameOver(string reason) {
		Time.timeScale = 0;
		GetGUIManager().ShowGameOver(reason);
	}

	public void OnPlayerWins() {
		Time.timeScale = 0;
		GetGUIManager().ShowYouWin();
	}

	public void RestartLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
