using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cradle;
using Cradle.StoryFormats.Sugar;

///DialogManager receives the call for a dialog start with a Cradle story
///Then, it waits for each passage of the story, reads and sends each line as an action to the DialogExecutioner
///It also reads special actions inside brackets ('<' and '>')
///At the end of the passage, it asks the DialogExecutioner to execute the passage actions

namespace DialogSystem {

	public struct CharacterLine {
		public string text;
		public string character;
	}

	[RequireComponent(typeof(DialogExecutioner))]
	public class DialogManager : MonoBehaviour {

		public static DialogManager instance;

		public Story testStory;
		public bool startDialogAutomatically;


		private Story currentStory;
		public Story CurrentStory {
			get {
				return currentStory;
			}
		}

		private DialogExecutioner dialogExecutioner;

		void Awake () {
			if (instance == null) {
				instance = this;
				dialogExecutioner = GetComponent<DialogExecutioner>();
			} else {
				Destroy(this.gameObject);
			}
		}

		void Start () {
			Clear();
			if (startDialogAutomatically) {
				StartDialog (testStory);
			}
		}

		void OnEnable () {
			
		}

		void OnDisable () {
			SignOffStory();
		}

		void InvokeDialog () {
			StartDialog (testStory);
		}

		public void StartDialog (Story newStory) {

			EndDialog ();

			SignInStory (newStory);

			currentStory.Begin();
			Game.Pause();
		}

		public void EndDialog () {
			SignOffStory();
			Clear();
			Game.Play();
		}

		public void PassCharacterLine () {
			dialogExecutioner.NextAction();
		}

		#region Story Event-related functions
		private void SignInStory (Story targetStory) {
			SignOffStory();
			currentStory = targetStory;
			currentStory.OnPassageEnter += StoryOnPassageEnter;
			currentStory.OnOutput += StoryOnOutput;
			currentStory.OnPassageDone += StoryOnPassageDone;
		}

		private void SignOffStory () {
			if (currentStory != null) {
				currentStory.OnPassageEnter -= StoryOnPassageEnter;
				currentStory.OnOutput -= StoryOnOutput;
				currentStory.OnPassageDone -= StoryOnPassageDone;
				currentStory = null;
			}
		}

		private void StoryOnPassageEnter (StoryPassage passage) {
			Clear();
		}


		private void StoryOnOutput (StoryOutput output) {
			if (output is StoryLink) {
				EnqueueDialogLink(output);
			} else {
				EnqueueCharacterLine (output);
			}
		}

		private void Clear () {
			dialogExecutioner.Clear ();
		}

		private void StoryOnPassageDone (StoryPassage output)
		{
			DisplayPassage();
		}
		#endregion

		#region Executioner-related functions
		private void EnqueueCharacterLine (StoryOutput s) {
			string outputString = s.Text;

			if (outputString.IsBlank())
				return;
			
			int startOrderIndex = outputString.IndexOf('<');
			int finishOrderIndex = outputString.IndexOf('>') - 1;

			if (startOrderIndex >= 0) {
				if (finishOrderIndex >= 0) {
					int ordersLength = finishOrderIndex - startOrderIndex;
					string orders = outputString.Substring (startOrderIndex + 1, ordersLength);
					string [] separateOrders = orders.Split(';');
					foreach (string order in separateOrders) {
						dialogExecutioner.EnqueueOrder(order);
					}
				} else {
					Debug.LogError ("ERROR in DialogManager: TwineAction tag does not close on line " + s.Index + " of " + currentStory.CurrentPassage.Name);
				}
			} else {
				string displayLineOrder = "DisplayLine/" + outputString;
				dialogExecutioner.EnqueueOrder (displayLineOrder);
			}	
		}

		private void EnqueueDialogLink (StoryOutput s) {
			StoryLink link = (StoryLink) s;
			string order = "DisplayNextLink";
			dialogExecutioner.EnqueueOrder (order);
			dialogExecutioner.EnqueueLink (link);
		}

		public void DisplayPassage () {
			dialogExecutioner.ExecutePassage();
		}
		#endregion

		#region Static methods
		public static Transform GetCharacterTransform (string characterName) {
			GameObject go = GameObject.Find (characterName);

			if (go != null) {
				return go.transform;
			} else {
				return Camera.main.transform;
			}
		}
		#endregion

	}

}

