using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ArmySpawner))]
public class ArmySpawnerEditor : Editor
{
	void OnSceneGUI () {
		ArmySpawner myTarget = (ArmySpawner)target;
		Color fillColour;
		Color outlineColour;

		if (myTarget.armyType == Army.RedArmy) {
			fillColour = new Color(1f, 0.8f, 0.8f, 0.3f);
			outlineColour = new Color(1f, 0f, 0f, 0.3f);
		}
		else {
			fillColour = new Color(0.8f, 0.8f, 1f, 0.3f);
			outlineColour = new Color(0f, 0f, 1f, 0.3f);
		}

		Handles.DrawSolidRectangleWithOutline(myTarget.spawnArea, fillColour, outlineColour);
	}
}