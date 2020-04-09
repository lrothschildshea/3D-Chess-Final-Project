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

    bool isDarkPiece(GameObject gamePiece)
    {
        return darkPieces.Contains(gamePiece);
    }
	
	// Update is called once per frame
	void Update () {

		//gets child object by index
		//Debug.Log(transform.GetChild(0).gameObject);



		//detect the object that has been clicked and change its color
		if(Input.GetMouseButtonDown(0) && !moving) {
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			
			if( Physics.Raycast(ray, out hit, 100)){
				Debug.Log( hit.transform.gameObject.name );

                //NEED TO CONFIRM NOT TRYING TO MOVE PIECE OR PLATFORM!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


                //use list of pieces to get actual confirmation
                if (!hit.transform.gameObject.name.Contains("Cube") && selectedPiece == null && !(hit.transform.gameObject.name.Contains("Rod") || hit.transform.gameObject.name.Contains("Platform"))){
					selectedPiece = hit.transform.gameObject;
					selectedPieceColor = hit.transform.gameObject.GetComponent<Renderer>().material.color;
					hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
				}

				if(selectedPiece != null && hit.transform.gameObject.name.Contains("Cube") && !(hit.transform.gameObject.name.Contains("Rod") || hit.transform.gameObject.name.Contains("Platform"))){

					if(tileAvailable(hit.transform.gameObject, selectedPiece)){
						selectedTile = hit.transform.gameObject;
						selectedTileColor = hit.transform.gameObject.GetComponent<Renderer>().material.color;
						hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
						moving = true;
					}

				}

				if(selectedPlatform == null && (hit.transform.gameObject.name.Contains("Rod") || hit.transform.gameObject.name.Contains("Platform"))){
					//DELETE THIS LATER AND FIX ABOVE IF STATEMENT THIS IS REALLY BAD AND I HATE IT.
					selectedPiece = null;


					GameObject parent = hit.transform.gameObject.transform.parent.gameObject;
					selectedPlatformColor = parent.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color;
					parent.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Color.green;
					parent.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = Color.green;
					selectedPlatform = parent.transform.parent.gameObject;
				}

				if(selectedPlatform != null && hit.transform.gameObject.name.Contains("Cylinder")){
					//DELETE THIS LATER AND FIX ABOVE IF STATEMENT THIS IS REALLY BAD AND I HATE IT.
					selectedPiece = null;

					//THE CYLINDERS COLOR IS STILL BROKEN

					selectedPlatformLoc = hit.transform.gameObject;
					selectedPlatformLocColor = hit.transform.gameObject.GetComponent<Renderer>().material.color;
					hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
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

					moving = false;
					selectedPiece = null;
					selectedTile = null;
					selectedPieceColor = Color.red;
					selectedTileColor = Color.red;
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

					moving = false;
					selectedPlatform = null;
					selectedPlatformLoc = null;
					selectedPlatformColor = Color.red;
					selectedPlatformLocColor = Color.red;
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
