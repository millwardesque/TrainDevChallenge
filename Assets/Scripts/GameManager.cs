using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

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
		private set {
			m_state = value;

			if (m_state == GameState.Running) {
				Time.timeScale = 1f;
			}
			else if (m_state == GameState.Paused) {
				Time.timeScale = 0f;
			}
			else if (m_state == GameState.GameOver) {
				Time.timeScale = 0f;
				Messenger.SendMessage(this, "Game Over", deathReason);
			}
			else if (m_state == GameState.PlayerWins) {
				Time.timeScale = 0f;
				Messenger.SendMessage(this, "Player Wins");
				LoadLevel(m_level.nextLevel);
			}
		}
	}

	Level m_level;
	string deathReason = "";

	MessageManager m_messenger = new MessageManager();
	public MessageManager Messenger {
		get { return m_messenger; }
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
		m_level = GameObject.FindObjectOfType<Level>();
		State = GameState.Starting;
	}

	void Update() {
		if (State == GameState.Starting) {
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
		deathReason = reason;
		State = GameState.GameOver;
	}

	public void OnPlayerWins() {
		State = GameState.PlayerWins;
	}

	public void RestartLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void LoadLevel(string levelName) {
		SceneManager.LoadScene(levelName);
	}
}
