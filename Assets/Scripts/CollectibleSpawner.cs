using UnityEngine;
using UnityEditor;
using System.Collections;

public class CollectibleSpawner : MonoBehaviour {
	public Collectible collectiblePrefab;
	public int startingNumber = 10;
	public Rect spawnArea;

	void Awake() {
		Debug.Assert(collectiblePrefab != null, string.Format("Item Spawner '{0}': Collectible prefab is null", name));
	}

	public void SpawnCollectibles() {
		for (int i = 0; i < startingNumber; ++i) {
			SpawnCollectible();
		}
	}

	public void SpawnCollectible() {
		Collectible newCollectible = Instantiate<Collectible>(collectiblePrefab);
		SetRandomPosition(newCollectible.transform);
		GameManager.Instance.Messenger.SendMessage(this, "Collectible Spawned");
	}

	void SetRandomPosition(Transform itemTransform) {
		itemTransform.SetParent(transform, false);
		float newX = Random.Range(spawnArea.xMin, spawnArea.xMax);
		float newY = Random.Range(spawnArea.yMin, spawnArea.yMax);
		itemTransform.position = new Vector2(newX, newY);
	}
}
