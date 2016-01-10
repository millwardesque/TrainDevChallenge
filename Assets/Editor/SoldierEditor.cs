using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Soldier))]
public class SoldierEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Soldier myTarget = (Soldier)target;

		DrawDefaultInspector();
		EditorGUILayout.LabelField("State", myTarget.State.ToString());
		EditorGUILayout.ObjectField("Nearest Enemy", myTarget.NearestEnemy, typeof(Soldier), false);
	}
}