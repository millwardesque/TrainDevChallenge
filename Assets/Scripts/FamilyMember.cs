using UnityEngine;
using System.Collections;

public class FamilyMember : Actor {
	Animator bodyAnimator;

	// Use this for initialization
	void Start () {
		bodyAnimator = GetComponentInChildren<Animator>();
		State = new ActorStandingState();
		m_direction.Direction = MovementDirection.None;
		IsUserControlled = false;
	}

	public override void SetAnimationTrigger(string trigger) {
		bodyAnimator.SetTrigger(trigger);
	}
}
