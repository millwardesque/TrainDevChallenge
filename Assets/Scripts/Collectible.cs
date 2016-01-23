using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {
	public float hungerAdjustment = 0f;
	public float illnessAdjustment = 0f;
	public float happinessAdjustment = 0f;

	bool hasBeenDroppedOff = false;

	public void OnPickedUp() {
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;

		GetComponentInChildren<Bounceable>().gameObject.SetActive(false);
	}

	public void OnDroppedOff() {
		hasBeenDroppedOff = true;
	}

	public static int CountCollectiblesRemaining() {
		int count = 0;
		Collectible[] collectibles = GameObject.FindObjectsOfType<Collectible>();
		for (int i = 0; i < collectibles.Length; ++i) {
			if (!collectibles[i].hasBeenDroppedOff) {
				count++;
			}
		}

		return count;
	}
}
