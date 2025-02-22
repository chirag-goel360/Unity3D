﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	public float turnDelay = .1f;
	public static GameManager instance = null;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;

	public BoardManager boardScript;

	private Text levelText;
	private GameObject levelImage;
	// Enemies appear at lvl 3
	private int level = 0;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private bool doingSetup;

	// Use this for initialization
	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
		enemies = new List<Enemy> ();
		boardScript = GetComponent<BoardManager> ();
		//InitGame ();
	}

	void InitGame () {
		doingSetup = true;

		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		levelText.text = "Day " + level;
		levelImage.SetActive(true);

		//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
		Invoke("HideLevelImage", levelStartDelay);

		// Remove Enemies from level
		enemies.Clear ();
		boardScript.SetupScene (level);
	}

	public void GameOver() {
		levelText.text = "You starved after " + level + " days.";
		//Enable black background image gameObject.
		levelImage.SetActive(true);
		enabled = false;
	}

	// Update is called once per frame
	void Update () {
		if (playersTurn || enemiesMoving || doingSetup) {
			return;
		}
		StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList(Enemy script) {
		enemies.Add (script);
	}

	//Hides black image used between levels
	void HideLevelImage() {
		//Disable the levelImage gameObject.
		levelImage.SetActive(false);

		//Set doingSetup to false allowing player to move again.
		doingSetup = false;
	}

	//This is called each time a scene is loaded.
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
		//Add one to our level number.
		level++;
		//Call InitGame to initialize our level.
		InitGame(); 
	}

	void OnEnable() {
		//Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
		SceneManager.sceneLoaded += OnLevelFinishedLoading; 
	}

	void OnDisable() {
		//Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
		//Remember to always have an unsubscription for every delegate you subscribe to!
		SceneManager.sceneLoaded -= OnLevelFinishedLoading; 
	}


	IEnumerator MoveEnemies() {
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++) {
			enemies [i].MoveEnemy ();
			yield return new WaitForSeconds (enemies [i].moveTime);
		}

		playersTurn = true;
		enemiesMoving = false;
	}
}
