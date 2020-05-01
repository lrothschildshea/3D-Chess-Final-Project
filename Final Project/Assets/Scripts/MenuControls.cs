using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour {

	
	private GameObject welcomeScreen;
	private GameObject instructionsScreen;
	public GameObject hud;
	public GameObject upgradeScreen;
	private GameObject gameOverScreen;
	private GameObject pauseScreen;
    private GameObject forfeitConfirmationScreen;
    private GameObject drawConfirmationScreen;

	private GameObject statsScreen;

	private TestScript mainScript;

	void Start () {
		welcomeScreen = GameObject.Find("WelcomeScreen");
		instructionsScreen = GameObject.Find("InstructionsScreen");
		hud = GameObject.Find("HUD");
		upgradeScreen = GameObject.Find("UpgradeScreen");
		gameOverScreen = GameObject.Find("GameOverScreen");
		pauseScreen = GameObject.Find("PauseScreen");
        forfeitConfirmationScreen = GameObject.Find("ForfeitConfirmationScreen");
        drawConfirmationScreen = GameObject.Find("DrawConfirmationScreen");
		statsScreen = GameObject.Find("StatsScreen");
		mainScript = GameObject.Find("GameBoard").GetComponent<TestScript>();
		disableAllExcept(welcomeScreen);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape) && !welcomeScreen.activeSelf && !instructionsScreen.activeSelf && !gameOverScreen.activeSelf && !upgradeScreen.activeSelf && !pauseScreen.activeSelf && !forfeitConfirmationScreen.activeSelf && !statsScreen.activeSelf){
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
        if (exception != forfeitConfirmationScreen){
            disableForfeitConfirmationScreen();
        }
        if(exception != drawConfirmationScreen){
            disableDrawConfirmationScreen();
        }
		if(exception != statsScreen){
            disableStatsScreen();
        }
	}

	public void startGame(){
		enableHUD();
		mainScript.gameStarted = true;
        if (mainScript.singlePlayer){
            GameObject.Find("DrawButton").SetActive(false);
        }

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
        mainScript.gameOver = true;
        disableAllExcept(gameOverScreen);
	}

	public void disableGameOverScreen(){
		gameOverScreen.SetActive(false);
	}

	public void enablePauseScreen(){
		pauseScreen.SetActive(true);
		disableAllExcept(pauseScreen);
		mainScript.paused = true;
	}

	public void disablePauseScreen(){
		pauseScreen.SetActive(false);
		mainScript.paused = false;
	}

    public void singePlayerStart(){
        mainScript.singlePlayer = true;
        startGame();
    }
    
    public void twoPlayerStart(){
        mainScript.singlePlayer = false;
        startGame();
    }

    public void enableForfeitConfirmationScreen(){
        forfeitConfirmationScreen.SetActive(true);
        disableAllExcept(forfeitConfirmationScreen);
        mainScript.paused = true;
    }

    public void disableForfeitConfirmationScreen(){
        forfeitConfirmationScreen.SetActive(false);
        mainScript.paused = false;
    }

    public void enableDrawConfirmationScreen(){
        drawConfirmationScreen.SetActive(true);
        disableAllExcept(drawConfirmationScreen);
        mainScript.paused = true;
    }

    public void disableDrawConfirmationScreen(){
        drawConfirmationScreen.SetActive(false);
        mainScript.paused = false;
    }

	public void enableStatsScreen(){
        statsScreen.SetActive(true);
		GameObject.Find("StatsScreen").GetComponent<StatsLogic>().read = false;
        disableAllExcept(statsScreen);
    }

    public void disableStatsScreen(){
        statsScreen.SetActive(false);
    }

    public void returnToMainMenu(){
        SceneManager.LoadScene("Board");
    }

	public void acceptForfeit(){
		mainScript.forfeit = true;
		mainScript.lightWon = !mainScript.lightsTurn;
		enableGameOverScreen();
	}

	public void acceptDraw(){
		mainScript.draw = true;
		enableGameOverScreen();
	}
}
