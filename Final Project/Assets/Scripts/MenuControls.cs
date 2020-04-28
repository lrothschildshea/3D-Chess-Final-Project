using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControls : MonoBehaviour {

	
	private GameObject welcomeScreen;
	private GameObject instructionsScreen;
	public GameObject hud;
	public GameObject upgradeScreen;
	private GameObject gameOverScreen;
	private GameObject pauseScreen;

	void Start () {
		welcomeScreen = GameObject.Find("WelcomeScreen");
		instructionsScreen = GameObject.Find("InstructionsScreen");
		hud = GameObject.Find("HUD");
		upgradeScreen = GameObject.Find("UpgradeScreen");
		gameOverScreen = GameObject.Find("GameOverScreen");
		pauseScreen = GameObject.Find("PauseScreen");
		disableAllExcept(welcomeScreen);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape) && !welcomeScreen.activeSelf && !instructionsScreen.activeSelf && !gameOverScreen.activeSelf && !upgradeScreen.activeSelf && !pauseScreen.activeSelf){
			enablePauseScreen();
		} else if(Input.GetKeyDown(KeyCode.Escape) && pauseScreen.activeSelf){
			enableHUD();
		}
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
		if(exception != upgradeScreen){
			disableUpgradeScreen();
		}
		if(exception != gameOverScreen){
			disableGameOverScreen();
		}
		if(exception != pauseScreen){
			disablePauseScreen();
		}
	}

	public void startGame(){
		enableHUD();
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

	public void enableUpgradeScreen(){
		upgradeScreen.SetActive(true);
		disableAllExcept(upgradeScreen);
	}

	public void disableUpgradeScreen(){
		upgradeScreen.SetActive(false);
	}

	public void enableGameOverScreen(){
		gameOverScreen.SetActive(true);
		disableAllExcept(gameOverScreen);
	}

	public void disableGameOverScreen(){
		gameOverScreen.SetActive(false);
	}

	public void enablePauseScreen(){
		pauseScreen.SetActive(true);
		disableAllExcept(pauseScreen);
		GameObject.Find("GameBoard").GetComponent<TestScript>().paused = true;
	}

	public void disablePauseScreen(){
		pauseScreen.SetActive(false);
		GameObject.Find("GameBoard").GetComponent<TestScript>().paused = false;
	}

    public void singePlayerStart(){
        GameObject.Find("GameBoard").GetComponent<TestScript>().singlePlayer = true;
        startGame();
    }
    
    public void twoPlayerStart(){
        GameObject.Find("GameBoard").GetComponent<TestScript>().singlePlayer = false;
        startGame();
    }
}
