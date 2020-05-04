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
	private Coroutine currentRoutine;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		currentRoutine= null;
	}

	void Awake () {
		audioSource = GetComponent<AudioSource>();
		currentRoutine= null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void playSongAndTitleAfter(AudioClip clip){
		Debug.Log("This sucks");
		if(currentRoutine != null){
			StopCoroutine(currentRoutine);
		}
		
		currentRoutine = StartCoroutine(playAndThenLoop(clip, titleSong));
	}

	public void playSongAndBackgroundAfter(AudioClip clip){
		Debug.Log("This is good");
		if(currentRoutine != null){
			StopCoroutine(currentRoutine);
		}
		currentRoutine = StartCoroutine(playAndThenLoop(clip, backgroundSong));
	}

	IEnumerator playAndThenLoop(AudioClip song, AudioClip loop){
		if(audioSource.isPlaying){
			audioSource.Stop();
		}

		audioSource.loop = false;
		audioSource.clip = song;
		Debug.Log("This asdfghijksadjkl  " + song.name);
		audioSource.Play();
		yield return new WaitForSeconds(song.length);
		Debug.Log("This sucks  " + loop.name);
		audioSource.clip = loop;
		audioSource.Play();
		audioSource.loop = true;
	}
}
