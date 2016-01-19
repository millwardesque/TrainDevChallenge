using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public Text hungerLabel;
	public Text illnessLabel;
	public Text remainingCollectiblesLabel;
	public Text itemsInSackLabel;
	public GameObject youWinPanel;
	public GameObject gameOverPanel;
	public Text gameOverReason;

	void Awake() {
		Debug.Assert(hungerLabel != null, "GUIManager: Hunger label is null");
		Debug.Assert(illnessLabel != null, "GUIManager: Illness label is null");
		Debug.Assert(remainingCollectiblesLabel != null, "GUIManager: Remaining Collectibles label is null");
		Debug.Assert(itemsInSackLabel != null, "GUIManager: Items-in-sack label is null");
		Debug.Assert(youWinPanel != null, "GUIManager: You-Win panel is null");
		Debug.Assert(gameOverPanel != null, "GUIManager: Game-Over panel is null");
		Debug.Assert(gameOverReason != null, "GUIManager: Game-Over reason is null");
	}

	public void OnHungerUpdate(int hunger) {
		hungerLabel.text = string.Format("Family Hunger: {0}%", hunger);
	}

	public void OnIllnessUpdate(int illness) {
		illnessLabel.text = string.Format("Family Illness: {0}%", illness);
	}

	public void OnCollectibleUpdate(int collectiblesRemaining) {
		remainingCollectiblesLabel.text = string.Format("Items Remaining: {0}", collectiblesRemaining);
	}

	public void OnItemsInSackUpdate(int itemsInSack) {
		itemsInSackLabel.text = string.Format("Items in Sack: {0}", itemsInSack);
	}

	public void OnPlayAgain() {
		HideYouWin();
		HideGameOver();
		GameManager.Instance.RestartLevel();
	}

	public void ShowYouWin() {
		youWinPanel.SetActive(true);
	}

	public void HideYouWin() {
		youWinPanel.SetActive(false);
	}

	public void ShowGameOver(string reason) {
		gameOverPanel.SetActive(true);
		gameOverReason.text = reason;
	}

	public void HideGameOver() {
		gameOverPanel.SetActive(false);
	}
}
