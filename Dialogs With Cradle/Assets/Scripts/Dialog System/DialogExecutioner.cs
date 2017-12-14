using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cradle;
using Cradle.StoryFormats.Sugar;

///Old Lady Game Project
///Written by Brenner Pacelli on December 6th, 2017
///Last updated on December 14th, 2017
///DialogExecution executes all actions in the passage
///It has all possible special actions incripted and coordinates the DialogBox and the DialogOptionGroup

namespace DialogSystem {
	[RequireComponent(typeof(DialogManager))]
	public class DialogExecutioner : MonoBehaviour {

		private bool ignoreNextLinkAction;
		private bool jumpToNextAction;

		private Queue<string> passageOrders = new Queue<string>();
		private Queue<StoryLink> passageLinks = new Queue<StoryLink>();

		private DialogManager dialogManager;

		public Transform player;
		public DialogBox dialogBox;
		public DialogOptionGroup dialogOptionGroup;

		private CharacterLine lastCharacterLine;

		//Just for debug tracking
		private int actionCount = 0;

		void Awake () {

			dialogManager = GetComponent<DialogManager>();
		}

		#region Before-execution preparations

		public void Clear () {
			passageOrders.Clear();
			passageLinks.Clear ();
			dialogBox.Clear();
			dialogOptionGroup.Clear();
		}

		public void EnqueueOrder (string orders) {
			passageOrders.Enqueue(orders);
		}

		public void EnqueueLink (StoryLink link) {
			passageLinks.Enqueue(link);
		}

		#endregion

		#region Execution functions
		public void ExecutePassage () {
			if (IsThereAnAction ()) {
				StartCoroutine ("ExecuteEntirePassage");
			}
		}

		private IEnumerator ExecuteEntirePassage () {

			if (PassageHasNoLinks()) {
				passageOrders.Enqueue ("EndDialog");
			}

			ignoreNextLinkAction = false;

			while (IsThereAnAction ()) {
				WaitForNextAction();
				ExecuteAction (passageOrders.Dequeue());
				while (!jumpToNextAction) {
					yield return null;
				}
			}
		}

		private void ExecuteAction (string orders) {
			TwineAction action = new TwineAction (orders);
			actionCount++;
			Debug.Log ("Action " + actionCount + " is " + orders);

			if (!action.parameters.IsBlank()) {
				gameObject.SendMessage (action.command, action.parameters);
			} else {
				Invoke (action.command, 0f);
			}
		}

		private void WaitForNextAction () {
			jumpToNextAction = false;
		}

		public void NextAction () {
			jumpToNextAction = true;
		}

		private void DisplayLinkOption (StoryLink outputLink) {
			dialogOptionGroup.PositionToCharacter(player);
			dialogOptionGroup.InsertOption (outputLink);
		}
		#endregion

		#region Passage-related checks
		private bool PassageHasOneLink () {
			if (passageLinks.Count == 1) {
				return (passageLinks.Peek().Text.IsBlank());
			}
			return false;
		}

		private bool PassageHasNoLinks () {
			return (passageLinks.Count == 0);
		}

		private bool IsThereAnAction () {
			return (passageOrders.Count > 0);
		}
		#endregion

		#region Twine Normal functions
		private void DisplayLine (string line) {

			int characterDivisorIndex = line.IndexOf(':');

			if (characterDivisorIndex > 0) {
				lastCharacterLine.character = line.Substring (0, characterDivisorIndex);
			}

			if (line.Length > (characterDivisorIndex + 2)) {
				lastCharacterLine.text = line.Substring (characterDivisorIndex + 1);
			} else {
				lastCharacterLine.text = "";
			}

			dialogBox.DisplayCharacterLine (lastCharacterLine);
			if (passageOrders.Count == 0) {
				dialogBox.InsertEndListener();
			} else {
				dialogBox.InsertPassListener();
			}
			WaitForNextAction();
		}

		private void DisplayNextLink () {
			StoryLink nextLink = passageLinks.Dequeue ();

			if (!ignoreNextLinkAction) {
				if (PassageHasOneLink()) {				
					DialogManager.instance.CurrentStory.DoLink(nextLink);
				} else {
					DisplayLinkOption (nextLink);
				}
			}

			ignoreNextLinkAction = false;
			NextAction();
		}
		#endregion

		#region Examples of Twine Special Functions
		/// Here there are some examples of actions we are using in our game.

		/*
		private void CheckQuest (string quest) {
			bool isQuestProposed = QuestManager.instance.IsQuestProposedByString (quest);
			ignoreNextLinkAction = !isQuestProposed;
			NextAction();
		}

		private void StartQuest (string quest) {
			int questID = 0;

			if (int.TryParse (quest, out questID)) {
				QuestManager.instance.AcceptQuest (questID);
			} else {
				QuestManager.instance.AcceptQuestByString (quest);
			}
			NextAction();
		}

		private void GetObject (string objectToGet) {
			NextAction();
		}

		private void EndDialog () {
			DialogManager.instance.EndDialog ();
		}

		private void AddToAlignment (string info) {
			string[] valueStrings = info.Split (',');
			if (valueStrings.Length < 3) {
				Debug.LogError ("ERROR in DialogoExecutioner: Alignment string has less than 3 values");
				return;
			}

			float dayValue = 0f;
			float synthValue = 0f;
			float nightValue = 0f;

			if (!float.TryParse (valueStrings[0], out dayValue)) {
				Debug.LogError ("ERROR in DialogoExecutioner: Alignment day value is not a number");
			}

			if (!float.TryParse (valueStrings[1], out synthValue)) {
				Debug.LogError ("ERROR in DialogoExecutioner: Alignment synth value is not a number");
			}

			if (!float.TryParse (valueStrings[2], out nightValue)) {
				Debug.LogError ("ERROR in DialogoExecutioner: Alignment night value is not a number");
			}

			DramaManager.instance.PlayerModelScript.UpdatePlayerAlignment (dayValue, synthValue, nightValue);
		}

		private void Action (string actionToExecute) {
		
		}*/
		#endregion

	}
}

