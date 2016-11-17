using UnityEngine;
using System.Collections;
using Rewired;

public class ActorStandingState : ActorState {
	private Player m_playerInput;

	public ActorStandingState() {
		m_name = "ActorStandingState";
	}

	public override void Enter(Actor actor, ActorState oldState) {
		base.Enter(actor, oldState);

		m_playerInput = ReInput.players.GetPlayer(0);
	
		m_actor.rb.velocity = Vector2.zero;
		m_actor.Direction.Direction = MovementDirection.None;

		if (oldState != null && oldState.Name == "ActorWalkingState") {
			m_actor.SetAnimationTrigger("Idle Down");
		}
	}

	public override void FixedUpdate() {
		base.FixedUpdate();

		if (m_actor.IsUserControlled) {
			float x = m_playerInput.GetAxis("Move Horizontal");
			float y = m_playerInput.GetAxis("Move Vertical");

			m_actor.rb.velocity = new Vector2(x, y) * m_actor.maxSpeed * Time.fixedDeltaTime;
		}

		if (Mathf.Abs(m_actor.rb.velocity.x) > float.Epsilon || Mathf.Abs(m_actor.rb.velocity.y) > float.Epsilon) {
			m_actor.State = new ActorWalkingState();
		}
	}
}
