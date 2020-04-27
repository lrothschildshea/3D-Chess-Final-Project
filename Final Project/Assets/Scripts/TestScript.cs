using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TestScript : MonoBehaviour {

	private bool enableMovesets = true;
	private GameObject selectedPiece = null;
	private GameObject selectedTile = null;
	private GameObject selectedPlatform = null;
	private GameObject selectedPlatformLoc = null;
	private List<GameObject> availablePegs = null;
	private bool moving = false;
	private bool standPreppedForMove = false;

	private Color lightTeamColor;
	private Color darkTeamColor;
	private Color lightTileColor;
    private Color darkTileColor;
	private Color standColor;
	private Color pegColor;

    private GameObject gameBoard;
    private List<GameObject> lightPieces;
    private List<GameObject> darkPieces;
    private List<GameObject> tiles;
    private List<GameObject> pegs;
    private List<GameObject> stands;
    private List<GameObject> levels;

	private int lightCapturedCounter;
	private float[,] lightCapturedPiecesLocations;
	private int darkCapturedCounter;
	private float[,] darkCapturedPiecesLocations;
	private float capturedY;
    private List<GameObject> availableMoves;

	private GameObject capturedPiece;

	private bool lightsTurn;

    private List<GameObject> stationaryPawns;

	private float timeSinceTextChange = 0f;
	GameObject bottomPrompt;

	bool gameOver;
	public bool gameStarted;

	private bool upgrading;
	public String upgradeSelection;
	private List<GameObject> lightUpgradeTiles;
	private List<GameObject> darkUpgradeTiles;

	//set in unity
	public AudioClip checkRedAlert;
	public GameObject lightKnightPrefab;
	public GameObject lightRookPrefab;
	public GameObject lightBishopPrefab;
	public GameObject lightQueenPrefab;
	public GameObject darkKnightPrefab;
	public GameObject darkRookPrefab;
	public GameObject darkBishopPrefab;
	public GameObject darkQueenPrefab;

	private List<GameObject> piecesToUpgrade;
	private bool foundAllUpgradePieces;
	private GameObject lightOptions;
	private GameObject darkOptions;

    private int movesWithOutPawnOrCapture;

    private float lightTimer;
    private float darkTimer;
    private bool canCount;
    private bool doOnce;

	private bool intermediatesPrepped;
	private Vector3 t1; //move left or right
	private Vector3 t2; //go up/down
	private Vector3 t3;// slide along z axis
	private Vector3 t4;// go to right or left to peg

	private bool finishedT1;
	private bool finishedT2;
	private bool finishedT3;

	private bool changeStandY;
	private bool hasSetChangeStandY;


	private MenuControls menuScript;

	// Use this for initialization
	void Start () {
		gameStarted = false;
		gameOver = false;
		lightsTurn = true;
		upgrading = false;
        lightPieces = new List<GameObject>();
        darkPieces = new List<GameObject>();
        tiles = new List<GameObject>();
        pegs = new List<GameObject>();
        stands = new List<GameObject>();
        availableMoves = new List<GameObject>();
        stationaryPawns = new List<GameObject>();
        levels = new List<GameObject>();
		bottomPrompt = GameObject.Find("Bottom Prompt");
        lightTimer = 1800f;
        darkTimer = 1800f;
        canCount = true;
        doOnce = false;
		menuScript = GameObject.Find("Menus").GetComponent<MenuControls>();
		piecesToUpgrade = new List<GameObject>();
		foundAllUpgradePieces = false;
		upgradeSelection = null;
		lightOptions = GameObject.Find("lightOptions");
		darkOptions = GameObject.Find("darkOptions");
		intermediatesPrepped = false;
		t1 = new Vector3(1000f, 1000f, 1000f);
		t2 = new Vector3(1000f, 1000f, 1000f);
		t3 = new Vector3(1000f, 1000f, 1000f);
		t4 = new Vector3(1000f, 1000f, 1000f);
		finishedT1 = false;
		finishedT2 = false;
		finishedT3 = false;
		changeStandY = false;
		hasSetChangeStandY = false;
        //get lists of objects used throughout the game
        gameBoard = GameObject.Find("GameBoard");
        foreach(Transform group in gameBoard.transform){
            if (group.gameObject.name.Contains("Level"))
            {
                levels.Add(group.gameObject);
            }
            foreach(Transform piece in group){
                GameObject gamePiece = piece.gameObject;
                if(gamePiece.name.Contains("Light") && !gamePiece.name.Contains("Captured")){
                    lightPieces.Add(gamePiece);
                }
                if (gamePiece.name.Contains("Dark") && !gamePiece.name.Contains("Captured")){
                    darkPieces.Add(gamePiece);
                }
                if (gamePiece.name.Contains("Tile")){
                    tiles.Add(gamePiece);
                }
                if (gamePiece.name.Contains("Peg")){
                    pegs.Add(gamePiece);
                }
                if (gamePiece.name.Contains("Stand")){
                    stands.Add(gamePiece);
                }
                if (gamePiece.name.Contains("Pawn"))
                {
                    stationaryPawns.Add(gamePiece);
                }
            }
        }

		lightCapturedCounter = 0;
		darkCapturedCounter = 0;
		lightCapturedPiecesLocations = new float[16,2];
		darkCapturedPiecesLocations = new float[16,2];

		Vector3 capPos = GameObject.Find("Captured Dark").transform.position;

		capturedY = capPos.y + .05f;

		for(int i = 0; i < 2; i++){
			for(int j = 0; j < 8; j++){
				if(i == 0){
					darkCapturedPiecesLocations[8*i + j, 0] = capPos.x + .5f;
				} else {
					darkCapturedPiecesLocations[8*i + j, 0] = capPos.x - .5f;
				}
				darkCapturedPiecesLocations[8*i + j, 1] = capPos.z - 3.5f + j;
			}
		}

		
		capPos = GameObject.Find("Captured Light").transform.position;

		for(int i = 0; i < 2; i++){
			for(int j = 0; j < 8; j++){
				if(i == 0){
					lightCapturedPiecesLocations[8*i + j, 0] = capPos.x - .5f;
				} else {
					lightCapturedPiecesLocations[8*i + j, 0] = capPos.x + .5f;
				}
				lightCapturedPiecesLocations[8*i + j, 1] = capPos.z - 3.5f + j;
			}
		}

		lightTeamColor = lightPieces[0].GetComponent<Renderer>().material.color;
		darkTeamColor = darkPieces[0].GetComponent<Renderer>().material.color;
        lightTileColor = tiles[1].GetComponent<Renderer>().material.color;
        darkTileColor = tiles[0].GetComponent<Renderer>().material.color;
		standColor = stands[0].transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color;
		pegColor = pegs[0].GetComponent<Renderer>().material.color;
    }

    bool isDarkPiece(GameObject gamePiece){
        return darkPieces.Contains(gamePiece);
    }

	bool isLightPiece(GameObject gamePiece){
        return lightPieces.Contains(gamePiece);
    }

	bool isTile(GameObject gamePiece){
        return tiles.Contains(gamePiece);
    }

	bool isStand(GameObject gamePiece){
		bool isRod = gamePiece.name.Contains("Rod");
		bool isPlatform = gamePiece.name.Contains("Platform");
		bool isStand = stands.Contains(gamePiece);
		return (isRod || isPlatform || isStand);
	}

	bool isPeg(GameObject gamePiece){
        return pegs.Contains(gamePiece);
    }

	bool isLightTile(GameObject tile){
        if(tile.name.Contains("0 0")|| tile.name.Contains("1 1") || tile.name.Contains("2 0") || tile.name.Contains("3 1") || tile.name.Contains("0 2") || tile.name.Contains("1 3") || tile.name.Contains("2 2") || tile.name.Contains("3 3")){
            return false;
        }
        return true;
    }

	void resetForNextAction(){
		setBottomPrompt("");
		resetForNextActionWithoutTogglingTurn();
        movesWithOutPawnOrCapture++;
		lightsTurn = !lightsTurn;

		List<GameObject> friendlyPieces;
		List<GameObject> enemyPieces;
		GameObject myKing;
		if(lightsTurn){
			friendlyPieces = lightPieces;
			enemyPieces = darkPieces;
			myKing = GameObject.Find("KingLight");
			
		} else {
			friendlyPieces = darkPieces;
			enemyPieces = lightPieces;
			myKing = GameObject.Find("KingDark");
		}

        if (movesWithOutPawnOrCapture > 50){
            setBottomPrompt("The last 50 consecutive moves have taken place without the movement of any pawn and without the capture of any piece! Game over.");
            gameOver = true;
        }

        bool legalMovePresent = false;
        List<GameObject[]> actions = getAllLegalActions(friendlyPieces);
        if (actions.Count > 0){
            legalMovePresent = true;
        }

        bool checkState = check(myKing, enemyPieces);
        if (!legalMovePresent && checkState){
            setBottomPrompt("You are in checkmate! Game over.");
            gameOver = true;
            
        }
        else if (!legalMovePresent && !checkState){
            setBottomPrompt("You have enetered a stalemate! Game over.");
            gameOver = true;
        }
        else if(legalMovePresent && checkState){
            setBottomPrompt("You are in check!");
        }

        if (checkState){
            GameObject.Find("GameBoard").GetComponent<AudioSource>().PlayOneShot(checkRedAlert, .2F);
        }
	}

	void resetForNextActionWithoutTogglingTurn(){
		selectedPiece = null;
		selectedTile = null;
		selectedPlatform = null;
		selectedPlatformLoc = null;
		capturedPiece = null;
		moving = false;
        availableMoves = null;
		availablePegs = null;
		standPreppedForMove = false;
		foundAllUpgradePieces = false;
		upgrading = false;
		reColorPegs();
		reColorTiles(tiles);
        availableMoves = new List<GameObject>();
        availablePegs = new List<GameObject>();
		t1 = new Vector3(1000f, 1000f, 1000f);
		t2 = new Vector3(1000f, 1000f, 1000f);
		t3 = new Vector3(1000f, 1000f, 1000f);
		t4 = new Vector3(1000f, 1000f, 1000f);
		hasSetChangeStandY = false;
		changeStandY = false;
		intermediatesPrepped = false;
		finishedT1 = false;
		finishedT2 = false;
		finishedT3 = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!gameStarted){
			return;
		}
		if(gameOver){
			GameObject.Find("Menus").GetComponent<MenuControls>().enableGameOverScreen();
		}

        if(lightsTurn){
            if (lightTimer >= 0.0f && canCount) {
                lightTimer -= Time.deltaTime;

            }
            else if (lightTimer <= 0.0f && !doOnce){
                canCount = false;
                doOnce = true;
                lightTimer = 0.0f;
                setBottomPrompt("You have used more than the alloted time for your turn! Game over.");
                gameOver = true;
            }
        }
        else{
            if(darkTimer >= 0.0f && canCount){
                darkTimer -= Time.deltaTime;
            }
            else if(darkTimer <= 0.0f && !doOnce){
                canCount = false;
                doOnce = true;
                darkTimer = 0.0f;
                setBottomPrompt("You have used more than the alloted time for your turn! Game over.");
                gameOver = true;
            }
        }

		updateBottomPromt();
        updateTimers();

		//detect the object that has been clicked and change its color
		if(Input.GetMouseButtonDown(0) && !moving) {
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, 100)){
				GameObject clicked = hit.transform.gameObject;

				//deselection
				if(clicked == selectedPiece){
					if(isLightPiece(selectedPiece)){
						selectedPiece.GetComponent<Renderer>().material.color = lightTeamColor;
					} else {
						selectedPiece.GetComponent<Renderer>().material.color = darkTeamColor;
					}
					resetForNextActionWithoutTogglingTurn();
					return;
				} else if(clicked.transform.parent.gameObject.transform.parent.gameObject == selectedPlatform){
					selectedPlatform.transform.GetChild(4).transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = standColor;
					selectedPlatform.transform.GetChild(4).transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = standColor;
					resetForNextActionWithoutTogglingTurn();
					return;
				}

				bool myPiece = (lightsTurn && isLightPiece(clicked)) || (!lightsTurn && isDarkPiece(clicked));

				//click on piece
                if (myPiece && selectedPiece == null && selectedPlatform == null && !isCaptured(clicked)){
					selectedPiece = clicked;
					clicked.GetComponent<Renderer>().material.color = Color.yellow;
                    List<GameObject[]> safeMoves = getSafeMoves(selectedPiece, getAvailableMoves(selectedPiece));
                    availableMoves = new List<GameObject>();
                    foreach(GameObject[] pair in safeMoves){
                        availableMoves.Add(pair[1]);
                    }
                    colorAvailableTiles(availableMoves);
				}

				//click on destination tile
				if(selectedPiece != null && isTile(clicked) && availableMoves.Contains(clicked)){
                    int available = tileAvailable(clicked, selectedPiece);
					if(available > 0){
						reColorTiles(availableMoves);
                        if (selectedPiece.name.Contains("Pawn"))
                        {
                            stationaryPawns.Remove(selectedPiece);
                        }
						selectedTile = clicked;
						clicked.GetComponent<Renderer>().material.color = Color.yellow;
						moving = true;
						if(available > 1){
							capturedPiece = getPieceOnTile(clicked);
							if(isLightPiece(capturedPiece)){
								lightCapturedCounter++;
							} else {
								darkCapturedCounter++;
							}
						}
					}

				}

				//click on stand
				if(isStand(clicked) && selectedPlatform == null && selectedPiece == null){
					if(canMoveStand(clicked.transform.parent.gameObject.transform.parent.gameObject) < 2){
						GameObject parent = clicked.transform.parent.gameObject;
						selectedPlatform = parent.transform.parent.gameObject;
						parent.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Color.yellow;
						parent.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                        availablePegs = new List<GameObject>();
                        List<GameObject[]> pairs = getLegalPegs(parent, getAvailablePegs(parent));
						foreach(GameObject[] pair in pairs){
                            availablePegs.Add(pair[1]);
							pair[1].GetComponent<Renderer>().material.color = Color.red; 
						}
					} else {
						setBottomPrompt("That stand cannot be moved right now.");
					}

				}

				//click on destination peg
				if(selectedPlatform != null && isPeg(clicked) && availablePegs.Contains(clicked)){
					selectedPlatformLoc = clicked;
					reColorPegs();
					clicked.GetComponent<Renderer>().material.color = Color.yellow;
					moving = true;
				}

			}
     	}

		if(moving & !upgrading){
			//moving piece
			if(selectedPiece != null){
                if (selectedPiece.name.Contains("Pawn")){
                    movesWithOutPawnOrCapture = -1;
                }
				Vector3 location = selectedTile.transform.position;
				location.y += .05f;
				selectedPiece.transform.position = Vector3.Lerp(selectedPiece.transform.position, location, Time.deltaTime * 3.5f);

				Vector3 dest = new Vector3(100000, 100000, 100000);

				if(capturedPiece != null){
                    movesWithOutPawnOrCapture = -1;
                    if (isLightPiece(capturedPiece)){
						dest = new Vector3(lightCapturedPiecesLocations[lightCapturedCounter-1, 0], capturedY, lightCapturedPiecesLocations[lightCapturedCounter-1, 1]);
						capturedPiece.transform.position = Vector3.Lerp(capturedPiece.transform.position, dest, Time.deltaTime * 3.5f);
					} else {
						dest = new Vector3(darkCapturedPiecesLocations[darkCapturedCounter-1, 0], capturedY, darkCapturedPiecesLocations[darkCapturedCounter-1, 1]);
						capturedPiece.transform.position = Vector3.Lerp(capturedPiece.transform.position, dest, Time.deltaTime * 3.5f);
					}
				}

				bool selectedClose = ((selectedPiece.transform.position - location).magnitude < .02);
				bool noCapPiece = capturedPiece == null;
				bool capPieceClose = capturedPiece != null && ((capturedPiece.transform.position - dest).magnitude < .02);

				if(selectedClose && (noCapPiece || capPieceClose)){
					selectedPiece.transform.position = location;
					if(isLightPiece(selectedPiece)){
						selectedPiece.GetComponent<Renderer>().material.color = lightTeamColor;
					} else {
						selectedPiece.GetComponent<Renderer>().material.color = darkTeamColor;
					}

					if(isLightTile(selectedTile)){
						selectedTile.GetComponent<Renderer>().material.color = lightTileColor;
					} else {
						selectedTile.GetComponent<Renderer>().material.color = darkTileColor;
					}
					

					//need to add a check above not hereish
					if(capturedPiece != null){
						capturedPiece.transform.position = dest;
						if(isLightPiece(capturedPiece)){
							lightPieces.Remove(capturedPiece);
						} else {
							darkPieces.Remove(capturedPiece);
						}
					}
					upgrading = true;
				}
			}

			//moving platform
			if(selectedPlatform != null){

				if(!standPreppedForMove){
					//assign piece on platform to platform for movement
					for(int i = 0; i < 4; i++){
						GameObject piece = getPieceOnTile(selectedPlatform.transform.GetChild(i).gameObject);
						if(piece != null){
							piece.transform.parent = selectedPlatform.transform;
						}
					}
					standPreppedForMove = true;
				}

				//adjust height
				Vector3 location = selectedPlatformLoc.transform.position;
				location.x -= .5f;
				location.y += (2.2f - .051f);
				location.z -= .5f;


				if(!hasSetChangeStandY){
					changeStandY = Math.Abs(location.y - selectedPlatform.transform.position.y) > .2f;
					hasSetChangeStandY = true;
				}

				if(changeStandY){
					float leftDist = (selectedPlatform.transform.position - GameObject.Find("Captured Dark").transform.position).magnitude;
					float rightDist = (selectedPlatform.transform.position - GameObject.Find("Captured Light").transform.position).magnitude;

					bool onLeft = leftDist < rightDist;

					if(!intermediatesPrepped){
						intermediatesPrepped = true;
						t1 = selectedPlatform.transform.position;
						if(onLeft){
							t1.x -= 3;
						} else{
							t1.x += 3;
						}
						
						t2 = new Vector3(t1.x, location.y, t1.z);
						t3 = new Vector3(t2.x, t2.y, location.z);
						t4 = location;
					}
					
					if(!finishedT1){
						selectedPlatform.transform.position = Vector3.Lerp(selectedPlatform.transform.position, t1, Time.deltaTime * 3.5f);
						if((selectedPlatform.transform.position - t1).magnitude < .02){
							selectedPlatform.transform.position = t1;
							finishedT1 = true;
						}
					} else if(!finishedT2){
						selectedPlatform.transform.position = Vector3.Lerp(selectedPlatform.transform.position, t2, Time.deltaTime * 3.5f);
						if((selectedPlatform.transform.position - t2).magnitude < .02){
							selectedPlatform.transform.position = t2;
							finishedT2 = true;
						}
					} else if(!finishedT3){
						selectedPlatform.transform.position = Vector3.Lerp(selectedPlatform.transform.position, t3, Time.deltaTime * 3.5f);
						if((selectedPlatform.transform.position - t3).magnitude < .02){
							selectedPlatform.transform.position = t3;
							finishedT3 = true;
						}
					} else {
						selectedPlatform.transform.position = Vector3.Lerp(selectedPlatform.transform.position, t4, Time.deltaTime * 3.5f);
					}
				} else {
					selectedPlatform.transform.position = Vector3.Lerp(selectedPlatform.transform.position, location, Time.deltaTime * 3.5f);
				}

				

				if((selectedPlatform.transform.position - location).magnitude < .02){
					selectedPlatform.transform.position = location;
					selectedPlatform.transform.GetChild(4).transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = standColor;
					selectedPlatform.transform.GetChild(4).transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = standColor;
					selectedPlatformLoc.GetComponent<Renderer>().material.color = pegColor;

					//reassign piece on platform to original parent
					for(int i = 0; i < 4; i++){
						GameObject piece = getPieceOnTile(selectedPlatform.transform.GetChild(i).gameObject);
						if(piece != null){
							if(isLightPiece(piece)){
								piece.transform.parent = GameObject.Find("White Pieces").transform;
							} else {
								piece.transform.parent = GameObject.Find("Black Pieces").transform;
							}
							
						}
					}
					upgrading = true;
				}
			}
		}

		if(moving && upgrading){
			if(!foundAllUpgradePieces){
				upgradePieces();
			} else {
				if(piecesToUpgrade.Count > 0 && !menuScript.upgradeScreen.activeSelf){
					menuScript.enableUpgradeScreen();
					//enable selection based off of color
					if(isLightPiece(piecesToUpgrade[0])){
						lightOptions.SetActive(true);
						darkOptions.SetActive(false);
					} else {
						lightOptions.SetActive(false);
						darkOptions.SetActive(true);
					}
				} else if(piecesToUpgrade.Count > 0 && menuScript.upgradeScreen.activeSelf && upgradeSelection != null){
					//upgrade Piece
					GameObject piece = piecesToUpgrade[0];
					GameObject newPiece;
					if(upgradeSelection.Contains("KL")){
						newPiece = Instantiate(lightKnightPrefab, piece.transform.position, Quaternion.identity);
					} else if(upgradeSelection.Contains("BL")){
						newPiece = Instantiate(lightBishopPrefab, piece.transform.position, Quaternion.identity);
					} else if(upgradeSelection.Contains("RL")){
						newPiece = Instantiate(lightRookPrefab, piece.transform.position, Quaternion.identity);
					} else if(upgradeSelection.Contains("QL")){
						newPiece = Instantiate(lightQueenPrefab, piece.transform.position, Quaternion.identity);
					} else if(upgradeSelection.Contains("KD")){
						newPiece = Instantiate(darkKnightPrefab, piece.transform.position, Quaternion.identity);
					} else if(upgradeSelection.Contains("BD")){
						newPiece = Instantiate(darkBishopPrefab, piece.transform.position, Quaternion.identity);
					} else if(upgradeSelection.Contains("RD")){
						newPiece = Instantiate(darkRookPrefab, piece.transform.position, Quaternion.identity);
					} else{
						newPiece = Instantiate(darkQueenPrefab, piece.transform.position, Quaternion.identity);
					}
					newPiece.transform.localScale = piece.transform.localScale;

					if(upgradeSelection.Contains("L")){
						lightPieces.Add(newPiece);
						lightPieces.Remove(piece);
						newPiece.transform.parent = GameObject.Find("White Pieces").transform;
					} else {
						darkPieces.Add(newPiece);
						darkPieces.Remove(piece);
						newPiece.transform.parent = GameObject.Find("Black Pieces").transform;
					}

					piecesToUpgrade.Remove(piece);
					Destroy(piece);
					upgradeSelection = null;
					menuScript.enableHUD();
					
				}
				if(piecesToUpgrade.Count == 0){
					menuScript.enableHUD();
					resetForNextAction();
				}
			}
		}
	}

	GameObject getPieceOnTile(GameObject tile){
		Vector3 location = tile.transform.position;

		foreach(GameObject piece in lightPieces){
			if((piece.transform.position - location).magnitude < .06){
				return piece;
			}
		}
		foreach(GameObject piece in darkPieces){
			if((piece.transform.position - location).magnitude < .06){
				return piece;
			}
		}
		return null;
	}

	bool isCaptured(GameObject piece){

		Vector3 capPos;

		if(isLightPiece(piece)){
			capPos = GameObject.Find("Captured Light").transform.position;
		} else {
			capPos = GameObject.Find("Captured Dark").transform.position;
		}

		if((piece.transform.position - capPos).magnitude < 3.6){
			return true;
		}
		return false;

		
	}


	//0 your team there
	//1 tile empty
	//2 enemy team there
	int tileAvailable(GameObject tile, GameObject piece){
		GameObject pieceOnTile = getPieceOnTile(tile);
		if(pieceOnTile == null){
			return 1;
		}

		bool lPiece = isLightPiece(piece);
		bool lFoundPiece = isLightPiece(pieceOnTile);

		if((lPiece && lFoundPiece) || (!lPiece && !lFoundPiece)){
			return 0;
		} else {
			return 2;
		}
	}

    List<GameObject> getAvailableMoves(GameObject piece){
        List<GameObject> moves = tiles;

		if(!enableMovesets){
			return moves;
		}

        if (piece.name.Contains("Pawn")){
            moves = getPawnMoves(piece);
        } else if(piece.name.Contains("Knight")){
			moves = getKnightMoves(piece);
		} else if(piece.name.Contains("King")){
			moves = getKingMoves(piece);
		} else if(piece.name.Contains("Rook")){
			moves = getRookMoves(piece);
		} else if(piece.name.Contains("Bishop")){
			moves = getBishopMoves(piece);
		} else if(piece.name.Contains("Queen")){
			moves = getQueenMoves(piece);
		}

        return moves;
    }

	List<GameObject> getPawnMoves(GameObject pawn){
		List<GameObject> moves = new List<GameObject>();

		Vector3 pawnPos = pawn.transform.position;
		List<Vector3> candidatePositions = new List<Vector3>();
		if(isLightPiece(pawn)){
			
			candidatePositions.Add(new Vector3(pawnPos.x+1, pawnPos.y, pawnPos.z+1));
			candidatePositions.Add(new Vector3(pawnPos.x-1, pawnPos.y, pawnPos.z+1));
			candidatePositions.Add(new Vector3(pawnPos.x, pawnPos.y, pawnPos.z+1));
			if(stationaryPawns.Contains(pawn)){
				candidatePositions.Add(new Vector3(pawnPos.x, pawnPos.y, pawnPos.z+2));
			}
		}
		else{
			
			candidatePositions.Add(new Vector3(pawnPos.x+1, pawnPos.y, pawnPos.z-1));
			candidatePositions.Add(new Vector3(pawnPos.x-1, pawnPos.y, pawnPos.z-1));
			candidatePositions.Add(new Vector3(pawnPos.x, pawnPos.y, pawnPos.z-1));
			if(stationaryPawns.Contains(pawn)){
				candidatePositions.Add(new Vector3(pawnPos.x, pawnPos.y, pawnPos.z-2));
			}
		}

        foreach(GameObject t in tiles){
            for(int i = 2; i < candidatePositions.Count; i++){
                float distance = distance2D(t.transform.position, candidatePositions[i]);
                if (distance < .1){
                    moves.Add(t);
                }
            }
        }

        moves = blockingPawns(pawn, moves);

        foreach (GameObject t in tiles){
			for(int i = 0; i < 2; i++){
				float distance = distance2D(t.transform.position, candidatePositions[i]);
				if(distance<0.1 && tileAvailable(t, pawn)==2){
					moves.Add(t);
				}
			}
		}

		return moves;
	}

	List<GameObject> getKnightMoves(GameObject knight){
		List<GameObject> moves = new List<GameObject>();
		Vector3 knightPos = knight.transform.position;
		List<Vector3> candidatePositions = new List<Vector3>();
		
		candidatePositions.Add(new Vector3(knightPos.x + 1f, knightPos.y, knightPos.z - 2f));
		candidatePositions.Add(new Vector3(knightPos.x + 1f, knightPos.y, knightPos.z + 2f));
		candidatePositions.Add(new Vector3(knightPos.x - 1f, knightPos.y, knightPos.z - 2f));
		candidatePositions.Add(new Vector3(knightPos.x - 1f, knightPos.y, knightPos.z + 2f));
		candidatePositions.Add(new Vector3(knightPos.x + 2f, knightPos.y, knightPos.z - 1f));
		candidatePositions.Add(new Vector3(knightPos.x + 2f, knightPos.y, knightPos.z + 1f));
		candidatePositions.Add(new Vector3(knightPos.x - 2f, knightPos.y, knightPos.z - 1f));
		candidatePositions.Add(new Vector3(knightPos.x - 2f, knightPos.y, knightPos.z + 1f));

		for(int i = 0; i < candidatePositions.Count; i++){
			foreach(GameObject t in tiles){
                if ((distance2D(t.transform.position, candidatePositions[i]) < .1) && (tileAvailable(t, knight) > 0)){
                    moves.Add(t);
                }
            }
        }

		return moves;
	}

	List<GameObject> getKingMoves(GameObject king){
		List<GameObject> moves = new List<GameObject>();
		Vector3 kingPos = king.transform.position;
		List<Vector3> candidatePositions = new List<Vector3>();

		candidatePositions.Add(new Vector3(kingPos.x, kingPos.y, kingPos.z + 1));
		candidatePositions.Add(new Vector3(kingPos.x, kingPos.y, kingPos.z - 1));
		candidatePositions.Add(new Vector3(kingPos.x + 1, kingPos.y, kingPos.z));
		candidatePositions.Add(new Vector3(kingPos.x - 1, kingPos.y, kingPos.z));
		candidatePositions.Add(new Vector3(kingPos.x + 1, kingPos.y, kingPos.z + 1));
		candidatePositions.Add(new Vector3(kingPos.x + 1, kingPos.y, kingPos.z - 1));
		candidatePositions.Add(new Vector3(kingPos.x - 1, kingPos.y, kingPos.z + 1));
		candidatePositions.Add(new Vector3(kingPos.x - 1, kingPos.y, kingPos.z - 1));

		for(int i = 0; i < candidatePositions.Count; i++){
			foreach(GameObject t in tiles){
                if ((distance2D(t.transform.position, candidatePositions[i]) < .1) && (tileAvailable(t, king) > 0)){
                    moves.Add(t);
                }
            }
        }

		return moves;
	}

	List<GameObject> getRookMoves(GameObject rook){
		List<GameObject> moves = new List<GameObject>();
		Vector3 rookPos = rook.transform.position;


		//forward white
		for(int i = 1; i < 10; i++){
			Vector3 candidate = new Vector3(rookPos.x, rookPos.y, rookPos.z + i);

			bool blocked = false;
			foreach(GameObject t in tiles){
				int avail = tileAvailable(t, rook);
				bool correctDistance = (distance2D(t.transform.position, candidate) < .1);

				if(correctDistance && (avail > 0)){
					moves.Add(t);
				}

				if(correctDistance && (avail == 0 || avail == 2)){
					blocked = true;
				}
			}
			if(blocked){
				break;
			}
		}

		//backward white
		for(int i = 1; i < 10; i++){
			Vector3 candidate = new Vector3(rookPos.x, rookPos.y, rookPos.z - i);

			bool blocked = false;
			foreach(GameObject t in tiles){
				int avail = tileAvailable(t, rook);
				bool correctDistance = (distance2D(t.transform.position, candidate) < .1);

				if(correctDistance && (avail > 0)){
					moves.Add(t);
				}

				if(correctDistance && (avail == 0 || avail == 2)){
					blocked = true;
				}
			}
			if(blocked){
				break;
			}
		}

		//right white
		for(int i = 1; i < 6; i++){
			Vector3 candidate = new Vector3(rookPos.x + i, rookPos.y, rookPos.z);

			bool blocked = false;
			foreach(GameObject t in tiles){
				int avail = tileAvailable(t, rook);
				bool correctDistance = (distance2D(t.transform.position, candidate) < .1);

				if(correctDistance && (avail > 0)){
					moves.Add(t);
				}

				if(correctDistance && (avail == 0 || avail == 2)){
					blocked = true;
				}
			}
			if(blocked){
				break;
			}
		}

		//left white
		for(int i = 1; i < 6; i++){
			Vector3 candidate = new Vector3(rookPos.x - i, rookPos.y, rookPos.z);

			bool blocked = false;
			foreach(GameObject t in tiles){
				int avail = tileAvailable(t, rook);
				bool correctDistance = (distance2D(t.transform.position, candidate) < .1);

				if(correctDistance && (avail > 0)){
					moves.Add(t);
				}

				if(correctDistance && (avail == 0 || avail == 2)){
					blocked = true;
				}
			}
			if(blocked){
				break;
			}
		}

		return moves;
	}

	List<GameObject> getBishopMoves(GameObject bishop){
		List<GameObject> moves = new List<GameObject>();
		Vector3 bishopPos = bishop.transform.position;

		
		//forward right white
		for(int i = 1; i < 6; i++){
			Vector3 candidate = new Vector3(bishopPos.x + i, bishopPos.y, bishopPos.z + i);

			bool blocked = false;
			foreach(GameObject t in tiles){
				int avail = tileAvailable(t, bishop);
				bool correctDistance = (distance2D(t.transform.position, candidate) < .1);

				if(correctDistance && (avail > 0)){
					moves.Add(t);
				}

				if(correctDistance && (avail == 0 || avail == 2)){
					blocked = true;
				}
			}
			if(blocked){
				break;
			}
		}

		//forward left white
		for(int i = 1; i < 6; i++){
			Vector3 candidate = new Vector3(bishopPos.x - i, bishopPos.y, bishopPos.z + i);

			bool blocked = false;
			foreach(GameObject t in tiles){
				int avail = tileAvailable(t, bishop);
				bool correctDistance = (distance2D(t.transform.position, candidate) < .1);

				if(correctDistance && (avail > 0)){
					moves.Add(t);
				}

				if(correctDistance && (avail == 0 || avail == 2)){
					blocked = true;
				}
			}
			if(blocked){
				break;
			}
		}

		//backward right white
		for(int i = 1; i < 6; i++){
			Vector3 candidate = new Vector3(bishopPos.x + i, bishopPos.y, bishopPos.z - i);

			bool blocked = false;
			foreach(GameObject t in tiles){
				int avail = tileAvailable(t, bishop);
				bool correctDistance = (distance2D(t.transform.position, candidate) < .1);

				if(correctDistance && (avail > 0)){
					moves.Add(t);
				}

				if(correctDistance && (avail == 0 || avail == 2)){
					blocked = true;
				}
			}
			if(blocked){
				break;
			}
		}

		//backward left white
		for(int i = 1; i < 6; i++){
			Vector3 candidate = new Vector3(bishopPos.x - i, bishopPos.y, bishopPos.z - i);

			bool blocked = false;
			foreach(GameObject t in tiles){
				int avail = tileAvailable(t, bishop);
				bool correctDistance = (distance2D(t.transform.position, candidate) < .1);

				if(correctDistance && (avail > 0)){
					moves.Add(t);
				}

				if(correctDistance && (avail == 0 || avail == 2)){
					blocked = true;
				}
			}
			if(blocked){
				break;
			}
		}

		return moves;
	}

	List<GameObject> getQueenMoves(GameObject queen){
		List<GameObject> moves = new List<GameObject>();
		moves.AddRange(getRookMoves(queen));
		moves.AddRange(getBishopMoves(queen));
		return moves;
	}

    float distance2D(Vector3 a, Vector3 b)
    {
        return (a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z);
    }

    GameObject getTileUnderPiece(GameObject piece){
        GameObject tile = null;

        for(int i = 0; i<tiles.Count; i++){
            float dist = (piece.transform.position - tiles[i].transform.position).magnitude;
            if (dist < 0.051){
                tile = tiles[i];
            }
        }

        return tile;
    }

    void colorAvailableTiles(List<GameObject> moves)
    {
        for (int i = 0; i < moves.Count; i++){
            if (isLightTile(moves[i])){
				if(getPieceOnTile(moves[i]) == null){
					moves[i].GetComponent<Renderer>().material.color = new Color(215f/255f, 117f/255f, 130f/255f);
				} else {
					moves[i].GetComponent<Renderer>().material.color = new Color(204f/255f, 33f/255f, 33f/255f);
				}
            }
            else{
				if(getPieceOnTile(moves[i]) == null){
					moves[i].GetComponent<Renderer>().material.color = new Color(109f/255f, 54f/255f, 147f/255f);
				} else {
					moves[i].GetComponent<Renderer>().material.color = new Color(145f/255f, 25f/255f, 25f/255f);
				}
                
            }
        }
    }
        

	void reColorTiles(List<GameObject> tilesToBeColored){
		for(int i = 0; i < tilesToBeColored.Count; i++){
			if(isLightTile(tilesToBeColored[i])){
				tilesToBeColored[i].GetComponent<Renderer>().material.color = lightTileColor;
			} else {
				tilesToBeColored[i].GetComponent<Renderer>().material.color = darkTileColor;
			}
		}
	}

	void reColorPegs(){
		for(int i = 0; i < pegs.Count; i++){
			pegs[i].GetComponent<Renderer>().material.color = pegColor;
		}
	}

	List<GameObject> getAvailablePegs(GameObject stand){
		List<GameObject> preResults = new List<GameObject>();
		GameObject currentPeg = null;
		preResults.AddRange(pegs);
		for(int i  = 0; i < pegs.Count; i++){
			for(int j = 0; j <stands.Count; j++){
				float distance = (pegs[i].transform.position-stands[j].transform.position).magnitude;
				if(distance < 0.1){
					preResults.Remove(pegs[i]);
					break;
				}
			}

			float dist= (pegs[i].transform.position-stand.transform.position).magnitude;
			if(dist < .1){
				currentPeg = pegs[i];
			}
		}

		List<GameObject> results = new List<GameObject>();
		results.AddRange(preResults);

		int amtOnStand = canMoveStand(stand.transform.parent.gameObject);

		foreach(GameObject p in preResults){

			bool sameX = Math.Abs(p.transform.position.x - currentPeg.transform.position.x) < .05;
			bool sameZed = Math.Abs(p.transform.position.z - currentPeg.transform.position.z) < .05;
			bool sameY = Math.Abs(p.transform.position.y - currentPeg.transform.position.y) < .05;

			if(!sameX && !(sameZed && sameY)){
				results.Remove(p);
			}else if(amtOnStand > 0){
				//remove backward choices
				if(lightsTurn && (currentPeg.transform.position.z - p.transform.position.z) > .1){
					results.Remove(p);
			} else if(!lightsTurn && (currentPeg.transform.position.z - p.transform.position.z) < -.1){
					results.Remove(p);
				}
			}
		}

		return results;
	}


	// return count of pieces on stand
	//0 - empty - can go backwards
	//1 - can move forwards
	//2 or more - cannot move
	//returns 10 when enemy on stand - cannot move
	int canMoveStand(GameObject standLevel){
		int count = 0;
		for(int i = 0; i < 4; i++){
			GameObject piece = getPieceOnTile(standLevel.transform.GetChild(i).gameObject);
			if(piece != null){
				if((lightsTurn && isLightPiece(piece)) || (!lightsTurn && isDarkPiece(piece))){
					count += 1;
				} else {
					return 10;
				}
			}
		}
		return count;
	}

    List<GameObject> blockingPawns(GameObject pawn, List<GameObject> moves){
        List<GameObject> occupiedTiles = new List<GameObject>();
        foreach(GameObject m in moves)
        {
            if(tileAvailable(m, pawn) != 1){
                occupiedTiles.Add(m);
            }
        }

        List<GameObject> finalMoves = new List<GameObject>();
        finalMoves.AddRange(moves);
        foreach(GameObject m in moves)
        {
            if (occupiedTiles.Contains(m)){
                finalMoves.Remove(m);
            }
            else{
                for(int i = 0; i < occupiedTiles.Count; i++){
                    float diffZed = m.transform.position.z - occupiedTiles[i].transform.position.z;
                    if (isLightPiece(pawn) && diffZed > 0.5) {
                        finalMoves.Remove(m);
                    }
                    else if(isDarkPiece(pawn) && diffZed < -0.5){
                        finalMoves.Remove(m);
                    }
                }
            }
        }
        return finalMoves;
    }

	void updateBottomPromt(){
		timeSinceTextChange += Time.deltaTime;
		if(timeSinceTextChange > 5.0f){
			setBottomPrompt("");
		}
	}

	void setBottomPrompt(String str){
		if(bottomPrompt == null){
			//can occur due to menus getting disabled and enabled
			bottomPrompt = GameObject.Find("Bottom Prompt");
		}
		bottomPrompt.GetComponent<Text>().text = str;
		timeSinceTextChange = 0f;
	}

    void movePieceToTile(GameObject piece, GameObject tile)
    {
        Vector3 location = tile.transform.position;
        location.y += .05f;
        piece.transform.position = location;
    }

    void moveStandToPeg(GameObject stand, GameObject peg){
        Vector3 location = peg.transform.position;
        for (int i = 0; i < 4; i++)
        {
            GameObject piece = getPieceOnTile(stand.transform.GetChild(i).gameObject);
            if (piece != null)
            {
                piece.transform.parent = stand.transform;
            }
        }

        location.x -= .5f;
        location.y += (2.2f - .051f);
        location.z -= .5f;

        stand.transform.position = location;

        for (int i = 0; i < 4; i++){
            GameObject piece = getPieceOnTile(stand.transform.GetChild(i).gameObject);
            if (piece != null){
                if (isLightPiece(piece)){
                    piece.transform.parent = GameObject.Find("White Pieces").transform;
                }
                else{
                    piece.transform.parent = GameObject.Find("Black Pieces").transform;
                }
            }
        }
    }

    bool check(GameObject king, List<GameObject> opposingPieces)
    {
        List<GameObject> opponentMoveSet = new List<GameObject>();
        foreach(GameObject opponent in opposingPieces){
            //NEED TO GET ALL LEGAL ACTIONS FOR THE OPPONENT
            opponentMoveSet.AddRange(getAvailableMoves(opponent));
        }
        if (opponentMoveSet.Contains(getTileUnderPiece(king))){
            return true;
        }
        return false;
    }

    List<GameObject[]> getSafeMoves(GameObject piece, List<GameObject> moves)
    {
        GameObject myKing = GameObject.Find("KingLight");
        List<GameObject> opponentsPieces = new List<GameObject>();
        if (lightsTurn){
            opponentsPieces.AddRange(darkPieces);
        }
        else
        {
            myKing = GameObject.Find("KingDark");
            opponentsPieces.AddRange(lightPieces);
        }

        GameObject ogTile = getTileUnderPiece(piece);
        List<GameObject> finalMoves = new List<GameObject>();
        finalMoves.AddRange(moves);

        foreach(GameObject pos in moves)
        {
			GameObject enemyOnPos = getPieceOnTile(pos);
			//assumes no friendly in moves
			if(enemyOnPos != null){
				enemyOnPos.transform.position = new Vector3(1000f, 1000f, 1000f);
			}
            simulateMovement(new GameObject[2] { piece, pos});
            if(check(myKing, opponentsPieces)){
                finalMoves.Remove(pos);
            }

			if(enemyOnPos != null){
                simulateMovement(new GameObject[2] { enemyOnPos, pos});
			}
        }
        simulateMovement(new GameObject[2] { piece, ogTile});

        List<GameObject[]> pairs = new List<GameObject[]>();
        foreach(GameObject tile in finalMoves){
            pairs.Add(new GameObject[2] { piece, tile });
        }

        return pairs;
    }

    List<GameObject[]> getAllLegalActions(List<GameObject> friendlyPieces)
    {
        List<GameObject[]> listOfLegalActions = new List<GameObject[]>();

        foreach (GameObject f in friendlyPieces){
            listOfLegalActions.AddRange(getSafeMoves(f, getAvailableMoves(f)));
        }

        List<GameObject> moveableStands = getAvailableStands();
        foreach(GameObject s in moveableStands){
            List<GameObject> pegsAvailableToStand = getAvailablePegs(s);
            listOfLegalActions.AddRange(getLegalPegs(s, pegsAvailableToStand));
        }

        return listOfLegalActions;
    }

    List<GameObject[]> getLegalPegs(GameObject stand, List<GameObject> pegList){
        GameObject parent = stand.transform.parent.gameObject;

        GameObject myKing = GameObject.Find("KingLight");
        List<GameObject> opponentsPieces = new List<GameObject>();
        if (lightsTurn)
        {
            opponentsPieces.AddRange(darkPieces);
        }
        else
        {
            myKing = GameObject.Find("KingDark");
            opponentsPieces.AddRange(lightPieces);
        }

        GameObject ogPeg = getPegUnderStand(stand);
        List<GameObject> finalMoves = new List<GameObject>();
        finalMoves.AddRange(pegList);

        foreach(GameObject p in pegList){
            simulateMovement(new GameObject[2] { parent, p});
            if (check(myKing, opponentsPieces)){
                finalMoves.Remove(p);
            }
        }

        simulateMovement(new GameObject[2] { parent, ogPeg });

        List<GameObject[]> pairs = new List<GameObject[]>();
        foreach(GameObject peg in finalMoves){
            pairs.Add(new GameObject[2] { stand, peg });
        }
        return pairs;
    }

    List<GameObject> getAvailableStands(){
        List<GameObject> moveableStands = new List<GameObject>();
        foreach(GameObject s in stands){
            if (canMoveStand(s.transform.parent.gameObject) < 2){
                moveableStands.Add(s);
            }
        }
        return moveableStands;
    }

    void simulateMovement(GameObject[] pair){
        if (pair[1].name.Contains("Tile")) {
            movePieceToTile(pair[0], pair[1]);
        }
        else if (pair[1].name.Contains("Peg"))
        {
            moveStandToPeg(pair[0], pair[1]);
        }
    }

    void upgradePieces(){
		//upgrade light pieces
		List<GameObject> tempLightPieces = new List<GameObject>();
		tempLightPieces.AddRange(lightPieces);
		foreach(GameObject piece in tempLightPieces){
			if(piece.name.Contains("Pawn")){
				GameObject tile = getTileUnderPiece(piece);

				bool onLevel3 = tile.transform.parent.gameObject == GameObject.Find("Level 3");
				bool onBackRowOfMainLevel = tile.name.EndsWith("3");
				bool onLeftTile = tile.name.Contains("Tile 0");
				bool onMiddleTile = tile.name.Contains("Tile 1") || tile.name.Contains("Tile 2");
				bool onRightTile = tile.name.Contains("Tile 3");
				
				bool onSubLevel = tile.transform.parent.gameObject.name.Contains(".5.1") || tile.transform.parent.gameObject.name.Contains(".5.2");
				bool onBackRowOfSubLevel = tile.name.EndsWith("1");
				bool onBackLeftPeg = false;
				bool onBackRightPeg = false;

				GameObject backLeftPeg = GameObject.Find("Level 3").transform.GetChild(18).gameObject;
				GameObject backRightPeg = GameObject.Find("Level 3").transform.GetChild(19).gameObject;
				GameObject backLeftStand = null;
				GameObject backRightStand = null;

				foreach(GameObject stand in stands){
					float dist1 = (backLeftPeg.transform.position - stand.transform.position).magnitude;
					float dist2 = (backRightPeg.transform.position - stand.transform.position).magnitude;
					if(dist1 < .1 ){
						backLeftStand = stand;
					} else if( dist2 < .1){
						backRightStand = stand;
					}
				}

				if(onSubLevel){
					float dist1 = (backLeftPeg.transform.position - tile.transform.parent.gameObject.transform.GetChild(4).gameObject.transform.position).magnitude;
					float dist2 = (backRightPeg.transform.position - tile.transform.parent.gameObject.transform.GetChild(4).gameObject.transform.position).magnitude;
					if(dist1 < .1 ){
						onBackLeftPeg = true;
					} else if( dist2 < .1){
						onBackRightPeg = true;
					}
				}

				bool onBackMiddleOfLvl3 = (onLevel3 && onBackRowOfMainLevel && onMiddleTile);
				bool onBackLeftOfLvl3NoStand = (onLevel3 && onBackRowOfMainLevel && onLeftTile && (backLeftStand == null));
				bool onBackRightOfLvl3NoStand = (onLevel3 && onBackRowOfMainLevel && onRightTile && (backRightStand == null));
				bool onBackOfSubLvlAndSubLvlOnBack = (onSubLevel && onBackRowOfSubLevel && (onBackLeftPeg || onBackRightPeg));

				bool shouldUpgrade = onBackMiddleOfLvl3 || onBackLeftOfLvl3NoStand || onBackRightOfLvl3NoStand || onBackOfSubLvlAndSubLvlOnBack;

				if(shouldUpgrade){
					piecesToUpgrade.Add(piece);
				}
			}
		}

		//upgrade dark pieces
		List<GameObject> tempDarkPieces = new List<GameObject>();
		tempDarkPieces.AddRange(darkPieces);
		foreach(GameObject piece in tempDarkPieces){
			if(piece.name.Contains("Pawn")){
				GameObject tile = getTileUnderPiece(piece);

				bool onLevel1 = tile.transform.parent.gameObject == GameObject.Find("Level 1");
				bool onFrontRowOfMainLevel = tile.name.EndsWith("0");
				bool onLeftTile = tile.name.Contains("Tile 0");
				bool onMiddleTile = tile.name.Contains("Tile 1") || tile.name.Contains("Tile 2");
				bool onRightTile = tile.name.Contains("Tile 3");
				
				bool onSubLevel = tile.transform.parent.gameObject.name.Contains(".5.1") || tile.transform.parent.gameObject.name.Contains(".5.2");
				bool onFrontRowOfSubLevel = tile.name.EndsWith("0");
				bool onFrontLeftPeg = false;
				bool onFrontRightPeg = false;

				GameObject frontLeftPeg = GameObject.Find("Level 1").transform.GetChild(16).gameObject;
				GameObject frontRightPeg = GameObject.Find("Level 1").transform.GetChild(17).gameObject;
				GameObject frontLeftStand = null;
				GameObject frontRightStand = null;

				foreach(GameObject stand in stands){
					float dist1 = (frontLeftPeg.transform.position - stand.transform.position).magnitude;
					float dist2 = (frontRightPeg.transform.position - stand.transform.position).magnitude;
					if(dist1 < .1 ){
						frontLeftStand = stand;
					} else if( dist2 < .1){
						frontRightStand = stand;
					}
				}

				if(onSubLevel){
					float dist1 = (frontLeftPeg.transform.position - tile.transform.parent.gameObject.transform.GetChild(4).gameObject.transform.position).magnitude;
					float dist2 = (frontRightPeg.transform.position - tile.transform.parent.gameObject.transform.GetChild(4).gameObject.transform.position).magnitude;
					if(dist1 < .1 ){
						onFrontLeftPeg = true;
					} else if( dist2 < .1){
						onFrontRightPeg = true;
					}
				}

				bool onFrontMiddleOfLvl1 = (onLevel1 && onFrontRowOfMainLevel && onMiddleTile);
				bool onFrontLeftOfLvl1NoStand = (onLevel1 && onFrontRowOfMainLevel && onLeftTile && (frontLeftStand == null));
				bool onFrontRightOfLvl1NoStand = (onLevel1 && onFrontRowOfMainLevel && onRightTile && (frontRightStand == null));
				bool onFrontOfSubLvlAndSubLvlOnFront = (onSubLevel && onFrontRowOfSubLevel && (onFrontLeftPeg || onFrontRightPeg));

				bool shouldUpgrade = onFrontMiddleOfLvl1 || onFrontLeftOfLvl1NoStand || onFrontRightOfLvl1NoStand || onFrontOfSubLvlAndSubLvlOnFront;

				if(shouldUpgrade){
					piecesToUpgrade.Add(piece);
				}
			}
		}
		foundAllUpgradePieces = true;
	}

    void updateTimers(){
        TimeSpan lightSpan = new TimeSpan(0, 0, (int)lightTimer);
        TimeSpan darkSpan = new TimeSpan(0, 0, (int)darkTimer);
		if(menuScript.hud.activeSelf){
        	GameObject.Find("LightTimer").GetComponent<Text>().text = string.Format("{0}:{1:00}", (int)lightSpan.TotalMinutes, lightSpan.Seconds);
        	GameObject.Find("DarkTimer").GetComponent<Text>().text = string.Format("{0}:{1:00}", (int)darkSpan.TotalMinutes, darkSpan.Seconds);
		}
    }

    GameObject getPegUnderStand(GameObject stand){
        GameObject peg = pegs[0];
        float min_dist = (stand.transform.position-peg.transform.position).magnitude;
        for(int i = 1; i < pegs.Count; i++){
            float curr_dist = (stand.transform.position-pegs[i].transform.position).magnitude;
            if (min_dist > curr_dist){
                peg = pegs[i];
                min_dist = curr_dist;
            }
        }
        return peg;
    }

}
