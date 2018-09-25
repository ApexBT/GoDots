using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Settings : MonoBehaviour {

	public static int mapSize = 0;
	public static int playUntill = 0;

    public Dropdown FeildSize;

    public Dropdown PlayUntill;

	public void mapSizeSellection(){
		mapSize = FeildSize.value;
		Debug.Log("Map size" + mapSize);
	}

	public void gameTimeSelection(){
		playUntill = PlayUntill.value;
		Debug.Log("We play untill" + playUntill);
	}

}
