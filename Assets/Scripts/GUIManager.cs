using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public Text hungerLabel;
	public Text illnessLabel;
	public Text happinessLabel;
	public Text remainingCollectiblesLabel;
	public Text itemsInSackLabel;
	public GameObject youWinPanel;
	public GameObject gameOverPanel;
	public Text gameOverReason;

	void Awake() {
		Debug.Assert(hungerLabel != null, "GUIManager: Hunger label is null");
		Debug.Assert(illnessLabel != null, "GUIManager: Illness label is null");
		Debug.Assert(happinessLabel != null, "GUIManager: Happiness label is null");
		Debug.Assert(remainingCollectiblesLabel != null, "GUIManager: Remaining Collectibles label is null");
		Debug.Assert(itemsInSackLabel != null, "GUIManager: Items-in-sack label is null");
		Debug.Assert(youWinPanel != null, "GUIManager: You-Win panel is null");
		Debug.Assert(gameOverPanel != null, "GUIManager: Game-Over panel is null");
		Debug.Assert(gameOverReason != null, "GUIManager: Game-Over reason is null");
	}

	void Start() {
		GameManager.Instance.Messenger.AddListener("Player Wins", OnPlayerWins);
		GameManager.Instance.Messenger.AddListener("Game Over", OnGameOver);
		GameManager.Instance.Messenger.AddListener("Collectible Spawned", OnCollectibleCountChanged);
		GameManager.Instance.Messenger.AddListener("Collectible Dropped Off", OnCollectibleCountChanged);
		GameManager.Instance.Messenger.AddListener("Family Hunger Changed", OnHungerChanged);
		GameManager.Instance.Messenger.AddListener("Family Happiness Changed", OnHappinessChanged);
		GameManager.Instance.Messenger.AddListener("Family Illness Changed", OnIllnessChanged);
		GameManager.Instance.Messenger.AddListener("Sack Updated", OnSackUpdated);
	}

	public void OnHungerUpdate(int hunger) {
		if (hungerLabel != null) {
			hungerLabel.text = string.Format("Family Hunger: {0}%", hunger);
		}
	}

	public void OnIllnessUpdate(int illness) {
		if (illnessLabel != null) {
			illnessLabel.text = string.Format("Family Illness: {0}%", illness);
		}
	}

	public void OnHappinessUpdate(int happiness) {
		if (happinessLabel) {
			happinessLabel.text = string.Format("Family Happiness: {0}%", happiness);
		}
	}

	public void UpdateCollectibleCounter(int collectiblesRemaining) {
		if (remainingCollectiblesLabel != null) {
			remainingCollectiblesLabel.text = string.Format("Items Remaining: {0}", collectiblesRemaining);
		}
	}

	public void OnItemsInSackUpdate(int itemsInSack) {
		if (itemsInSackLabel != null) {
			itemsInSackLabel.text = string.Format("Items in Sack: {0}", itemsInSack);
		}
	}

	public void OnPlayAgain() {
		HideYouWin();
		HideGameOver();
		GameManager.Instance.RestartLevel();
	}

	public void ShowYouWin() {
		if (youWinPanel != null) {
			youWinPanel.SetActive(true);
		}
	}

	public void HideYouWin() {
		if (youWinPanel != null) {
			youWinPanel.SetActive(false);
		}
	}

	public void ShowGameOver(string reason) {
		if (gameOverPanel != null) {
			gameOverPanel.SetActive(true);
		}

		if (gameOverReason != null) {
			gameOverReason.text = reason;
		}
	}

	public void HideGameOver() {
		if (gameOverPanel != null) {
			gameOverPanel.SetActive(false);
		}
	}

	public void OnPlayerWins(Message message) {
		ShowYouWin();
	}

	public void OnGameOver(Message message) {
		string reason = (message.data != null ? (string)message.data : "");
		ShowGameOver(reason);
	}

	public void OnCollectibleCountChanged(Message message) {
		UpdateCollectibleCounter(Collectible.CountCollectiblesRemaining());
	}

	public void OnHungerChanged(Message message) {
		Debug.Assert(message.data != null, "OnHungerChanged@GUIManager: Message data is null");

		int hunger = (int)message.data;
		OnHungerUpdate(hunger);
	}

	public void OnHappinessChanged(Message message) {
		Debug.Assert(message.data != null, "OnHappinessChanged@GUIManager: Message data is null");

		int happiness = (int)message.data;
		OnHappinessUpdate(happiness);
	}

	public void OnIllnessChanged(Message message) {
		Debug.Assert(message.data != null, "OnIllnessChanged@GUIManager: Message data is null");

		int illness = (int)message.data;
		OnIllnessUpdate(illness);
	}

	public void OnSackUpdated(Message message) {
		Debug.Assert(message.data != null, "OnSackUpdated@GUIManager: Message data is null");

		Collectible[] collectible = (Collectible[])message.data;
		OnItemsInSackUpdate(collectible.Length);
	}
}
