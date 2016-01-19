using UnityEngine;
using UnityEditor;
using System.Collections;

public class ArmySpawner : MonoBehaviour {
	public Army armyType;
	public Vector2 marchDirection;
	public Soldier soldierPrefab;
	public float respawnTime;

	public Rect spawnArea;

	void Awake() {
		Debug.Assert(soldierPrefab != null, string.Format("Army Spawner '{0}': Soldier prefab is null", name));
	}

	public void SpawnSoldier() {
		Soldier newSoldier = Instantiate<Soldier>(soldierPrefab);
		RespawnSoldier(newSoldier);
	}

	public void RespawnSoldier(Soldier soldier) {
		// Set position
		soldier.transform.SetParent(transform, false);
		float newX = Random.Range(spawnArea.xMin, spawnArea.xMax);
		float newY = Random.Range(spawnArea.yMin, spawnArea.yMax);
		soldier.transform.position = new Vector2(newX, newY);

		// Set parameters
		soldier.marchDirection = marchDirection;
		soldier.army = armyType;
		soldier.respawnTime = respawnTime;
		soldier.Health = soldier.maxHealth;
		soldier.State = SoldierState.Marching;
		soldier.mySpawner = this;
	}
}
