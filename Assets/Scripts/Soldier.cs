using UnityEngine;
using System.Collections;

public enum Army {
	RedArmy,
	BlueArmy
};

public enum SoldierState {
	Marching,
	Firing,
	Reloading,
	Dead
};

[RequireComponent (typeof(Rigidbody2D))]
public class Soldier : MonoBehaviour {
	public Army army;
	public Vector2 marchDirection;
	public float firingDistance = 2f;
	public float marchSpeed = 1f;
	public float reloadTime = 2f;
	public float respawnDistance = 10f;
	public float respawnTime = 2f;
	public int maxHealth = 2;
	public Bullet bulletPrefab;
	public float accuracyDeviation = 0.1f;
	public ArmySpawner mySpawner;

	Animator myAnimator;

	int m_health;
	public int Health {
		get { return m_health; }
		set {
			m_health = value;
			if (m_health <= 0) {
				State = SoldierState.Dead;
			}
		}
	}

	Soldier m_nearestEnemy;
	public Soldier NearestEnemy {
		get { return m_nearestEnemy; }
	}

	SoldierState m_state;
	public SoldierState State {
		get { return m_state; }
		set {
			SoldierState oldState = m_state;
			m_state = value;

			if (oldState == SoldierState.Dead) {
				GetComponentInChildren<BoxCollider2D>().enabled = true;
			}

			if (m_state == SoldierState.Firing) {
				SetAnimationTrigger("Stop");
				rb.velocity = Vector2.zero;
			}
			else if (m_state == SoldierState.Marching) {
				string marchDirectionName = "Walk Up";

				if (Mathf.Abs(marchDirection.x) >= Mathf.Abs(marchDirection.y)) {
					if (marchDirection.x > 0) {
						marchDirectionName = "Walk Right";
					}
					else {
						marchDirectionName = "Walk Left";
					}
				}
				else {
					if (marchDirection.y > 0) {
						marchDirectionName = "Walk Up";
					}
					else {
						marchDirectionName = "Walk Down";
					}
				}
				SetAnimationTrigger(marchDirectionName);
			}
			else if (m_state == SoldierState.Reloading) {
				SetAnimationTrigger("Stop");
				currentReloadTime = reloadTime;
			}
			else if (m_state == SoldierState.Dead) {
				SetAnimationTrigger("Dead");
				currentRespawnTime = respawnTime;
				GetComponentInChildren<BoxCollider2D>().enabled = false;
				rb.velocity = Vector2.zero;
			}
		}
	}

	float currentReloadTime;
	float currentRespawnTime;
	Rigidbody2D rb;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
		Debug.Assert(bulletPrefab != null, "Soldier has no bullet prefab");
	}

	void Start () {
		if (mySpawner == null) {
			Debug.LogAssertion(string.Format("Soldier '{0}' has no spawner", name));
		}
		myAnimator = GetComponentInChildren<Animator>();
		State = SoldierState.Marching;
		Health = maxHealth;

		if (army == Army.BlueArmy) {
			GetComponentInChildren<SpriteRenderer>().color = new Color(0.3f, 0.3f, 1f);
		}
		else if (army == Army.RedArmy) {
			GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 0.3f, 0.3f);
		}
	}

	void LateUpdate() {
		Utilities.SetYBasedSortOrder(GetComponentInChildren<SpriteRenderer>(), transform.position.y);
	}

	void FixedUpdate () {
		if (GameManager.Instance.State != GameState.Running) {
			return;
		}

		switch (State) {
		case SoldierState.Marching:
			FixedUpdateMarching();
			break;
		case SoldierState.Firing:
			FixedUpdateFiring();
			break;
		case SoldierState.Reloading:
			FixedUpdateReloading();
			break;
		case SoldierState.Dead:
			FixedUpdateDead();
			break;
		default:
			break;
		}
	}

	void FixedUpdateMarching() {
		// Find the nearest enemy soldier to figure out if we should stop marching
		// This doesn't need to be recalculated every physics frame, but it's simpler to implement right now...
		FindNearestEnemy();

		if (m_nearestEnemy != null) {
			float distance = (transform.position - m_nearestEnemy.transform.position).magnitude;
			if (distance <= firingDistance) {
				State = SoldierState.Firing;
				return;
			}
		}

		// March!
		rb.velocity = marchDirection * marchSpeed * Time.fixedDeltaTime;
	}

	void FixedUpdateFiring() {
		// Check to see if we have a target to shoot.
		if (m_nearestEnemy == null || m_nearestEnemy.State == SoldierState.Dead) {
			FindNearestEnemy();

			// If there's *no* valid target to shoot, resume the march.
			if (m_nearestEnemy == null) {
				State = SoldierState.Marching;
				return;
			}
			else {
				// If the nearest target is too far to shoot, resume the march.
				float distance = (transform.position - m_nearestEnemy.transform.position).magnitude;
				if (distance > firingDistance) {
					State = SoldierState.Marching;
					return;
				}
			}
		}

		float enemydistance = (transform.position - m_nearestEnemy.transform.position).magnitude;
		if (enemydistance > firingDistance) {
			State = SoldierState.Marching;
			return;
		}

		Vector2 enemyDirection = (Vector2)(m_nearestEnemy.transform.position - transform.position);
		enemyDirection.Normalize();
		enemyDirection.x += Random.Range(-accuracyDeviation, accuracyDeviation);
		enemyDirection.y += Random.Range(-accuracyDeviation, accuracyDeviation);
		enemyDirection.Normalize();

		// Spawn bullet
		Bullet newBullet = Instantiate<Bullet>(bulletPrefab);
		newBullet.gameObject.name = "Bullet - (" + name + ")";
		newBullet.shooter = gameObject;
		newBullet.transform.position = transform.position;
		newBullet.transform.position += (Vector3)(enemyDirection * 1.1f);	// @TODO Fix this hack. Shifts the bullet out of the shooter's collider

		// Fire bullet.
		newBullet.Fire(enemyDirection);

		// Reload.
		State = SoldierState.Reloading;
	}

	void FixedUpdateReloading() {
		currentReloadTime -= Time.fixedDeltaTime;
		if (currentReloadTime <= 0f) {
			State = SoldierState.Firing;
		}
	}

	void FixedUpdateDead() {
		currentRespawnTime -= Time.fixedDeltaTime;
		if (currentRespawnTime <= 0f) {
			Respawn();
			return;
		}
	}

	void FindNearestEnemy() {
		Vector2 myPosition = transform.position;
		Soldier newNearestEnemy = null;
		Soldier[] soldiers = GameObject.FindObjectsOfType<Soldier>();
		float nearestSqDistance = 0f;
		for (int i = 0; i < soldiers.Length; ++i) {
			if (soldiers[i].army == army || soldiers[i].State == SoldierState.Dead) {
				continue;
			}

			float sqDistance = (myPosition - (Vector2)soldiers[i].transform.position).sqrMagnitude;
			if (newNearestEnemy == null || sqDistance < nearestSqDistance) {
				nearestSqDistance = sqDistance;
				newNearestEnemy = soldiers[i];
			}
		}

		m_nearestEnemy = newNearestEnemy;
	}

	void OnCollisionEnter2D(Collision2D col) {
		Bullet bullet = col.collider.GetComponent<Bullet>();
		if (bullet != null) {
			Health -= 1;
		}
	}

	void Respawn() {
		if (mySpawner != null) {
			mySpawner.RespawnSoldier(this);
		}
	}

	void SetAnimationTrigger(string triggerName) {
		if (myAnimator != null) {
			myAnimator.SetTrigger(triggerName);
		}
	}
}
