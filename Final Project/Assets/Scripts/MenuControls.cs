using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControls : MonoBehaviour {

	
	private GameObject welcomeScreen;
	private GameObject instructionsScreen;
	private GameObject hud;
	private GameObject gameOverScreen;

	void Start () {
		welcomeScreen = GameObject.Find("WelcomeScreen");
		instructionsScreen = GameObject.Find("InstructionsScreen");
		hud = GameObject.Find("HUD");
		gameOverScreen = GameObject.Find("GameOverScreen");

		disableAllExcept(welcomeScreen);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void disableAllExcept(GameObject exception){
		if(exception != welcomeScreen){
			disableWelcomeScreen();
		}
		if(exception != instructionsScreen){
			disableInstructionScreen();
		}
		if(exception != hud){
			disableHUD();
		}
		if(exception != gameOverScreen){
			disableGameOverScreen();
		}
	}

	public void startGame(){
		enableHUD();
		disableAllExcept(hud);
		GameObject.Find("GameBoard").GetComponent<TestScript>().gameStarted = true;
	}

	public void enableWelcomeScreen(){
		welcomeScreen.SetActive(true);
		disableAllExcept(welcomeScreen);
	}
	public void disableWelcomeScreen(){
		welcomeScreen.SetActive(false);
	}

	public void enableInstructionsScreen(){
		instructionsScreen.SetActive(true);
		disableAllExcept(instructionsScreen);
	}
	public void disableInstructionScreen(){
		instructionsScreen.SetActive(false);
	}

	public void enableHUD(){
		hud.SetActive(true);
		disableAllExcept(hud);
	}
	public void disableHUD(){
		hud.SetActive(false);
	}

	public void enableGameOverScreen(){
		gameOverScreen.SetActive(true);
		disableAllExcept(gameOverScreen);
	}

	public void disableGameOverScreen(){
		gameOverScreen.SetActive(false);
	}
}
