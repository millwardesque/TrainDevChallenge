using UnityEngine;

public class ActorState {
	protected Actor m_actor;

	protected string m_name;
	public string Name {
		get { return m_name; }
	}

	public virtual void Enter(Actor actor, ActorState oldState) {
		Debug.Log(string.Format("{0}: Changing state from {1} to {2}", actor.name, (oldState == null ? "<null>" : oldState.Name), this.Name));
		m_actor = actor;
	}

	public virtual void Exit() { }
	public virtual void FixedUpdate() { }
}
