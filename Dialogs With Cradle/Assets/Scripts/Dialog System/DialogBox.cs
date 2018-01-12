using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cradle;
using Cradle.StoryFormats.Sugar;

namespace DialogSystem {
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Button))]
	public class DialogBox : MonoBehaviour, CharacterCanvasOriented {

		private GUIStyle style = new GUIStyle();

		public Vector2 maxSize;
		private Vector2 currentSize;

		public Vector2 minMargin;
		public Vector2 padding;

		private RectTransform rect;
		private RectTransform canvasRect;

		private Button button;

		#region CharacterCanvasOriented implementation
		public void PositionToCharacter (Transform character)
		{
			Vector2 characterHeadPosition = character.GetPositionWithSpriteOffset (Vector2.up);
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

		private RectTransform characterLineRect;

		public Text characterLine;

		public float delayBetweenChars = 0.05f;
		private int lineLimit;

		void Awake () {
			rect = GetComponent<RectTransform>();
			button = GetComponent<Button>();
			canvasRect = DialogManager.instance.GetComponent<RectTransform>();
			characterLineRect = characterLine.GetComponent<RectTransform>();
			style.font = characterLine.font;
			style.fontSize = characterLine.fontSize;
		}

		#region General functions
		public void DisplayCharacterLine (CharacterLine characterLine) {
			CalculateSize(characterLine.text);
			SetSize(currentSize);
			PositionToCharacter(DialogManager.GetCharacterTransform (characterLine.character.Trim()));
			InsertText(characterLine.text);
			Show ();
		}

		void Hide () {
			if (gameObject.activeSelf)
				gameObject.SetActive(false);
		}

		void Show () {
			if (!gameObject.activeSelf)
				gameObject.SetActive(true);
		}

		public void Clear () {
			ClearLinks();
			ClearText();
			Hide();
		}
		#endregion

		#region UI Button-related functions
		public void InsertLink (StoryLink link) {
			ClearLinks();
			button.onClick.AddListener(() => DialogManager.instance.CurrentStory.DoLink((StoryLink) link));
		}

		public void InsertEndListener () {
			ClearLinks();
			button.onClick.AddListener(() => DialogManager.instance.EndDialog());
		}

		public void InsertPassListener () {
			ClearLinks();
			button.onClick.AddListener (() => DialogManager.instance.PassCharacterLine());
		}

		private void ClearLinks () {
			button.onClick.RemoveAllListeners();
		}
		#endregion

		#region UI Text and Image functions
		private void InsertText (string line) {
			characterLineRect.SetMargin (padding);
			characterLine.text = line.Trim();
		}

		private void ClearText () {
			characterLine.text = "";
		}

		private void SetSize (Vector2 size) {
			rect.SetSize (size);
			rect.AdjustRectPositionToParent (canvasRect, minMargin);
		}


		private void CalculateSize (string line) {
			ResetExpectedSize();
			CalculateLineExpectedSize(line);
			AddPaddingToExpectedSize();
		}

		private void ResetExpectedSize () {
			currentSize.x = 0f;
			currentSize.y = style.CalcHeight (new GUIContent ("A"), 1) + characterLine.lineSpacing;
		}

		private void CalculateLineExpectedSize (string line) {

			string[] words = line.Split(' ');
			bool oneLine = true;

			foreach (string word in words) {

				Vector2 wordSize = style.CalcSize (new GUIContent(word + "A"));
				currentSize.x += wordSize.x;

				if (currentSize.x >= maxSize.x) {
					//Debug.Log ("More than one line!");
					oneLine = false;
					currentSize.x = wordSize.x;
					currentSize.y += wordSize.y + characterLine.lineSpacing;
				}

			}

			if (!oneLine) {
				currentSize.x = maxSize.x;
			}
		}

		private void AddPaddingToExpectedSize () {
			currentSize.x += padding.x * 2;
			currentSize.y += padding.y * 2;
		}
		#endregion
	}
}

