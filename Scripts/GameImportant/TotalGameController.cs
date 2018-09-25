using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFind;

public class TotalGameController : MonoBehaviour {

	public GameObject redDott; // Red Dot Prefab
	public GameObject blueDott; // Blue Dot Prefab
	private GameObject currentDott; // Prefab that is the current place

	public static int mapSize = 32; // This is the map size. It should be devisable by 4

	public GameObject connection; // Connection line between dotts

	public GameObject mapTilePrefabs; // The actuall map prefab that is a 4x4 tile
	public GameObject mapTiles; // The Tile Prefab that containds all of the tiles as childern

	public static bool canPlace = true; // A Bool so that you can not place a bot if something is wrong

	private bool[,] tilemapBlue = new bool[mapSize+2, mapSize+2]; // Creates a bool the size of the map +2 so that you can go arround the Map
	private bool[,] tilemapRed = new bool[mapSize+2, mapSize+2]; // Creates a bool the size of the map +2 so that you can go arround the Map
    private bool[,] placedRedDotts = new bool[mapSize+2, mapSize+2]; // Creates a bool the size of the map +2 so that you can go arround the Map
	private bool[,] placedBlueDotts = new bool[mapSize+2, mapSize+2];

	private Vector3 mousePosition; // Captures the mouse position as a vector
	private float cursorPositionX; // Cursor position X
    private float cursorPositionY; // Cursor position Y

    public int blueDottNumber = 0;
	public int redDottNumber = 0;
	private int dottNumber = 0; // The total amount of Dotts placed

	private List<float> positionsX = new List<float>();
	private List<float> positionsY = new List<float>();
    private List<float> positionsRedX = new List<float>();
    private List<float> positionsRedY = new List<float>();
    private List<float> positionsBlueX = new List<float>();
    private List<float> positionsBlueY = new List<float>();

    private List<float> surroundingDottsX = new List<float>();
    private List<float> surroundingDottsY = new List<float>();

