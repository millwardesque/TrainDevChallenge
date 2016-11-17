using UnityEngine;
using System.Collections;
using Rewired;

public class ActorWalkingState : ActorState {
	private Player m_playerInput;

	public ActorWalkingState() {
		m_name = "ActorWalkingState";
	}

	public override void Enter (Actor actor, ActorState oldState)
	{
		base.Enter (actor, oldState);
		m_playerInput = ReInput.players.GetPlayer(0);
	}

	public override void FixedUpdate() {
		base.FixedUpdate();

		if (m_actor.IsUserControlled) {
			float x = m_playerInput.GetAxis("Move Horizontal");
			float y = m_playerInput.GetAxis("Move Vertical");
			bool isRunning = m_playerInput.GetButton("Run");
			m_actor.rb.velocity = new Vector2(x, y) * m_actor.maxSpeed * Time.fixedDeltaTime * (isRunning ? 1.5f : 1f);
		}

		if (Mathf.Abs(m_actor.rb.velocity.x) >= Mathf.Abs(m_actor.rb.velocity.y) && Mathf.Abs(m_actor.rb.velocity.x) > m_actor.minAnimationSpeed) {
			if (m_actor.rb.velocity.x > m_actor.minAnimationSpeed) {
				m_actor.Direction.Direction = MovementDirection.Right;
			}
			else if (m_actor.rb.velocity.x < -m_actor.minAnimationSpeed) {
				m_actor.Direction.Direction = MovementDirection.Left;
			}
		}
		else if (Mathf.Abs(m_actor.rb.velocity.x) < Mathf.Abs(m_actor.rb.velocity.y) && Mathf.Abs(m_actor.rb.velocity.y) > m_actor.minAnimationSpeed) {
			if (m_actor.rb.velocity.y < -m_actor.minAnimationSpeed) {
				m_actor.Direction.Direction = MovementDirection.Down;
			}
			else if (m_actor.rb.velocity.y > m_actor.minAnimationSpeed) {
				m_actor.Direction.Direction = MovementDirection.Up;
			}
		}
		else {
			m_actor.State = new ActorStandingState();
		}
	}
}
