using UnityEngine;
using System.Collections;

public class Level1 : Level {
	// Use this for initialization
	void Start () {
		GameManager.Instance.Messenger.AddListener("Collectible Dropped Off", OnCollectibleDroppedOff);
	}

	public void OnCollectibleDroppedOff(Message message) {
		int remaining = Collectible.CountCollectiblesRemaining();
		if (remaining == 0) {
			GameManager.Instance.OnPlayerWins();
		}
	}
}
