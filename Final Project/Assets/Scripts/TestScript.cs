using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestScript : MonoBehaviour {

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

	bool isLightTile(GameObject tile){
        if(tile.name.Contains("0 0")|| tile.name.Contains("1 1") || tile.name.Contains("2 0") || tile.name.Contains("3 1") || tile.name.Contains("0 2") || tile.name.Contains("1 3") || tile.name.Contains("2 2") || tile.name.Contains("3 3")){
            return false;
        }
        return true;
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
		availablePegs = null;
		standPreppedForMove = false;
		reColorPegs();
		reColorTiles(tiles);
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
				if(isStand(clicked) && selectedPlatform == null && selectedPiece == null && canMoveStand(clicked.transform.parent.gameObject.transform.parent.gameObject)){
					GameObject parent = clicked.transform.parent.gameObject;
					selectedPlatform = parent.transform.parent.gameObject;
					parent.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Color.green;
					parent.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = Color.green;
					availablePegs = getAvailablePegs(parent);
					foreach(GameObject p in availablePegs){
						p.GetComponent<Renderer>().material.color = Color.red; 
					}
				}

				//click on destination peg
				if(selectedPlatform != null && isPeg(clicked) && availablePegs.Contains(clicked)){
					selectedPlatformLoc = clicked;
					reColorPegs();
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
						capturedPiece.transform.position = dest;
					}
					resetForNextAction();
				}
			}

			//moving platform
			if(selectedPlatform != null){
				Vector3 location = selectedPlatformLoc.transform.position;


				if(!standPreppedForMove){
					//assign piece onp platform to platform for movement
					for(int i = 0; i < 4; i++){
						GameObject piece = getPieceOnTile(selectedPlatform.transform.GetChild(i).gameObject);
						if(piece != null){
							piece.transform.parent = selectedPlatform.transform;
						}
					}
					standPreppedForMove = true;
				}

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

        if (piece.name.Contains("Pawn")){
            moves = getPawnMoves(piece);
        }

		//color available squares
        for(int i  = 0; i < moves.Count; i++){
			if(isLightTile(moves[i])){
				moves[i].GetComponent<Renderer>().material.color = Color.cyan;
			} else {
				moves[i].GetComponent<Renderer>().material.color = Color.blue;
			}
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
			for(int i = 0; i < candidatePositions.Count; i++){
				float distance = (t.transform.position.x - candidatePositions[i].x)*(t.transform.position.x - candidatePositions[i].x) + (t.transform.position.z - candidatePositions[i].z)*(t.transform.position.z - candidatePositions[i].z);
				if(i < 2){
					if(distance<0.1 && tileAvailable(t, pawn)==2){
						moves.Add(t);
					}
				}
				else{
					if(distance < .1){
						moves.Add(t);
					}
				}
			}
		}

		//BLOCKING RULES

		return moves;
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

	//ADD CHECK FOR MOVING backwards rule !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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

		foreach(GameObject p in preResults){

			bool sameX = Math.Abs(p.transform.position.x - currentPeg.transform.position.x) < .05;
			bool sameZed = Math.Abs(p.transform.position.z - currentPeg.transform.position.z) < .05;
			bool sameY = Math.Abs(p.transform.position.y - currentPeg.transform.position.y) < .05;

			if(!sameX && !(sameZed && sameY)){
				results.Remove(p);
			}
		}


		//figure out backwards rule here

		return results;
	}


	//probably make this return int so backwards is easier
	bool canMoveStand(GameObject standLevel){
		int count = 0;
		for(int i = 0; i < 4; i++){
			GameObject piece = getPieceOnTile(standLevel.transform.GetChild(i).gameObject);
			if(piece != null){
				if((lightsTurn && isLightPiece(piece)) || (!lightsTurn && isDarkPiece(piece))){
					count += 1;
				} else {
					Debug.Log("enemy on stand");
					return false;
				}
			}
		}
		return count < 2;
	}

}
