using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

	public Text redScoreUI;
	public Text blueScoreUI;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		redScoreUI.text = TotalGameController.redScore.ToString();
		blueScoreUI.text = TotalGameController.blueScore.ToString();
	}
}
