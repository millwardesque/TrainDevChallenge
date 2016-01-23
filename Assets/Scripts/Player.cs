﻿using UnityEngine;
using System.Collections;

public enum PlayerState {
	Standing,
	Walking,
	Stunned,
	Dead
};

[RequireComponent (typeof(Rigidbody2D))]
public class Player : MonoBehaviour {
	public float maxSpeed = 2f;
	public float minAnimationSpeed = 0.1f;
	public GameObject pickupSack;
	public float stunnedDuration = 1f;
	public float deathDuration = 1f;
	float stunnedRemaining = 0f;
	float deathRemaining = 0f;
	string deathReason;

	ActorDirection m_direction;

	PlayerState m_state;
	public PlayerState State {
		get { return m_state; }
		set {
			PlayerState oldState = m_state;
			m_state = value;

			if (m_state == PlayerState.Standing) {
				rb.velocity = Vector2.zero;
				m_direction.Direction = MovementDirection.None;

				if (oldState != PlayerState.Walking) {
					bodyAnimator.SetTrigger("Idle Down");
				}
			}
			else if (m_state == PlayerState.Walking) {
			}
			else if (m_state == PlayerState.Stunned) {
				rb.velocity = Vector2.zero;
				stunnedRemaining = stunnedDuration;
			}
			else if (m_state == PlayerState.Dead) {
				rb.velocity = Vector2.zero;
				deathRemaining = deathDuration;
				bodyAnimator.SetTrigger("Dead");
			}
		}
	}

	Rigidbody2D rb;
	Animator bodyAnimator;

	void Awake() {
		m_direction = new ActorDirection(gameObject);
		rb = GetComponent<Rigidbody2D>();
		Debug.Assert(pickupSack != null, "Player doesn't have pickup-sack attached.");
	}

	// Use this for initialization
	void Start () {
		bodyAnimator = GetComponentInChildren<Animator>();
		State = PlayerState.Standing;
		m_direction.Direction = MovementDirection.None;
	}

	void LateUpdate() {
		Utilities.SetYBasedSortOrder(GetComponentInChildren<SpriteRenderer>(), transform.position.y);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GameManager.Instance.State != GameState.Running) {
			return;
		}

		switch (State) {
		case PlayerState.Standing:
			OnStandingFixedUpdate();
			break;
		case PlayerState.Walking:
			OnWalkingFixedUpdate();
			break;
		case PlayerState.Stunned:
			OnStunnedFixedUpdate();
			break;
		case PlayerState.Dead:
			OnDeadFixedUpdate();
			break;
		default:
			break;
		}
	}

	void OnStandingFixedUpdate() {
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		rb.velocity = new Vector2(x, y) * maxSpeed;
		if (Mathf.Abs(x) > float.Epsilon || Mathf.Abs(y) > float.Epsilon) {
			State = PlayerState.Walking;
		}
	}

	void OnWalkingFixedUpdate() {
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		rb.velocity = new Vector2(x, y) * maxSpeed;

		if (Mathf.Abs(x) >= Mathf.Abs(y) && Mathf.Abs(x) > minAnimationSpeed) {
			if (x > minAnimationSpeed) {
				m_direction.Direction = MovementDirection.Right;
			}
			else if (x < -minAnimationSpeed) {
				m_direction.Direction = MovementDirection.Left;
			}
		}
		else if (Mathf.Abs(x) < Mathf.Abs(y) && Mathf.Abs(y) > minAnimationSpeed) {
			if (y < -minAnimationSpeed) {
				m_direction.Direction = MovementDirection.Down;
			}
			else if (y > minAnimationSpeed) {
				m_direction.Direction = MovementDirection.Up;
			}
		}
		else {
			State = PlayerState.Standing;
		}
	}

	void OnStunnedFixedUpdate() {
		stunnedRemaining -= Time.fixedDeltaTime;
		if (stunnedRemaining < float.Epsilon) {
			State = PlayerState.Standing;
		}
	}

	void OnDeadFixedUpdate() {
		deathRemaining -= Time.fixedDeltaTime;
		if (deathRemaining < float.Epsilon) {
			GameManager.Instance.OnGameOver(deathReason);
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
			Debug.Log(string.Format("Shot by bullet {0}", bullet.name));
			deathReason = "You got shot.";
			State = PlayerState.Dead;
			return;
		}

		Soldier soldier = col.collider.GetComponent<Soldier>();
		if (soldier != null && (State == PlayerState.Walking || State == PlayerState.Standing)) {
			State = PlayerState.Stunned;
			rb.MovePosition(transform.position + ((soldier.transform.position - transform.position).normalized * -0.5f));
			return;
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Dropzone") {
			Collectible[] collectiblesRemaining = GameObject.FindObjectsOfType<Collectible>();
			Collectible[] collectibles = pickupSack.GetComponentsInChildren<Collectible>();
			bool hasCollectedAll = (collectiblesRemaining.Length == collectibles.Length);

			for (int i = 0; i < collectibles.Length; ++i) {
				GameManager.Instance.GetFamily().AddCollectible(collectibles[i]);
				collectibles[i].transform.SetParent(null);
				collectibles[i].OnDroppedOff();

				GameManager.Instance.Messenger.SendMessage(this, "Collectible Dropped Off");
				GameObject.Destroy(collectibles[i].gameObject);
			}
				
			UpdateSackContents();

			if (hasCollectedAll) {
				GameManager.Instance.OnPlayerWins();
			}
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

	public void OnDirectionChange(DirectionChange messageObject) {
		DirectionChange change = messageObject;

		switch (change.newDirection) {
		case MovementDirection.Left:
			bodyAnimator.SetTrigger("Walk Left");
			break;
		case MovementDirection.Right:
			bodyAnimator.SetTrigger("Walk Right");
			break;
		case MovementDirection.Up:
			bodyAnimator.SetTrigger("Walk Up");
			break;
		case MovementDirection.Down:
			bodyAnimator.SetTrigger("Walk Down");
			break;
		case MovementDirection.None:
			bodyAnimator.SetTrigger("Stop");
			break;
		default:
			break;
		}
	}
}
