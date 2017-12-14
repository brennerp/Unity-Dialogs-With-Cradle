using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Written by Brenner Pacelli on November 20th, 2017
///TwineAction is a class to separate Twine strings into Commands and Parameters

namespace DialogSystem {
	public class TwineAction {

		public string command;
		public string parameters;

		public TwineAction () {
			command = "";
			parameters = "";
		}

		public TwineAction (string orders) {
			command = "";
			parameters = "";

			string [] twineActionInfo = orders.Split('/');

			if (twineActionInfo.Length >= 1) {
				command = twineActionInfo[0];
				if (twineActionInfo.Length >= 2) {
					parameters = twineActionInfo[1];
				}
			}
		}
	}
}

