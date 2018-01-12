using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CharacterCanvasOriented {

	RectTransform Rect { get; }
	RectTransform CanvasRect { get; }

	void PositionToCharacter (Transform character);
}
