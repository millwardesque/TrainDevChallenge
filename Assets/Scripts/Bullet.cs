using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {
	public GameObject shooter;	// Shooter who fired the bullet.
	public float firingSpeed = 5f;	// Speed of the bullet.
	public float lifespan = 3;	// Lifespan of the bullet in seconds.

	bool wasFired = false;

	Rigidbody2D rb;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
	}

	void Update() {
		if (wasFired) {
			lifespan -= Time.deltaTime;

			if (lifespan <= 0f) {
				Destroy(gameObject);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col) {
		// The other collider can handle damage, etc. This just cleans up the bullet.
		if (col.collider.gameObject != shooter) {
			GameObject.Destroy(gameObject);
		}
	}

	public void Fire(Vector2 direction) {
		rb.velocity = direction * firingSpeed;
		wasFired = true;
	}
}
