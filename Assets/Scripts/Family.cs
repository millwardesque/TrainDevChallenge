using UnityEngine;
using System.Collections;

public class Family : MonoBehaviour {
	public float maxHunger = 100f;
	public float maxIllness = 100f;
	public float maxHappiness = 100f;
	public float hungerIncreaseRate = 1f;
	public float illnessIncreaseRate = 1f;
	public float happinessIncreaseRate = 1f;

	float m_hunger;
	public float Hunger {
		get { return m_hunger; }
		set {
			if (Mathf.Abs(value - m_hunger) <= float.Epsilon) {
				return;
			}

			// Check if greater than max *before* the Clamp call just in case of floating-point precision errors.
			if (value >= maxHunger) {
				FamilyMember[] familyMembers = GetComponentsInChildren<FamilyMember>();
				for (int i = 0; i < familyMembers.Length; ++i) {
					familyMembers[i].State = FamilyMemberState.Dead;
				}

				GameManager.Instance.OnGameOver("Your family has starved to death.");
			}

			m_hunger = Mathf.Clamp(value, 0, maxHunger);
			GameManager.Instance.Messenger.SendMessage(this, "Family Hunger Changed", Mathf.FloorToInt(m_hunger));
		}
	}

	float m_illness;
	public float Illness {
		get { return m_illness; }
		set {
			if (Mathf.Abs(value - m_illness) <= float.Epsilon) {
				return;
			}

			// Check if greater than max *before* the Clamp call just in case of floating-point precision errors.
			if (value >= maxIllness) {
				FamilyMember[] familyMembers = GetComponentsInChildren<FamilyMember>();
				for (int i = 0; i < familyMembers.Length; ++i) {
					familyMembers[i].State = FamilyMemberState.Dead;
				}

				GameManager.Instance.OnGameOver("Your family has gotten too sick and died.");
			}

			m_illness = Mathf.Clamp(value, 0, maxIllness);
			GameManager.Instance.Messenger.SendMessage(this, "Family Illness Changed", Mathf.FloorToInt(m_illness));
		}
	}

	float m_happiness;
	public float Happiness {
		get { return m_happiness; }
		set {
			if (Mathf.Abs(value - m_happiness) <= float.Epsilon) {
				return;
			}

			// Check if greater than max *before* the Clamp call just in case of floating-point precision errors.
			if (value >= maxHappiness) {
				GameManager.Instance.OnPlayerWins();
			}

			m_happiness = Mathf.Clamp(value, 0, maxHappiness);
			GameManager.Instance.Messenger.SendMessage(this, "Family Happiness Changed", Mathf.FloorToInt(m_happiness));
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.Instance.State != GameState.Running) {
			return;
		}

		Hunger += hungerIncreaseRate * Time.deltaTime;
		Illness += illnessIncreaseRate * Time.deltaTime;
		Happiness += happinessIncreaseRate * Time.deltaTime;
	}

	public void AddCollectible(Collectible collectible) {
		Hunger += collectible.hungerAdjustment;
		Illness += collectible.illnessAdjustment;
		Happiness += collectible.happinessAdjustment;
	}
}
