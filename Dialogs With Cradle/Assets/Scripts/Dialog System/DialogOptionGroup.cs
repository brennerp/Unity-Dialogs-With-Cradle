using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cradle;
using Cradle.StoryFormats.Sugar;

///Old Lady Game Project
///Written by Brenner Pacelli on October 25th, 2017
///Last updated on December 14th, 2017
///DialogOptionGroup manages all DialogOption scripts

namespace DialogSystem {

	[RequireComponent(typeof(RectTransform))]
	public class DialogOptionGroup : MonoBehaviour, CharacterCanvasOriented {

		private RectTransform rect;
		private RectTransform canvasRect;

		public DialogOption[] options;
		private int currentOptionIndex;

		#region CharacterCanvasOriented implementation
		public void PositionToCharacter (Transform character)
		{
			Vector2 characterHeadPosition = character.GetPositionWithSpriteOffset (Vector2.zero);
			Vector2 characterHeadOnCanvas = canvasRect.WorldToScreenRectTransformPoint(characterHeadPosition);
			rect.SetPosition(characterHeadOnCanvas);
		}

		public RectTransform Rect {
			get {
				return rect;
			}
		}

		public RectTransform CanvasRect {
			get {
				return canvasRect;
			}
		}
		#endregion

		void Awake () {
			rect = GetComponent<RectTransform>();
			canvasRect = DialogManager.instance.GetComponent<RectTransform>();
		}

		public void InsertOption (StoryLink link) {
			options[currentOptionIndex].InsertLink (link);
			currentOptionIndex++;

			if (currentOptionIndex >= options.Length) {
				Debug.LogError ("ERROR in DialogOptionGroup: number of links surpassed array size!");
				currentOptionIndex = 0;
			}
		}

		public void Clear () {
			ClearOptions();
			currentOptionIndex = 0;
		}

		private void ClearOptions () {
			foreach (DialogOption o in options) {
				o.Clear();
			}
		}
	}

}
