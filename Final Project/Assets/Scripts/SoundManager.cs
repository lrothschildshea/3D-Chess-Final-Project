using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioClip titleSong;
	public AudioClip backgroundSong;
	public AudioClip captureSound;
	public AudioClip redAlertSound;
	public AudioClip upgradeSong;
	public AudioClip winSong;
	public AudioClip loseSong;
	public AudioClip drawSong;
	internal AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	void Awake(){
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void playSongAndTitleAfter(AudioClip clip){
		StartCoroutine(playAndThenLoop(clip, titleSong));
	}

	public void playSongAndBackgroundAfter(AudioClip clip){
		StartCoroutine(playAndThenLoop(clip, backgroundSong));
	}

	IEnumerator playAndThenLoop(AudioClip song, AudioClip loop){
		audioSource.loop = false;
		audioSource.clip = song;
		audioSource.Play();
		yield return new WaitForSeconds(song.length);
		audioSource.clip = loop;
		audioSource.Play();
		audioSource.loop = true;
	}
}
