using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {
	//This is an adapted version of a sample script from Unity.
	//This is just a quick example of how to use an Animator with the Retro 2D Characters.
	//In this script: get input from horizontal and vertical axis. Pass movement to animator variables and move GameObject.
	//The animator itself controls which animations to play, depending on, for instance, if vertical movement is greater than a certain value.
	Animator animator;
	float h;
	float v;
	
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");

		if (v != 0.0f) {
			transform.Translate(0.0f, (1.0f * v) * Time.deltaTime, 0.0f);
		}

		if (h != 0.0f) {
			transform.Translate((1.0f * h) * Time.deltaTime, 0.0f, 0.0f);
		}

		animator.SetFloat("Vertical Speed",v);
		animator.SetFloat("Horizontal Speed",h);
	}
}