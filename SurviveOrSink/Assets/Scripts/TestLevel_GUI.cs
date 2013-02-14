using UnityEngine;
using System.Collections;

public class TestLevel_GUI : MonoBehaviour {
	
	void OnGUI() {
		GUI.Box(new Rect(Screen.width-300,0,300,200),"Chat Window");
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
