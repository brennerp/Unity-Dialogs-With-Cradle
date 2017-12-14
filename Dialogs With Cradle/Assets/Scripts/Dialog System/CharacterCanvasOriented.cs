using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Old Lady Game Project
///Written by Brenner Pacelli on November 13th, 2017
///Last updated on November 13th, 2017
///CharacterCanvasOriented is an interface for objects in the canvas that need to align with objects in scene

public interface CharacterCanvasOriented {

	RectTransform Rect { get; }
	RectTransform CanvasRect { get; }

	void PositionToCharacter (Transform character);
}
