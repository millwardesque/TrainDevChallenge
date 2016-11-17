using UnityEngine;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;


public enum CutsceneState {
	WaitingToStart,
	WaitingForStep,
	NextStep,
	Running,
	Paused,
	Done
};

public class Level1Intro : MonoBehaviour {
	public ProCamera2DCinematics cinematics;
	public GameObject m_cutscene1_1;
	public Actor m_wife;
	PlayerCharacter m_player;

	int m_step = 0;

	CutsceneState m_state;
	public CutsceneState State {
		get { return m_state; }
		protected set {
			m_state = value;

			switch (m_state) {
			case CutsceneState.Running:
				m_player.IsUserControlled = false;
				cinematics.AddCinematicTarget(m_player.transform, 1, -1, 2, EaseType.EaseOut, "", "", -1);
				cinematics.Play();
				State = CutsceneState.NextStep;
				break;
			case CutsceneState.Done:
				cinematics.Stop();
				GameManager.Instance.Messenger.SendMessage(this, "Cutscene finished");
				m_player.IsUserControlled = true;
				break;
			default:
				break;
			}
		}
	}

	// Use this for initialization
	void Start () {
		m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
		State = CutsceneState.WaitingToStart;
	}
	
	// Update is called once per frame
	void Update () {
		switch (State) {
		case CutsceneState.WaitingToStart:
			StartCutscene();
			break;
		case CutsceneState.NextStep:
			m_step++;
			switch (m_step) {
			case 1:
				StartCoroutine("WalkInTogether");
				State = CutsceneState.WaitingForStep;
				break;
			case 2:
				StartCoroutine("FaceEachOtherInTogether");
				State = CutsceneState.WaitingForStep;
				break;
			default:
				State = CutsceneState.Done;
				break;
			}
			break;
		default:
			break;
		}
	}

	void StartCutscene() {
		State = CutsceneState.Running;
	}

	void PauseCutscene() {
		State = CutsceneState.Paused;
	}

	IEnumerator WalkInTogether() {
		while (m_player.transform.position.x < m_cutscene1_1.transform.position.x) {
			m_player.GetComponent<Rigidbody2D>().velocity = m_player.maxSpeed * Time.deltaTime * Vector2.right;
			m_wife.GetComponent<Rigidbody2D>().velocity = m_wife.maxSpeed * Time.deltaTime * Vector2.right;
			yield return null;
		}
		m_player.State = new ActorStandingState();
		m_wife.State = new ActorStandingState();
		m_wife.Direction.Direction = MovementDirection.Right;

		State = CutsceneState.NextStep;
	}

	IEnumerator FaceEachOtherInTogether() {
		while (m_player.transform.position.x < m_wife.transform.position.x + 0.5f) {
			m_player.GetComponent<Rigidbody2D>().velocity = m_player.maxSpeed * Time.deltaTime * Vector2.right;
			yield return null;
		}
		m_player.State = new ActorStandingState();
		m_player.Direction.Direction = MovementDirection.Left;

		State = CutsceneState.NextStep;
	}
}
