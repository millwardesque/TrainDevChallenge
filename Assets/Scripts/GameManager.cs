using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameState {
	Starting,
	Running,
	Paused,
	GameOver,
	PlayerWins
};

public class GameManager : MonoBehaviour {
	public GUIManager guiManager;
	public Family family;

	GameState m_state;
	public GameState State {
		get { return m_state; }
		set {
			m_state = value;
			Debug.Log(string.Format("Setting state to {0}", m_state));

			if (m_state == GameState.Starting) {
				ArmySpawner[] armySpawners = FindObjectsOfType<ArmySpawner>();
				for (int i = 0; i < armySpawners.Length; ++i) {
					for (int j = 0; j < 30; ++j) {
						armySpawners[i].SpawnSoldier();
					}
				}

				CollectibleSpawner[] collectibleSpawners = FindObjectsOfType<CollectibleSpawner>();
				for (int i = 0; i < collectibleSpawners.Length; ++i) {
					collectibleSpawners[i].SpawnCollectibles();
				}

				GameManager.Instance.GetGUIManager().OnCollectibleUpdate(GameObject.FindObjectsOfType<Collectible>().Length);
			}
			else if (m_state == GameState.Running) {
				Time.timeScale = 1f;
			}
			else if (m_state == GameState.Paused) {
				Time.timeScale = 0f;
			}
			else if (m_state == GameState.GameOver) {
				Time.timeScale = 0f;
			}
			else if (m_state == GameState.PlayerWins) {
				Time.timeScale = 0f;
			}
		}
	}

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
		State = GameState.Starting;
	}

	void Update() {
		if (State == GameState.Starting) {
			State = GameState.Running;
		}
	}

	public GUIManager GetGUIManager() {
		return guiManager;
	}

	public Family GetFamily() {
		return family;
	}

	public void OnGameOver(string reason) {
		Debug.Log(reason);
		GetGUIManager().ShowGameOver(reason);
		State = GameState.GameOver;
	}

	public void OnPlayerWins() {
		GetGUIManager().ShowYouWin();
		State = GameState.PlayerWins;
	}

	public void RestartLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
