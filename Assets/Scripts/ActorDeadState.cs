using UnityEngine;
using System.Collections;

public class ActorDeadState : ActorState {
	float deathRemaining = 0f;

	public ActorDeadState() {
		m_name = "ActorDeadState";
	}

	public override void Enter(Actor actor, ActorState oldState) {
		base.Enter(actor, oldState);

		m_actor.rb.velocity = Vector2.zero;
		deathRemaining = m_actor.deathDuration;
		m_actor.SetAnimationTrigger("Dead");
	}

	public override void FixedUpdate() {
		base.FixedUpdate();

		deathRemaining -= Time.fixedDeltaTime;
		if (deathRemaining < float.Epsilon) {
			// TODO GameManager.Instance.OnGameOver(deathReason);
		}
	}
}
