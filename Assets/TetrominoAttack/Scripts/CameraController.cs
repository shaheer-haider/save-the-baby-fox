using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public PlayerController playerController;
    public UIManager UIManager;
    public Transform target; // Camare rotate around this object
    public Camera cam;
    [HideInInspector]
    public bool startToRotateCamera = false; // Check started rotate camera
    public static bool isCameraRotateFinish = true; //Check finished rotate camera
    public float rotateSpeed = 90f; // How fast camera rotate
    public Color[] colors; //Background colors
   


    private bool enableCheck = true;
    private const float rotateAngle = 90f; // Angle rotate
    private float timToEnableCheck = 5f;
	// Use this for initialization
	void Start () {
        int indexColor = Random.Range(0, colors.Length);
        cam.backgroundColor = colors[indexColor];


        cam.transform.LookAt(target.transform.position); // Make camera always look at target
        StartCoroutine(RotateCamera());
       
	}
	
	// Update is called once per frame
	void Update () {
       
	}


    IEnumerator RotateCamera()
    {
        while (true)
        {
            // Camera rotate available when score reached 20,40,60....... and player not die
            if ((ScoreManager.Instance.Score != 0) && (ScoreManager.Instance.Score % UIManager.scoreToRotateCamera == 0) && !PlayerController.gameOver && enableCheck)
            {
                startToRotateCamera = true;
                isCameraRotateFinish = false;
                enableCheck = false;             
                SoundManager.Instance.PlaySound(SoundManager.Instance.cameraRotate);             
                playerController.touchDisable = true; //Disable touch screen 
                float currentAngle = 0f;
                //Start rotate
                while (currentAngle < rotateAngle)
                {
                    cam.transform.RotateAround(target.transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
                    currentAngle += rotateSpeed * Time.deltaTime;
                    yield return null;
                }
                playerController.touchDisable = false; //Enable touch screen
                startToRotateCamera = false;
                isCameraRotateFinish = true;
                StartCoroutine(WaiAndEnableCheck(timToEnableCheck));
            }
            yield return null;         
        }
    }

    IEnumerator WaiAndEnableCheck(float time)
    {
        yield return new WaitForSeconds(time);
        enableCheck = true;
    }
    
}
