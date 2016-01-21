using UnityEngine;
using System.Collections;

public class Family : MonoBehaviour {
	public float maxHunger = 100f;
	public float maxIllness = 100f;
	public float hungerIncreaseRate = 1f;
	public float illnessIncreaseRate = 1f;

	float m_hunger;
	public float Hunger {
		get { return m_hunger; }
		set {
			// Check if greater than max *before* the Clamp call just in case of floating-point precision errors.
			if (value >= maxHunger) {
				FamilyMember[] familyMembers = GetComponentsInChildren<FamilyMember>();
				for (int i = 0; i < familyMembers.Length; ++i) {
					familyMembers[i].State = FamilyMemberState.Dead;
				}

				GameManager.Instance.OnGameOver("Your family has starved to death.");
			}

			m_hunger = Mathf.Clamp(value, 0, maxHunger);
			GameManager.Instance.GetGUIManager().OnHungerUpdate(Mathf.FloorToInt(m_hunger));
		}
	}

	float m_illness;
	public float Illness {
		get { return m_illness; }
		set {
			// Check if greater than max *before* the Clamp call just in case of floating-point precision errors.
			if (value >= maxIllness) {
				FamilyMember[] familyMembers = GetComponentsInChildren<FamilyMember>();
				for (int i = 0; i < familyMembers.Length; ++i) {
					familyMembers[i].State = FamilyMemberState.Dead;
				}

				GameManager.Instance.OnGameOver("Your family has gotten too sick and died.");
			}

			m_illness = Mathf.Clamp(value, 0, maxIllness);
			GameManager.Instance.GetGUIManager().OnIllnessUpdate(Mathf.FloorToInt(m_illness));
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.Instance.State != GameState.Running) {
			return;
		}

		Hunger += hungerIncreaseRate * Time.deltaTime;
		Illness += illnessIncreaseRate * Time.deltaTime;
	}
}
