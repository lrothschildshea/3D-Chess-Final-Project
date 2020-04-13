using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestScript : MonoBehaviour {

	private GameObject selectedPiece = null;
	private GameObject selectedTile = null;
	private GameObject selectedPlatform = null;
	private GameObject selectedPlatformLoc = null;
	private bool moving = false;

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


	// Use this for initialization
	void Start () {
		lightsTurn = true;
        lightPieces = new List<GameObject>();
        darkPieces = new List<GameObject>();
        tiles = new List<GameObject>();
        pegs = new List<GameObject>();
        stands = new List<GameObject>();
        availableMoves = new List<GameObject>();
        stationaryPawns = new List<GameObject>();
        levels = new List<GameObject>();
        //get lists of objects used throughout the game
        gameBoard = GameObject.Find("GameBoard");
        foreach(Transform group in gameBoard.transform){
            if (group.gameObject.name.Contains("Level"))
            {
                levels.Add(group.gameObject);
            }
            foreach(Transform piece in group){
                GameObject gamePiece = piece.gameObject;
                if(gamePiece.name.Contains("Light")){
                    lightPieces.Add(gamePiece);
                }
                if (gamePiece.name.Contains("Dark")){
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

	void resetForNextAction(){
		resetForNextActionWithoutTogglingTurn();
		lightsTurn = !lightsTurn;
	}

	void resetForNextActionWithoutTogglingTurn(){
		selectedPiece = null;
		selectedTile = null;
		selectedPlatform = null;
		selectedPlatformLoc = null;
		capturedPiece = null;
		moving = false;
        availableMoves = null;
	}
	
	// Update is called once per frame
	void Update () {

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
                    availableMoves = getAvailableMoves(selectedPiece);
				}

				//click on destination tile
				if(selectedPiece != null && isTile(clicked) && availableMoves.Contains(clicked)){
                    int available = tileAvailable(clicked, selectedPiece);
					if(available > 0){
                        for(int i = 0; i < availableMoves.Count; i++){
                            if(isLightTile(availableMoves[i])){
                                availableMoves[i].GetComponent<Renderer>().material.color = lightTileColor;
                            } else {
                                availableMoves[i].GetComponent<Renderer>().material.color = darkTileColor;
                            }
                        }
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
					GameObject parent = clicked.transform.parent.gameObject;
					parent.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Color.green;
					parent.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = Color.green;
					selectedPlatform = parent.transform.parent.gameObject;
				}

				//click on destination peg
				if(selectedPlatform != null && isPeg(clicked)){
					selectedPlatformLoc = clicked;
					clicked.GetComponent<Renderer>().material.color = Color.green;
					moving = true;
				}

			}
     	}

		if(moving){


			//moving piece
			if(selectedPiece != null){
				Vector3 location = selectedTile.transform.position;
				location.y += .05f;
				selectedPiece.transform.position = Vector3.Lerp(selectedPiece.transform.position, location, Time.deltaTime * 3.5f);

				Vector3 dest = new Vector3(100000, 100000, 100000);

				if(capturedPiece != null){
					if(isLightPiece(capturedPiece)){
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
						if(isLightPiece(capturedPiece)){
							capturedPiece.transform.position = dest;
						} else {
							capturedPiece.transform.position = dest;
						}
					}
					resetForNextAction();
				}
			}

			//moving platform
			if(selectedPlatform != null){
				Vector3 location = selectedPlatformLoc.transform.position;

				//adjust height
				location.x -= .5f;
				location.y += (2.2f - .051f);
				location.z -= .5f;

				selectedPlatform.transform.position = Vector3.Lerp(selectedPlatform.transform.position, location, Time.deltaTime * 3.5f);

				if((selectedPlatform.transform.position - location).magnitude < .02){
					selectedPlatform.transform.position = location;
					selectedPlatform.transform.GetChild(4).transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = standColor;
					selectedPlatform.transform.GetChild(4).transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = standColor;
					selectedPlatformLoc.GetComponent<Renderer>().material.color = pegColor;
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
        GameObject[] location = getLevelAndTileOfPiece(piece);
        GameObject level = location[0];
        GameObject tile = location[1];
        int x = Int32.Parse(tile.name.Substring(5,1));
        int y = Int32.Parse(tile.name.Substring(7));
        //ADD Z TO HANDLE LEVELS BETTER

        if (piece.name.Contains("Pawn")){
            moves = new List<GameObject>();
            String candidate = "";
            if (isLightPiece(piece)){
                if (y + 1 < 4){
                    candidate = "Tile " + x.ToString() + " " + (y+1).ToString();
                    for(int i = 0; i < tiles.Count; i++){
                        if(tiles[i].transform.parent.name == level.name && tiles[i].name == candidate){
                            moves.Add(tiles[i]);
                        }
                    }
                }
                if(stationaryPawns.Contains(piece)){
                    if (y + 2 < 4){
                        candidate = "Tile " + x.ToString() + " " + (y + 2).ToString();
                        for (int i = 0; i < tiles.Count; i++){
                            if (tiles[i].transform.parent.name == level.name && tiles[i].name == candidate){
                                moves.Add(tiles[i]);
                            }
                        }
                    }
                }
            }
            else{
                if (y - 1 > -1){
                    candidate = "Tile " + x.ToString() + " " + (y - 1).ToString();
                    for (int i = 0; i < tiles.Count; i++){
                        if (tiles[i].transform.parent.name == level.name && tiles[i].name == candidate){
                            moves.Add(tiles[i]);
                        }
                    }
                }
                if (stationaryPawns.Contains(piece)){
                    if (y - 2 > -1){
                        candidate = "Tile " + x.ToString() + " " + (y - 2).ToString();
                        for (int i = 0; i < tiles.Count; i++){
                            if (tiles[i].transform.parent.name == level.name && tiles[i].name == candidate)
                            {
                                moves.Add(tiles[i]);
                            }
                        }
                    }
                }
            }
        }

        for(int i  = 0; i < moves.Count; i++){
			if(isLightTile(moves[i])){
				moves[i].GetComponent<Renderer>().material.color = Color.cyan;
			} else {
				moves[i].GetComponent<Renderer>().material.color = Color.blue;
			}
            
        }
        return moves;
    }

    bool isLightTile(GameObject tile){
        if(tile.name.Contains("0 0")|| tile.name.Contains("1 1") || tile.name.Contains("2 0") || tile.name.Contains("3 1") || tile.name.Contains("0 2") || tile.name.Contains("1 3") || tile.name.Contains("2 2") || tile.name.Contains("3 3")){
            return false;
        }
        return true;
    }

    GameObject[] getLevelAndTileOfPiece(GameObject piece){
        GameObject level = null;
        GameObject tile = null;
        for(int i = 0; i < levels.Count; i++){
            //NEED TO FIX HOW SUBLEVELS WORK!!!
            float vert_dist = Math.Abs(piece.transform.position.y - levels[i].transform.position.y);
            if (vert_dist < 0.052){
                level = levels[i];
            }
        }

        for(int i = 0; i<tiles.Count; i++){
            float dist = (piece.transform.position - tiles[i].transform.position).magnitude;
            if (dist < 0.052){
                tile = tiles[i];
            }
        }

        return new GameObject[2] {level,tile};
    }
}
