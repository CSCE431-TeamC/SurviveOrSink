using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Sounds : MonoBehaviour {
	public AudioClip explode_sound;
	public static bool play;

	// Use this for initialization
	void Start () {
		play = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		
		if (play) {
			audio.PlayOneShot(explode_sound);
			play = false;
		}
	}
}
