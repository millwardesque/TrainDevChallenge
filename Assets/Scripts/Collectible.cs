using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {
	public float hungerAdjustment = 0f;
	public float illnessAdjustment = 0f;
	public float happinessAdjustment = 0f;

	public void OnPickedUp() {
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;

		GetComponentInChildren<Bounceable>().gameObject.SetActive(false);
	}
}
