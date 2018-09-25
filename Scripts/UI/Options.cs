using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour {

    public GameObject MenuGO;
    public GameObject OptionsGO;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuGO.SetActive(true);
            OptionsGO.SetActive(false);
        }
    }
}
