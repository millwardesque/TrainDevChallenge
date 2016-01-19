using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class Player : MonoBehaviour {
	public float maxSpeed = 2f;
	public float minAnimationSpeed = 0.1f;
	public GameObject pickupSack;

	Rigidbody2D rb;
	Animator bodyAnimator;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
		Debug.Assert(pickupSack != null, "Player doesn't have pickup-sack attached.");
	}

	// Use this for initialization
	void Start () {
		bodyAnimator = GetComponentInChildren<Animator>();
	}

	void LateUpdate() {
		Utilities.SetYBasedSortOrder(GetComponentInChildren<SpriteRenderer>(), transform.position.y);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GameManager.Instance.State != GameState.Running) {
			return;
		}

		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		rb.velocity = new Vector2(x, y) * maxSpeed;

		if (Mathf.Abs(x) >= Mathf.Abs(y) && Mathf.Abs(x) > minAnimationSpeed) {
			if (x > minAnimationSpeed) {
				bodyAnimator.SetTrigger("Walk Right");
			}
			else if (x < -minAnimationSpeed) {
				bodyAnimator.SetTrigger("Walk Left");
			}
		}
		else if (Mathf.Abs(x) < Mathf.Abs(y) && Mathf.Abs(y) > minAnimationSpeed) {
			if (y < -minAnimationSpeed) {
				bodyAnimator.SetTrigger("Walk Down");
			}
			else if (y > minAnimationSpeed) {
				bodyAnimator.SetTrigger("Walk Up");
			}
		}
		else {
			bodyAnimator.SetTrigger("Stop");
		}
	}

	void OnCollisionEnter2D(Collision2D col) {
		Collectible collectible = col.collider.GetComponent<Collectible>();
		if (collectible != null) {
			collectible.transform.SetParent(pickupSack.transform, false);
			collectible.transform.position = new Vector2(0f, 0f);
			collectible.OnPickedUp();
			UpdateSackContents();
			return;
		}

		Bullet bullet = col.collider.GetComponent<Bullet>();
		if (bullet != null) {
			GameManager.Instance.OnGameOver("You got shot.");
			return;
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Dropzone") {
			Collectible[] collectiblesRemaining = GameObject.FindObjectsOfType<Collectible>();
			Collectible[] collectibles = pickupSack.GetComponentsInChildren<Collectible>();
			bool hasCollectedAll = (collectiblesRemaining.Length == collectibles.Length);

			for (int i = 0; i < collectibles.Length; ++i) {
				collectibles[i].transform.SetParent(null);
				GameObject.Destroy(collectibles[i].gameObject);

				GameManager.Instance.GetFamily().Hunger += collectibles[i].hungerAdjustment;
				GameManager.Instance.GetFamily().Illness += collectibles[i].illnessAdjustment;
			}
			GameManager.Instance.GetGUIManager().OnCollectibleUpdate(collectiblesRemaining.Length - collectibles.Length);

			UpdateSackContents();

			if (hasCollectedAll) {
				GameManager.Instance.OnPlayerWins();
			}
		}
	}

	void UpdateSackContents() {
		GameManager.Instance.GetGUIManager().OnItemsInSackUpdate(pickupSack.GetComponentsInChildren<Collectible>().Length);
	}
}
