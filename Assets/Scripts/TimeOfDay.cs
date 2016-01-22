using UnityEngine;
using System.Collections;

public class TimeOfDay : MonoBehaviour {
	public Color ambientDayColour = new Color(0.8f, 0.9f, 1f);
	public Color ambientNightColour = new Color(0.5f, 0.1f, 0.5f);
	public float secondsPerGameHour = 1f;
	public float startingGameHour = 3f;

	float m_gameHour;
	public float GameHour {
		get { return m_gameHour; }
		set {
			m_gameHour = value;
			m_gameHour = m_gameHour % 24f;
		}
	}

	void Start() {
		GameHour = startingGameHour;
	}
	
	// Update is called once per frame
	void Update () {
		GameHour += Time.deltaTime * 1f / secondsPerGameHour;

		if (GameHour >= 4f && GameHour < 6f) {
			Color color = Color.Lerp(ambientNightColour, ambientDayColour, (GameHour - 4f) / (6f - 4f));
			SetAmbientColour(color);
		}
		else if (GameHour >= 6f && GameHour < 20f) {
			SetAmbientColour(ambientDayColour);
		}
		else if (GameHour >= 20f && GameHour < 22f) {
			Color color = Color.Lerp(ambientDayColour, ambientNightColour, (GameHour - 20f) / (22f - 20f));
			SetAmbientColour(color);
		}
		else if (GameHour >= 22f || GameHour < 4f) {
			SetAmbientColour(ambientNightColour);
		}
	}

	void SetAmbientColour(Color colour) {
		RenderSettings.ambientSkyColor = colour;
	}
}
