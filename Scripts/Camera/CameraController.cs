using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public Camera camera;
    public float panSpeed = 2;
    private Vector3 dragOrigin;
    private Vector3 oldPos;

    public float zoomMin;
    public float zoomMax;
    private float zoomAmount = 1f;
    private float moveSmooth = 0.1f;

    public float mapSize;

    public float x, y, width, height;


    ////////
    // Awake
    void Awake()
    {
        // DontDestroyOnLoad( this ); if( FindObjectsOfType( GetType() ).Length > 1 ) Destroy( gameObject );
        ResetCamera();
    }

    ////////
    // Start
    void Start()
    {
        Camera.main.transform.position = new Vector3(25, 25, -1);
        ResetCamera();
    }

    ////////////////
    // RESET: Camera
    public void ResetCamera()
    {
        if (camera == null) camera = Camera.main;
        //// Camera Origin
        x = 0; y = 0;
        height = 2f * camera.orthographicSize;
        width = height * camera.aspect;
        camera.rect = new Rect(x, y, width, height);
    }

    /////////
    // Update
    void Update()
    {
        if (!PauseMenu.GameIsPaused)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                //// ZOOM: in
                camera.orthographicSize = (camera.orthographicSize > zoomMin)
                  ? camera.orthographicSize - zoomAmount
                  : zoomMin;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                //// ZOOM: out
                camera.orthographicSize = (camera.orthographicSize < zoomMax)
                  ? camera.orthographicSize + zoomAmount
                  : zoomMax;
            };

            //// Moving of camera via click / drag
            if (Input.GetMouseButtonDown(1))
            {
                dragOrigin = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                dragOrigin = camera.ScreenToWorldPoint(dragOrigin);
            };

            if (Input.GetMouseButton(1))
            {
                Vector3 currentPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                currentPos = camera.ScreenToWorldPoint(currentPos);
                Vector3 movePos = dragOrigin - currentPos;
                transform.position += (movePos * moveSmooth);

                if (Camera.main.transform.position.x <= -5)
                {
                    Camera.main.transform.position = new Vector3(-5, Camera.main.transform.position.y, -1);
                }

                if (Camera.main.transform.position.x >= 72)
                {
                    Camera.main.transform.position = new Vector3(72, Camera.main.transform.position.y, -1);
                }

                if (Camera.main.transform.position.y <= -5)
                {
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, -5, -1);
                }

                if (Camera.main.transform.position.y >= 72)
                {
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 72, -1);
                }
            }
        }
        
    }

}