using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class Actor : MonoBehaviour {
	public float minAnimationSpeed = 0.05f;
	public float maxSpeed = 100f;
	public float deathDuration = 1f;
	public float stunnedDuration = 1f;

	protected Rigidbody2D m_rb;
	public Rigidbody2D rb {
		get { return m_rb; }
	}

	protected ActorDirection m_direction;
	public ActorDirection Direction {
		get { return m_direction; }
	}

	protected ActorState m_state;
	public ActorState State {
		get { return m_state; }
		set {
			ActorState oldState = m_state;
			if (oldState != null) {
				oldState.Exit();
			}

			m_state = value;
			m_state.Enter(this, oldState);
		}
	}

	bool m_isUserControlled = true;
	public bool IsUserControlled {
		get { return m_isUserControlled; }
		set {
			m_isUserControlled = value;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (GameManager.Instance.State != GameState.Running) {
			return;
		}

		State.FixedUpdate();
	}

	void LateUpdate() {
		Utilities.SetYBasedSortOrder(GetComponentInChildren<SpriteRenderer>(), transform.position.y);
	}

	public virtual void Awake() {
		m_rb = GetComponent<Rigidbody2D>();
		m_direction = new ActorDirection(gameObject);
	}

	public void OnDirectionChange(DirectionChange messageObject) {
		DirectionChange change = messageObject;

		switch (change.newDirection) {
		case MovementDirection.Left:
			SetAnimationTrigger("Walk Left");
			break;
		case MovementDirection.Right:
			SetAnimationTrigger("Walk Right");
			break;
		case MovementDirection.Up:
			SetAnimationTrigger("Walk Up");
			break;
		case MovementDirection.Down:
			SetAnimationTrigger("Walk Down");
			break;
		case MovementDirection.None:
			SetAnimationTrigger("Stop");
			break;
		default:
			break;
		}
	}

	public virtual void SetAnimationTrigger(string trigger) { }
}
