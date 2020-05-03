using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour {

	
	private GameObject welcomeScreen;
	private GameObject instructionsScreen;
	internal GameObject hud;
	internal GameObject upgradeScreen;
	private GameObject gameOverScreen;
	private GameObject pauseScreen;
    private GameObject forfeitConfirmationScreen;
    private GameObject drawConfirmationScreen;

	private GameObject statsScreen;
	private GameObject controlsScreen;

	private GameLogic mainScript;

	public Text instructionsText;

	private string[] instructions;

	private int instructionsIndex;

	private GameObject nextButton;
	private GameObject prevButton;
	
	

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
		controlsScreen = GameObject.Find("ControlsScreen");
		mainScript = GameObject.Find("GameBoard").GetComponent<GameLogic>();

		nextButton = GameObject.Find("NextButton");
		prevButton = GameObject.Find("PrevButton");
		prevButton.SetActive(false);

		instructionsIndex = 0;
		instructions = new String[3];
		createInstructions();
		instructionsText.text = instructions[0];

		disableAllExcept(welcomeScreen);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape) && !welcomeScreen.activeSelf && !instructionsScreen.activeSelf && !gameOverScreen.activeSelf && !upgradeScreen.activeSelf && !pauseScreen.activeSelf && !forfeitConfirmationScreen.activeSelf && !statsScreen.activeSelf && !controlsScreen.activeSelf){
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
		if(exception != controlsScreen){
            disableControlsScreen();
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

	public void enableControlsScreen(){
		controlsScreen.SetActive(true);
        disableAllExcept(controlsScreen);
	}

	public void disableControlsScreen(){
		controlsScreen.SetActive(false);
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

	public void quitGame() {
    	 Application.Quit();
 	}

	public void instructionsNext(){
		instructionsIndex++;
		instructionsText.text = instructions[instructionsIndex];
		if(instructionsIndex == 2){
			nextButton.SetActive(false);
		}
		prevButton.SetActive(true);
	}

	public void instructionsPrev(){
		instructionsIndex--;
		instructionsText.text = instructions[instructionsIndex];
		if(instructionsIndex == 0){
			prevButton.SetActive(false);
		}
		nextButton.SetActive(true);
	}

	private void createInstructions(){

		instructions[0] = "Introduction\n\n" + 
		"Three-dimensional (3D) chess is a game played on a similar board with similar pieces to standard chess, but is played with a different set of rules that allow the players to take advantage of the multi leveled board.\n\n" + 
		"1.1: Two players take turns moving pieces or stands around the game board taking actions that are seen as legal according to the rules of the game, alternating turns once a player has concluded moving their piece or a stand. The player who is using the yellow pieces starts the game.\n\n" + 
		"1.2: The game board consists of three static main 4x4 “levels”, four 2x2 “stands” that can be moved around the board, and 12 “pegs” that indicate where a stand can be positioned.\n\n" + 
		"1.3: Each team starts with 1 King, 1 Queen, 2 Bishops, 2 Knights, 2 Rooks, and 8 Pawns setup in a predefined formation.\n\n" + 
		"1.4: Each player starts with a 30 minute clock. At the start of a player's turn, their clock begins to tick down and does not stop until the conclusion of that player’s turn.\n\n" + 
		"1.5: There are two game modes to select from in the main menu. “1 Player” starts a game of 3D chess where a Human player is pitted against an A.I. player where the Human player controls the yellow team and the A.I. player controls the blue team. “2 Player” starts a game of 3D Chess where two Human players are pitted against each other by alternating turns.\n\n" + 
		"1.6: The objective of the game is to place the opponent’s king in a position such that no legal action the opponent takes can prevent the “capture” of the opponent's king on the following move. The player who achieves this is said to have “checkmated” their opponent and won the game.";

		instructions[1] = "Movement of Pieces and Stands\n\n" + 
		"2.1: Any move that can be made with a piece from a standard game of chess can be made in 3D chess. Every overlapping square has the same color, which means the player has the option of which level to place their piece on given the squares are overlapping. For this reason, two pieces (of same or different teams) can occupy the same square on different levels.\n\n" + 
		"2.2: No piece can be moved to a square on the same level that is occupied by a piece of the same color. If a piece is moved to the same square and level as an opponent’s piece, the latter is said to be “captured” and is removed from the gameboard and the former takes its position on the gameboard.\n\n" + 
		"2.3: A piece, on any individual square, blocks the ability of pieces to move on all levels. The moving piece may be moved above or below the occupied square and can continue its move on the next turn.\n\n" + 
		"2.4: Vertical moves without horizontal movement are forbidden.\n\n" + 
		"2.5: When a pawn is at the edge furthest from its starting position (the opponent’s edge of the board) at the end of a turn, it must be exchanged for a bishop, a rook, a knight, or a queen as part of the same move. What is determined as the opponent’s edge changes depending on the state of the board. A square is said to be on the opponent’s edge of the board during a turn, if all pieces were removed from the board and if a player placed a pawn to that square and that pawn would have no legal actions on the player’s next turn. The player’s choice is not restricted to pieces that have been captured previously. This exchange of a pawn for another piece is referred to as a “promotion” and the effect of the new piece takes place immediately.\n\n" + 
		"2.6: The king can be moved in two different ways. Normally or by “castling”. Castling is the move of the king and either rook of the same color, counting as a single move of the king. To castle, the king swaps places with the selected rook. Castling is illegal as a player’s first move of the game, if the king has already been moved, or with a rook that has already been moved. If there is a piece between the king and the rook then castiling is prohibited.\n\n" + 
		"2.7: The king is said to be in “check”, if it can be captured by one or more of the opponent’s pieces on the opponent’s next turn. A player cannot end their turn in check.\n\n" + 
		"2.8: A stand can only be moved, if it holds no more than one piece (regardless of which piece that is). Stands are controlled by the player whose piece is located on it. If a stand is empty, either player can move the stand. Occupied stands can move forward to any unoccupied peg or to the side across the same level to an unoccupied peg. An empty board can also be moved backwards towards the player who is moving them, to any unoccupied peg.\n\n";

		instructions[2] = "The Completed Game\n\n" + 
		"3.1: The game is won by the player who has checkmated their opponent’s king with a legal move. This immediately ends the game.\n\n" + 
		"3.2: The game is won by the player whose opponent declares they forfeit. This immediately ends the game.\n\n" + 
		"3.3: The game is won by the player whose opponent runs out of time on their turn. This immediately ends the game.\n\n" + 
		"3.3: The game is drawn when the player to move has no legal actions and their king is not in check. The game is said to end in “stalemate”. This immediately ends the game.\n\n" + 
		"3.4: The game is drawn upon agreement between the two players during the game. This immediately ends the game.\n\n" + 
		"3.5: The game will be drawn if the last 50 consecutive moves have been made by each player without the movement of any pawn without the capture of any piece.\n\n";
	}
}
