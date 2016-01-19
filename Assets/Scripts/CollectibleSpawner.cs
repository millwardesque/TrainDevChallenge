using UnityEngine;
using UnityEditor;
using System.Collections;

public class CollectibleSpawner : MonoBehaviour {
	public Collectible foodPrefab;
	public Collectible healthPrefab;

	public int startingFood = 10;
	public int startingHealth = 10;

	public Rect spawnArea;

	void Awake() {
		Debug.Assert(foodPrefab != null, string.Format("Item Spawner '{0}': Food prefab is null", name));
		Debug.Assert(healthPrefab != null, string.Format("Item Spawner '{0}': Health prefab is null", name));
	}

	public void SpawnCollectibles() {
		for (int i = 0; i < startingFood; ++i) {
			SpawnFood();
		}

		for (int i = 0; i < startingHealth; ++i) {
			SpawnHealth();
		}
	}

	public void SpawnFood() {
		Collectible newFood = Instantiate<Collectible>(foodPrefab);
		SetRandomPosition(newFood.transform);
	}

	public void SpawnHealth() {
		Collectible newHealth = Instantiate<Collectible>(healthPrefab);
		SetRandomPosition(newHealth.transform);
	}

	void SetRandomPosition(Transform itemTransform) {
		itemTransform.SetParent(transform, false);
		float newX = Random.Range(spawnArea.xMin, spawnArea.xMax);
		float newY = Random.Range(spawnArea.yMin, spawnArea.yMax);
		itemTransform.position = new Vector2(newX, newY);
	}
}
