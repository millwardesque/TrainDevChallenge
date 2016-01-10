using UnityEngine;
using System.Collections;

public static class Utilities {

	public static void SetYBasedSortOrder(SpriteRenderer sprite, float y) {
		// Set the correct sort order in the layer
		// Adapted from http://answers.unity3d.com/questions/620318/sprite-layer-order-determined-by-y-value.html
		// This version checks to see if the order has changed to avoid setting a possible (though unconfirmed) dirty flag
		int oldSortingOrder = sprite.sortingOrder;
		int newSortingOrder = Mathf.RoundToInt(y * 100f) * -1;
		if (oldSortingOrder != newSortingOrder) {
			sprite.sortingOrder = newSortingOrder;
		}
	}
}