    public static int redScore = 0;
    public static int blueScore = 0;

 
	// Use this for initialization
	void Start () {
		currentDott = redDott; // Make the first dott be Red as Red starts first
		CreateTheVisualPlayingFeild();
		CreateAStarFeild();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0)) // Check if the Mouse is pressed
        {

            // Gets the Position of the Cursor in the World Position
            mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorPositionX = Mathf.Round(mousePosition.x / 4); // Gets the position interms of X and Y
            cursorPositionY = Mathf.Round(mousePosition.y / 4); // Gets the position interms of X and Y
            
            // Checks if a dott has been in that Position Before. If yes then you can not place another dott there
            for (int i = 0; i < dottNumber; i++)
            {
                if (positionsX[i] == cursorPositionX && positionsY[i] == cursorPositionY)
                {
                    canPlace = false;
                    break;
                }
                else
                {
                    canPlace = true;
                }
            }

            // Make sure that the dott can not be placed outside of the map
            if (cursorPositionX < 0 || cursorPositionX > mapSize - 3 || cursorPositionY < 0 || cursorPositionY > mapSize - 3)
            {
                canPlace = false;
            }

            if (canPlace && !PauseMenu.GameIsPaused)
            {
                placeDott();
                checkDotts();
                SurroundDotts(); 
                SwapDotts();
            }
        }
	}


	void CreateTheVisualPlayingFeild(){ 
		for(int x = 0; x <= mapSize; x++ ){
			for(int y = 0; y <= mapSize; y++){
                var new_tile = Instantiate(mapTilePrefabs, new Vector3((x * 16) + 6, (y * 16) + 6, 0), Quaternion.identity);
                new_tile.transform.parent = mapTiles.transform;
                new_tile.name = "Tile" + x + "_" + y;
			}
		}
	} // Makes the Visual Playing Feild
 	void SwapDotts(){
		if(currentDott == redDott){
			currentDott = blueDott;
		}else{
			currentDott = redDott;
		}
	} // Swap the Dotts

	void CreateAStarFeild(){
		// Makes every tile at the start of the Game positive for A*
        for (int x = 0; x < mapSize+2; x++) {
			for (int y = 0; y < mapSize+2; y++) {
				tilemapBlue [x, y] = true;
				tilemapRed [x, y] = true;
                placedBlueDotts [x,y] = true;
                placedRedDotts [x,y] = true;
			}
		}
	} // Creates a Playing Feild just for the algorighm and then makes all of the nodes positive because it is a new Game

    void checkDotts(){
        if (currentDott == redDott) { // See if the Current Dott is a Red Dott
			PathFind.Grid grid = new PathFind.Grid (mapSize+2, mapSize+2, tilemapRed); // Creates a new grid the size of the map
			for (int i = 0; i < blueDottNumber;) {
				PathFind.Point _from = new PathFind.Point (Mathf.RoundToInt (positionsBlueX [i])+1, Mathf.RoundToInt (positionsBlueY [i])+1); // Sets the "From" to a blue possition
				PathFind.Point _to = new PathFind.Point (0, 0); // Sets the goal to 0,0
				List<PathFind.Point> path = PathFind.Pathfinding.FindPath (grid ,_from, _to);

				if (path.Count == 0) {
                    int positionsX = Mathf.RoundToInt(positionsBlueX[i]);
                    int positionsY = Mathf.RoundToInt(positionsBlueY[i]);
                    findSurroundingDotts(positionsX, positionsY);
                    GameObject go = GameObject.Find("Dott_Blue_" + positionsBlueX[i] + "_" + positionsBlueY[i]);
                    //go.SetActive(false);
                    GameObject.Destroy(go);
                    redScore++;

                    if(GameObject.Find("Connection_Blue_Left_" + positionsBlueX[i] + "_" + positionsBlueY[i])){
                        GameObject right = GameObject.Find("Connection_Blue_Left_" + positionsBlueX[i] + "_" + positionsBlueY[i]);
                        GameObject.Destroy(right);                      
                    }

                    if(GameObject.Find("Connection_Blue_Up_" + positionsBlueX[i] + "_" + positionsBlueY[i])){
                        GameObject right = GameObject.Find("Connection_Blue_Up_" + positionsBlueX[i] + "_" + positionsBlueY[i]);
                        GameObject.Destroy(right);                      
                    }

                    if(GameObject.Find("Connection_Blue_Down_" + positionsBlueX[i] + "_" + positionsBlueY[i])){
                        GameObject left = GameObject.Find("Connection_Blue_Down_" + positionsBlueX[i] + "_" + positionsBlueY[i]);
                        GameObject.Destroy(left);                      
                    }

                    if(GameObject.Find("Connection_Blue_Right_" + positionsBlueX[i] + "_" + positionsBlueY[i])){
                        GameObject left = GameObject.Find("Connection_Blue_Right_" + positionsBlueX[i] + "_" + positionsBlueY[i]);
                        GameObject.Destroy(left);                      
                    }
                        

                    tilemapBlue[Mathf.RoundToInt(positionsBlueX[i])+1, Mathf.RoundToInt(positionsBlueY[i])+1] = true;
                    positionsBlueX.RemoveAt(i);
                    positionsBlueY.RemoveAt(i);
                    blueDottNumber--;
				}
                else
                {   
                    i++;
                }
			}

		} else { // See if the Current Dott is a Blue Dott
			PathFind.Grid grid = new PathFind.Grid (mapSize+2, mapSize+2, tilemapBlue); // Creates a new grid the size of the map
			for (int i = 0; i < redDottNumber;) {
				PathFind.Point _from = new PathFind.Point (Mathf.RoundToInt (positionsRedX [i])+1, Mathf.RoundToInt (positionsRedY [i])+1); // Sets the "Red" to a blue possition
				PathFind.Point _to = new PathFind.Point (0, 0); // Sets the goal to 0,0
				List<PathFind.Point> path = PathFind.Pathfinding.FindPath (grid ,_from, _to);

				if (path.Count == 0) {
                    int positionsX = Mathf.RoundToInt(positionsRedX[i]);
                    int positionsY = Mathf.RoundToInt(positionsRedY[i]);
                    findSurroundingDotts(positionsX, positionsY);
                    GameObject go = GameObject.Find("Dott_Red_" + positionsRedX[i] + "_" + positionsRedY[i]);
                    //go.SetActive(false);
                    GameObject.Destroy(go);
                    blueScore++;

                    if(GameObject.Find("Connection_Red_Left_" + positionsRedX[i] + "_" + positionsRedY[i])){
                        GameObject right = GameObject.Find("Connection_Red_Left_" + positionsRedX[i] + "_" + positionsRedY[i]);
                        GameObject.Destroy(right);                      
                    }

                    if(GameObject.Find("Connection_Red_Down_" + positionsRedX[i] + "_" + positionsRedY[i])){
                        GameObject right = GameObject.Find("Connection_Red_Down_" + positionsRedX[i] + "_" + positionsRedY[i]);
                        GameObject.Destroy(right);                      
                    }

                    if(GameObject.Find("Connection_Red_Up_" + positionsRedX[i] + "_" + positionsRedY[i])){
                        GameObject right = GameObject.Find("Connection_Red_Up_" + positionsRedX[i] + "_" + positionsRedY[i]);
                        GameObject.Destroy(right);                      
                    }

                    if(GameObject.Find("Connection_Red_Right_" + positionsRedX[i] + "_" + positionsRedY[i])){
                        GameObject left = GameObject.Find("Connection_Red_Right_" + positionsRedX[i] + "_" + positionsRedY[i]);
                        GameObject.Destroy(left);                      
                    }
                    
                    
                    tilemapRed[Mathf.RoundToInt(positionsRedX[i])+1, Mathf.RoundToInt(positionsRedY[i])+1] = true;
                    positionsRedX.RemoveAt(i);
                    positionsRedY.RemoveAt(i);
                    redDottNumber--;
                }
                else
                {
                    i++;
                }
			}

		}
    }

    void placeDott(){
        if (currentDott == redDott)
        {
            GameObject go = GameObject.Find("RedDotts"); // Adds Red Dotts as children to the RedDott prefab
            var NewDott = Instantiate(redDott, new Vector3(cursorPositionX * 4, cursorPositionY * 4, -.5f), Quaternion.identity); // Instantiates the RedDotts
            NewDott.transform.parent = go.transform; // Transforms Dott to the Parent
            NewDott.name = "Dott_Red_" + cursorPositionX + "_" + cursorPositionY;

            positionsRedX.Add(cursorPositionX); // Adds to the Red Positions
            positionsRedY.Add(cursorPositionY);

            placedRedDotts[Mathf.RoundToInt(cursorPositionX) + 1, Mathf.RoundToInt(cursorPositionY) + 1] = false;

            redDottNumber++;

            tilemapRed[Mathf.RoundToInt(cursorPositionX) + 1, Mathf.RoundToInt(cursorPositionY) + 1] = false; // Makes Red Dotts False on a Red Tile map
            }
            else
            {
            GameObject go = GameObject.Find("BlueDotts"); // Adds Blue Dotts as children to the BlueDott prefab
            var NewDott = Instantiate(blueDott, new Vector3(cursorPositionX * 4, cursorPositionY * 4, -.5f), Quaternion.identity); // Instantiates the Blue Dotts
            NewDott.transform.parent = go.transform; // Transforms Dotts to the Parent
            NewDott.name = "Dott_Blue_" + cursorPositionX + "_" + cursorPositionY;

            positionsBlueX.Add((cursorPositionX)); // Adds to the Blue Positions
            positionsBlueY.Add((cursorPositionY));

            placedBlueDotts[Mathf.RoundToInt(cursorPositionX) + 1, Mathf.RoundToInt(cursorPositionY) + 1] = false;

            blueDottNumber++;

            tilemapBlue[Mathf.RoundToInt(cursorPositionX) + 1, Mathf.RoundToInt(cursorPositionY) + 1] = false; // Makes Blue Dotts False on a blue Tile map
            }

        positionsX.Add(cursorPositionX); // Adds total positions so that you can tell what Xs and Ys are taken
        positionsY.Add(cursorPositionY);
        canPlace = false;
        dottNumber++;
    }

    void findSurroundingDotts(int trappedDottX, int trappedDottY){
        if(currentDott == redDott){
            bool clear = true;
            //Debug.Log(trappedDottX+ " trapped " + trappedDottY);
            PathFind.Grid grid = new PathFind.Grid (mapSize+2, mapSize+2, placedRedDotts); // Creates a new grid the size of the map
            for(int i = 0; i < redDottNumber; i++){
                for(int x = 0; x < surroundingDottsX.Count; x++){
                    if(surroundingDottsX[x] == positionsRedX[i] && surroundingDottsY[x] == positionsRedY[i]){
                        clear = false;
                    }
                }
                PathFind.Point _from = new PathFind.Point (Mathf.RoundToInt (positionsRedX [i])+1, Mathf.RoundToInt (positionsRedY [i])+1);
			    PathFind.Point _to = new PathFind.Point (trappedDottX+1, trappedDottY+1);
		        List<PathFind.Point> path = PathFind.Pathfinding.FindPath (grid ,_from, _to);
                if(path.Count != 0 && clear){
                    surroundingDottsX.Add(positionsRedX[i]);
                    surroundingDottsY.Add(positionsRedY[i]);
                }else{
                    clear = true;
                }
            }
        }else{
            bool clear = true;
            //Debug.Log(trappedDottX+ " trapped " + trappedDottY);
            PathFind.Grid grid = new PathFind.Grid (mapSize+2, mapSize+2, placedBlueDotts); // Creates a new grid the size of the map
            for(int i = 0; i < blueDottNumber; i++){
                for(int x = 0; x < surroundingDottsX.Count; x++){
                    if(surroundingDottsX[x] == positionsBlueX[i] && surroundingDottsY[x] == positionsBlueY[i]){
                        clear = false;
                    }
                }
                PathFind.Point _from = new PathFind.Point (Mathf.RoundToInt (positionsBlueX [i])+1, Mathf.RoundToInt (positionsBlueY [i])+1);
			    PathFind.Point _to = new PathFind.Point (trappedDottX+1, trappedDottY+1);
		        List<PathFind.Point> path = PathFind.Pathfinding.FindPath (grid ,_from, _to);
                if(path.Count != 0 && clear){
                    surroundingDottsX.Add(positionsBlueX[i]);
                    surroundingDottsY.Add(positionsBlueY[i]);
                }else{
                    clear = true;
                }
            }
        }
    }

    private void SurroundDotts(){

            for(int i = 0; i < surroundingDottsX.Count;){
                GameObject go = GameObject.Find("Connections");

                if(currentDott == redDott){
                    if(GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] + 1))){ // To the Top
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0, -90))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Red_Up_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }
                     if(GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i] - 1) + "_" + (surroundingDottsY[i] + 1)) && !GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i] - 1) + "_" + (surroundingDottsY[i])) && !GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] + 1))){ // Top Corner to the Left
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0, -45))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Red_Left_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                    if(GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i] - 1) + "_" + (surroundingDottsY[i])) && !GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i] - 1) + "_" + (surroundingDottsY[i] + 1)) && GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] + 1))){ // To the Left Corner Full Down
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0, 0))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Red_Left_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                    if(GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i] - 1) + "_" + (surroundingDottsY[i])) && !GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] + 1)) && !GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] - 1))){ // Top and Bottom Down Full
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0, 0))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Red_Left_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                    if(GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] - 1))){ // To the Bottom
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0,-270))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Red_Down_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                    if(GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i] + 1) + "_" + (surroundingDottsY[i])) && (GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i] + 1) + "_" + (surroundingDottsY[i] + 1)) || GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i] + 1) + "_" + (surroundingDottsY[i] - 1)))){ // Bottom to the Right
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0,-180))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Red_Right_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                    if(GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i] + 1) + "_" + (surroundingDottsY[i] + 1)) && !GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i] + 1) + "_" + (surroundingDottsY[i])) && !GameObject.Find("RedDotts/Dott_Red_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] + 1))){ // Top Corner to the Right
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0, -135))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Red_Right_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                }else{
                    
                    if(GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] + 1))){ // To the Top
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0, -90))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Blue_Up_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }
                     if(GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i] - 1) + "_" + (surroundingDottsY[i] + 1)) && !GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i] - 1) + "_" + (surroundingDottsY[i])) && !GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] + 1))){ // Top Corner to the Left
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0, -45))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Blue_Left_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                     if(GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i] - 1) + "_" + (surroundingDottsY[i])) && !GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i] - 1) + "_" + (surroundingDottsY[i] + 1)) && GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] + 1))){ // Left Across
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0, 0))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Blue_Left_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                    if(GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i] - 1) + "_" + (surroundingDottsY[i])) && !GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] + 1)) && !GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] - 1))){ // Top and Bottom Down Full
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0, 0))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Blue_Left_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                    if(GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] - 1))){ // To the Bottom
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0,-270))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Blue_Down_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                    if(GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i] + 1) + "_" + (surroundingDottsY[i])) && (GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i] + 1) + "_" + (surroundingDottsY[i] + 1)) || GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i] + 1) + "_" + (surroundingDottsY[i] - 1)))){ // Bottom to the Right
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0,-180))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Blue_Right_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }

                    if(GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i] + 1) + "_" + (surroundingDottsY[i] + 1)) && !GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i] + 1) + "_" + (surroundingDottsY[i])) && !GameObject.Find("BlueDotts/Dott_Blue_" + (surroundingDottsX[i]) + "_" + (surroundingDottsY[i] + 1))){ // Top Corner to the Right
                        var NewConnection = Instantiate(connection, new Vector3(surroundingDottsX[i] * 4, surroundingDottsY[i] * 4, -.5f),  Quaternion.Euler(new Vector3(0, 0, -135))); 
                        NewConnection.transform.parent = go.transform;
                        NewConnection.name = "Connection_Blue_Right_" + surroundingDottsX[i] + "_" + surroundingDottsY[i];
                    }
                
                }

            surroundingDottsX.Remove(surroundingDottsX[i]);
            surroundingDottsY.Remove(surroundingDottsY[i]);
        }
    }
    
}
