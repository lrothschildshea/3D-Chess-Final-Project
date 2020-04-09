using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

	private GameObject selectedPiece = null;
	private GameObject selectedTile = null;
	private GameObject selectedPlatform = null;
	private GameObject selectedPlatformLoc = null;
	private bool moving = false;
	private Color selectedPieceColor = Color.red;
	private Color selectedTileColor = Color.red;
	private Color selectedPlatformColor = Color.red;
	private Color selectedPlatformLocColor = Color.red;

    private GameObject gameBoard;
    private List<GameObject> lightPieces;
    private List<GameObject> darkPieces;
    private List<GameObject> tiles;
    private List<GameObject> pegs;
    private List<GameObject> stands;


	// Use this for initialization
	void Start () {
        lightPieces = new List<GameObject>();
        darkPieces = new List<GameObject>();
        tiles = new List<GameObject>();
        pegs = new List<GameObject>();
        stands = new List<GameObject>();
        //get lists of objects used throughout the game
        gameBoard = GameObject.Find("GameBoard");
        foreach(Transform group in gameBoard.transform)
        {
            foreach(Transform piece in group)
            {
                GameObject gamePiece = piece.gameObject;
                if(gamePiece.name.Contains("Light"))
                {
                    lightPieces.Add(gamePiece);
                }
                if (gamePiece.name.Contains("Dark"))
                {
                    darkPieces.Add(gamePiece);
                }
                if (gamePiece.name.Contains("Tile"))
                {
                    tiles.Add(gamePiece);
                }
                if (gamePiece.name.Contains("Peg"))
                {
                    pegs.Add(gamePiece);
                }
                if (gamePiece.name.Contains("Stand"))
                {
                    stands.Add(gamePiece);
                }
            }
        }
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
		selectedPiece = null;
		selectedTile = null;
		selectedPlatform = null;
		selectedPlatformLoc = null;
		moving = false;
		selectedPieceColor = Color.red;
		selectedTileColor = Color.red;
		selectedPlatformColor = Color.red;
		selectedPlatformLocColor = Color.red;
	}
	
	// Update is called once per frame
	void Update () {

		//detect the object that has been clicked and change its color
		if(Input.GetMouseButtonDown(0) && !moving) {
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, 100)){
				GameObject clicked = hit.transform.gameObject;

				//click on piece
                if ((isDarkPiece(clicked) || isLightPiece(clicked)) && selectedPiece == null && selectedPlatform == null){
					selectedPiece = clicked;
					selectedPieceColor = clicked.GetComponent<Renderer>().material.color;
					clicked.GetComponent<Renderer>().material.color = Color.yellow;
				}

				//click on destination tile
				if(selectedPiece != null && isTile(clicked)){
					if(tileAvailable(clicked, selectedPiece)){
						selectedTile = clicked;
						selectedTileColor = clicked.GetComponent<Renderer>().material.color;
						clicked.GetComponent<Renderer>().material.color = Color.yellow;
						moving = true;
					}

				}

				//click on stand
				if(isStand(clicked) && selectedPlatform == null && selectedPiece == null){
					GameObject parent = clicked.transform.parent.gameObject;
					selectedPlatformColor = parent.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color;
					parent.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Color.green;
					parent.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = Color.green;
					selectedPlatform = parent.transform.parent.gameObject;
				}

				//click on destination peg
				if(selectedPlatform != null && isPeg(clicked)){
					selectedPlatformLoc = clicked;
					selectedPlatformLocColor = clicked.GetComponent<Renderer>().material.color;
					clicked.GetComponent<Renderer>().material.color = Color.green;
					moving = true;
				}

			}
     	}

		if(moving){

			if(selectedPiece != null){
				Vector3 location = selectedTile.transform.position;
				location.y += .05f;
				selectedPiece.transform.position = Vector3.Lerp(selectedPiece.transform.position, location, Time.deltaTime * 3.5f);

				if((selectedPiece.transform.position - location).magnitude < .02){
					selectedPiece.transform.position = location;
					selectedPiece.GetComponent<Renderer>().material.color = selectedPieceColor;
					selectedTile.GetComponent<Renderer>().material.color = selectedTileColor;
					resetForNextAction();
				}
			}

			if(selectedPlatform != null){
				Vector3 location = selectedPlatformLoc.transform.position;

				//adjust height
				location.x -= .5f;
				location.y += (2.2f - .051f);
				location.z -= .5f;

				selectedPlatform.transform.position = Vector3.Lerp(selectedPlatform.transform.position, location, Time.deltaTime * 3.5f);

				if((selectedPlatform.transform.position - location).magnitude < .02){
					selectedPlatform.transform.position = location;
					selectedPlatform.transform.GetChild(4).transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = selectedPlatformColor;
					selectedPlatform.transform.GetChild(4).transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = selectedPlatformColor;
					selectedPlatformLoc.GetComponent<Renderer>().material.color = selectedPlatformLocColor;
					resetForNextAction();
				}
			}
		}
	}


	bool tileAvailable(GameObject tile, GameObject piece){
		Vector3 location = tile.transform.position;

		GameObject wp = GameObject.Find("White Pieces");
		GameObject bp = GameObject.Find("Black Pieces");

		foreach(Transform child in wp.transform){

			if((child.transform.position - location).magnitude < .06){
				if(piece.transform.parent.gameObject == wp){
					Debug.Log("Same team is there.");
					return false;
				} else {
					Debug.Log("Other team is there.");
					return true;
				}

			}
		}

		foreach(Transform child in bp.transform){

			if((child.transform.position - location).magnitude < .06){
				if(piece.transform.parent.gameObject == bp){
					Debug.Log("Same team is there.");
					return false;
				} else {
					Debug.Log("Other team is there.");
					return true;
				}

			}
		}
		return true;
	}
}
