﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public GameObject playerPrefab;
	public Text continueText;
	public Text scoreText;

	private float timeElapsed = 0f;
	private float bestTime = 0f;
	private float blinkTime = 0f;
	private bool blink;
	private bool gameStarted;
	private TimeManager timeManager;
    public static GameManager instance = null;
    private GameObject player;
	private GameObject floor;
	private Spawner spawner;
	private bool beatBestTime;
    private bool shouldAwake;

	void Awake(){

        floor = GameObject.Find ("Foreground");
		spawner = GameObject.Find ("Spawner").GetComponent<Spawner> ();
        timeManager = GetComponent<TimeManager> ();
	}

	// Use this for initialization
	void Start () {

		var floorHeight = floor.transform.localScale.y;

		var pos = floor.transform.position;
		pos.x = 0;
		pos.y = -((Screen.height / PixelPerfectCamera.pixelsToUnits) / 2) + (floorHeight / 2);
		floor.transform.position = pos;

		spawner.active = false;

		Time.timeScale = 0;

		continueText.text = "KNOPF KLICKEN UM ZU STARTEN";

		bestTime = PlayerPrefs.GetFloat ("BestTime");
	}
	
	// Update is called once per frame
	void Update () {
		if (!gameStarted && Time.timeScale == 0) {

			if(Input.anyKeyDown){

				timeManager.ManipulateTime(1, 1f);
				ResetGame();
			}
		}

		if (!gameStarted) {
			blinkTime ++;

			if (blinkTime % 40 == 0) {
				blink = !blink;
			}

			continueText.canvasRenderer.SetAlpha (blink ? 0 : 1);

			var textColor = beatBestTime ? "#FF0" : "#FFF";

			scoreText.text = "ZEIT: " + FormatTime (timeElapsed) + "\n<color="+textColor+">BEST: " + FormatTime (bestTime)+"</color>";
		} else {
			timeElapsed += Time.deltaTime;
			scoreText.text = "ZEIT: "+FormatTime(timeElapsed);
		}
	}

	void OnPlayerKilled(){
        SceneManager.LoadScene(5);
    }

	void ResetGame(){
		spawner.active = true;

		player = GameObjectUtil.Instantiate(playerPrefab, new Vector3(0, (Screen.height/PixelPerfectCamera.pixelsToUnits)/2 + 100, 0));

		var playerDestroyScript = player.GetComponent<DestroyOffscreen> ();
		playerDestroyScript.DestroyCallback += OnPlayerKilled;

		gameStarted = true;

		continueText.canvasRenderer.SetAlpha(0);

		timeElapsed = 0;
		beatBestTime = false;
	}

	string FormatTime(float value){
		TimeSpan t = TimeSpan.FromSeconds (value);

		return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
	}

}
