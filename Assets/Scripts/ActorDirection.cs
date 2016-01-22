using UnityEngine;
using System.Collections;

public enum MovementDirection {
	None,
	Up,
	Down,
	Left,
	Right
};

public class DirectionChange {
	public MovementDirection oldDirection;
	public MovementDirection newDirection;

	public DirectionChange(MovementDirection oldDirection, MovementDirection newDirection) {
		this.oldDirection = oldDirection;
		this.newDirection = newDirection;
	}

	public override string ToString() {
		return "Direction change from " + oldDirection.ToString() + " to " + newDirection.ToString();
	}
};

public class ActorDirection {
	GameObject actor;

	MovementDirection m_direction;
	public MovementDirection Direction {
		get { return m_direction; }
		set {
			MovementDirection oldDirection = m_direction;
			m_direction = value;

			if (oldDirection != m_direction) {
				DirectionChange directionChange = new DirectionChange(oldDirection, m_direction);
				actor.BroadcastMessage("OnDirectionChange", directionChange);
			}
		}
	}

	public ActorDirection(GameObject actor) {
		this.actor = actor;
	}
}
