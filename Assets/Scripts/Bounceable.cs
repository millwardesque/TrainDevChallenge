using UnityEngine;
using System.Collections;


public class Bounceable : MonoBehaviour {
	public float bounceHeight = 0.5f;
	public float bounceDuration = 1.0f;
	bool isBouncingUp = true;

	float bounceTime = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bounceTime += Time.deltaTime;
		if (bounceTime > bounceDuration / 2f) {
			isBouncingUp ^= true;
			bounceTime -= bounceDuration;
		}

		if (isBouncingUp) {
			transform.position += new Vector3(0f, Time.deltaTime * bounceHeight / bounceDuration / 2f, 0f);
		}
		else {
			transform.position -= new Vector3(0f,  Time.deltaTime * bounceHeight / bounceDuration / 2f, 0f);
		}
	}
}
