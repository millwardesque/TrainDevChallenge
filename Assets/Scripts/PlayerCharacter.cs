using UnityEngine;
using System.Collections;

public class PlayerCharacter : Actor {
	public GameObject pickupSack;
	string deathReason;
		
	Animator bodyAnimator;

	public override void Awake() {
		base.Awake();
		Debug.Assert(pickupSack != null, "Player doesn't have pickup-sack attached.");
	}

	// Use this for initialization
	void Start () {
		bodyAnimator = GetComponentInChildren<Animator>();
		State = new ActorStandingState();
		m_direction .Direction = MovementDirection.None;
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
			Debug.Log(string.Format("Shot by bullet {0}", bullet.name));
			deathReason = "You got shot.";
			State = new ActorDeadState();
			return;
		}

		Soldier soldier = col.collider.GetComponent<Soldier>();
		if (soldier != null && (State.Name == "ActorWalkingState" || State.Name == "ActorStandingState")) {
			State = new ActorStunnedState();
			rb.MovePosition(transform.position + ((soldier.transform.position - transform.position).normalized * -0.5f));
			return;
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Dropzone") {
			Collectible[] collectibles = pickupSack.GetComponentsInChildren<Collectible>();

			for (int i = 0; i < collectibles.Length; ++i) {
				GameManager.Instance.GetFamily().AddCollectible(collectibles[i]);
				collectibles[i].transform.SetParent(null);
				collectibles[i].OnDroppedOff();

				GameManager.Instance.Messenger.SendMessage(this, "Collectible Dropped Off");
				GameObject.Destroy(collectibles[i].gameObject);
			}
				
			UpdateSackContents();
		}
		else if (col.tag == "Crater") {
			// @TODO Replace this with a non-hack
			gameObject.layer = LayerMask.NameToLayer("In Cover");
			GetComponentInChildren<SpriteRenderer>().sortingLayerName = "In Cover";
			GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
			transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		if (col.tag == "Crater") {
			// @TODO Replace this with a non-hack
			gameObject.layer = LayerMask.NameToLayer("Player");
			GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Actors";
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	void UpdateSackContents() {
		GameManager.Instance.Messenger.SendMessage(this, "Sack Updated", pickupSack.GetComponentsInChildren<Collectible>());
	}

	public override void SetAnimationTrigger(string trigger) {
		bodyAnimator.SetTrigger(trigger);
	}
}
