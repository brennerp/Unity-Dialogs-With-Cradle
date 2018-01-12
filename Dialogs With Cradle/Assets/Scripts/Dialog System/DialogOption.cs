using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cradle;
using Cradle.StoryFormats.Sugar;

///DialogOption manages the display of one dialog link

namespace DialogSystem {

	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(Button))]

	[System.Serializable]
	public class DialogOption : MonoBehaviour {

		private Button button;
		private Text text;

		void Awake () {
			button = GetComponent<Button>();
			text = GetComponentInChildren<Text>();
		}

		#region General functions
		private void Hide () {
			gameObject.SetActive(false);
		}

		private void Show () {
			gameObject.SetActive(true);
		}

		public void Clear () {
			ClearLinksAndTexts();
			Hide();
		}
		#endregion

		#region UI Button and Text-related functions
		public void InsertLink (StoryLink link) {
			InsertText (link.Text);
			AddLinkListener (link);
			Show ();
		}

		private void AddLinkListener (StoryLink link) {
			button.onClick.AddListener(() => DialogManager.instance.CurrentStory.DoLink((StoryLink) link));
		}

		private void InsertText (string optionText) {
			text.text = optionText;
		}

		private void ClearLinksAndTexts () {
			button.onClick.RemoveAllListeners();
			text.text = "";
		}
		#endregion



	}
}

