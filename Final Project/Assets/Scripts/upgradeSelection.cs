﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class upgradeSelection : MonoBehaviour {

	private GameLogic mainScript;
	private MenuControls menuScript;

	// Use this for initialization
	void Start () {
		mainScript = GameObject.Find("GameBoard").GetComponent<GameLogic>();
		menuScript = GameObject.Find("Menus").GetComponent<MenuControls>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0.0f, 10.0f*Time.deltaTime, 0.0f, Space.Self);

			if(Input.GetMouseButtonDown(0)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, 100)){
					GameObject clicked = hit.transform.gameObject;
					if(clicked == gameObject){
						mainScript.upgradeSelection = gameObject.name;
						menuScript.enableHUD();
					}
				}
			}
	}
}
