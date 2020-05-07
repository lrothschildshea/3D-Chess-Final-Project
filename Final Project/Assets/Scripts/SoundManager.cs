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
	public AudioClip pieceSound;
	public AudioClip standSound;
	internal AudioSource audioSource;
    private int counter;

	void Awake () {
		audioSource = GetComponent<AudioSource>();
        counter = 0;
	}

	public void playSongAndTitleAfter(AudioClip clip){
        counter++;
		if(clip == winSong){
        	StartCoroutine(playAndThenLoop(clip, titleSong, counter, 0.7F, 1F));
		} else {
        	StartCoroutine(playAndThenLoop(clip, titleSong, counter, 1F, 1F));
		}
    }

	public void playSongAndBackgroundAfter(AudioClip clip){
        counter++;
        StartCoroutine(playAndThenLoop(clip, backgroundSong, counter, 0.5F, 0.5F));
    }

	IEnumerator playAndThenLoop(AudioClip song, AudioClip loop, int currentCounter, float volume1, float volume2){
		if(audioSource.isPlaying){
			audioSource.Stop();
		}
        audioSource.loop = false;
        audioSource.clip = song;
		audioSource.volume = volume1;
        audioSource.Play();
		yield return new WaitForSeconds(song.length);
        if (counter == currentCounter){
            audioSource.clip = loop;
			audioSource.volume = volume2;
            audioSource.Play();
            audioSource.loop = true;
        }
	}
}
