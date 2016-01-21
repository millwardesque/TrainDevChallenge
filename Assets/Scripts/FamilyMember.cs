using UnityEngine;
using System.Collections;

public enum FamilyMemberState {
	Standing,
	Dead
};

public class FamilyMember : MonoBehaviour {
	FamilyMemberState m_state;
	public FamilyMemberState State {
		get { return m_state; }
		set {
			FamilyMemberState oldState = m_state;
			m_state = value;
			Debug.Log(string.Format("Family Member state: {0}", m_state));

			if (m_state == FamilyMemberState.Standing) {
				bodyAnimator.SetTrigger("Idle Down");
			}
			else if (m_state == FamilyMemberState.Dead) {
				bodyAnimator.SetTrigger("Dead");
			}
		}
	}

	Animator bodyAnimator;

	// Use this for initialization
	void Start () {
		bodyAnimator = GetComponentInChildren<Animator>();
		State = FamilyMemberState.Standing;
	}
}
