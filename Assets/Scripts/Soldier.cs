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
				GetComponentInChildren<SpriteRenderer>().enabled = true;
				GetComponentInChildren<BoxCollider2D>().enabled = true;

			}

			if (m_state == SoldierState.Firing) {
				rb.velocity = Vector2.zero;
			}
			else if (m_state == SoldierState.Reloading) {
				currentReloadTime = reloadTime;
			}
			else if (m_state == SoldierState.Dead) {
				currentRespawnTime = respawnTime;
				GetComponentInChildren<SpriteRenderer>().enabled = false;
				GetComponentInChildren<BoxCollider2D>().enabled = false;
				rb.velocity = Vector2.zero;
			}
		}
	}

	int health;
	float currentReloadTime;
	float currentRespawnTime;
	Rigidbody2D rb;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
		Debug.Assert(bulletPrefab != null, "Soldier has no bullet prefab");
	}

	void Start () {
		State = SoldierState.Marching;
		health = maxHealth;
	}

	void LateUpdate() {
		Utilities.SetYBasedSortOrder(GetComponentInChildren<SpriteRenderer>(), transform.position.y);
	}

	void FixedUpdate () {
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
		rb.velocity = marchDirection * marchSpeed;
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
			Respawn((Vector2)transform.position + respawnDistance * -marchDirection);
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
			health -= 1;
			if (health <= 0) {
				State = SoldierState.Dead;
			}
		}
	}

	void Respawn(Vector2 position) {
		health = maxHealth;
		transform.position = position;
		State = SoldierState.Marching;
	}
}
