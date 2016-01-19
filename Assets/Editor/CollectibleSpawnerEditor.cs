using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CollectibleSpawner))]
public class CollectibleSpawnerEditor : Editor {
	void OnSceneGUI () {
		CollectibleSpawner myTarget = (CollectibleSpawner)target;
		Color fillColour;
		Color outlineColour;

		fillColour = new Color(0.8f, 1.0f, 0.8f, 0.3f);
		outlineColour = new Color(0f, 1f, 0f, 0.3f);

		Handles.DrawSolidRectangleWithOutline(myTarget.spawnArea, fillColour, outlineColour);
	}
}
