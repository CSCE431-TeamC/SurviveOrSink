using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Sounds : MonoBehaviour {
	public AudioClip explode_sound;
	public AudioClip miss_sound;
	public static bool playHit = false;
	public static bool playMiss = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (playHit) {
			audio.PlayOneShot(explode_sound);
			playHit = false;
		}
		if (playMiss) {
			audio.PlayOneShot(miss_sound);
			playMiss = false;
		}
	}
}
