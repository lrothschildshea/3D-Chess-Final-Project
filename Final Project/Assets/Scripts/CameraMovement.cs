using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	private float cameraVerticalSpeed = 8f;
	private float cameraHorizontalSpeed = 40f;
	private float zoomSpeed = 40f;
	private float minZoomDist = 8f;
	private float maxZoomDist = 25f;
	private float minCameraHeight = 12f;
	private float maxCameraHeight = 30f;

	private int count = 0;

	private GameLogic mainScript;

	private GameObject mainCamera;

	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find("Main Camera");
		mainCamera.transform.LookAt(transform);
		mainScript = GameObject.Find("GameBoard").GetComponent<GameLogic>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!mainScript.gameStarted){
			return;
		}

		if(mainScript.paused){
			return;
		}

		if(mainScript.lockCamera){
			count++;
			if(count > 1){
				count = 0;
				mainScript.lockCamera = false;
			}
			return;
		}

		//camera rotation
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + cameraHorizontalSpeed*Time.deltaTime, transform.eulerAngles.z);
		}
		if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - cameraHorizontalSpeed*Time.deltaTime, transform.eulerAngles.z);
		}
		
		//camera height
		if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && transform.position.y < maxCameraHeight){
			transform.position = new Vector3(transform.position.x, transform.position.y + cameraVerticalSpeed*Time.deltaTime, transform.position.z);
		}
		if((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && transform.position.y > minCameraHeight){
			transform.position = new Vector3(transform.position.x, transform.position.y - cameraVerticalSpeed*Time.deltaTime, transform.position.z);
		}

		//camera zoom
		if (Input.GetAxis("Mouse ScrollWheel") > 0f ){
			if((mainCamera.transform.position - transform.position).magnitude > minZoomDist){
				mainCamera.transform.position = mainCamera.transform.position + mainCamera.transform.forward*zoomSpeed*Time.deltaTime;
			}
		} else if (Input.GetAxis("Mouse ScrollWheel") < 0f ){
			if((mainCamera.transform.position - transform.position).magnitude < maxZoomDist){
				mainCamera.transform.position = mainCamera.transform.position - mainCamera.transform.forward*zoomSpeed*Time.deltaTime;
			}
		}
	}
}
