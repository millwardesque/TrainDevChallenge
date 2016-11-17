using UnityEngine;
using System.Collections;

public class ActorStunnedState : ActorState {
	float m_stunnedRemaining = 0f;

	public ActorStunnedState() {
		m_name = "ActorStunnedState";
	}

	public override void Enter(Actor actor, ActorState oldState) {
		base.Enter(actor, oldState);

		m_actor.rb.velocity = Vector2.zero;
		m_stunnedRemaining = m_actor.stunnedDuration;
	}

	public override void FixedUpdate() {
		base.FixedUpdate();

		m_stunnedRemaining -= Time.fixedDeltaTime;
		if (m_stunnedRemaining < float.Epsilon) {
			m_actor.State = new ActorStandingState();
		}
	}
}
