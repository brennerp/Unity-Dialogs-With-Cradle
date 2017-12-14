using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Written by Brenner Pacelli on November 7th, 2017
///Last updated on December 14th, 2017
///Generic Game base-script. It will be updated later to carry important variables and manage saves.

public class Game {
	
	public static bool paused {
		get;
		private set;
	}

	public delegate void GameHandler ();
	public static event GameHandler OnGamePaused;
	public static event GameHandler OnGamePlayed;

	public static void Pause () {
		paused = true;

		if (OnGamePaused != null)
			OnGamePaused();
	}

	public static void Play () {
		paused = false;

		if (OnGamePlayed != null)
			OnGamePlayed();
	}
}

