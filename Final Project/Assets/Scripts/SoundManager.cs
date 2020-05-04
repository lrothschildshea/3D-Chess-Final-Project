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
    private int counter;

	void Awake () {
		audioSource = GetComponent<AudioSource>();
		currentRoutine= null;
        counter = 0;
	}

	public void playSongAndTitleAfter(AudioClip clip){
        counter++;
        currentRoutine = StartCoroutine(playAndThenLoop(clip, titleSong, counter));
    }

	public void playSongAndBackgroundAfter(AudioClip clip){
        counter++;
        currentRoutine = StartCoroutine(playAndThenLoop(clip, backgroundSong, counter));
    }

	IEnumerator playAndThenLoop(AudioClip song, AudioClip loop, int currentCounter){
		if(audioSource.isPlaying){
			audioSource.Stop();
		}
        audioSource.loop = false;
        audioSource.clip = song;
        audioSource.Play();
        yield return new WaitForSeconds(song.length);
        if (counter == currentCounter){
            audioSource.clip = loop;
            audioSource.Play();
            audioSource.loop = true;
        }
	}
}
