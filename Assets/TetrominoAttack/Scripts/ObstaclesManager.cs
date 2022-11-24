using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstaclesManager : MonoBehaviour
{

    public GroundManager groundManager;
    public PlayerController playerController;
    public CameraController cameraController;
    public GameObject obstaclesManager;
    public GameObject[] arrayObstacles; //Array of obstacle
    public GameObject item; //item
    [Range(0f, 1f)]
    public float itemFrequency; //Probabilities to create item, if == 1, item will be create every time RandomObstacle function have called, else item will not create
    public float timeToWaitToCreateObstacle = 3f; //Time to wait to create obstacle
    public float timeDecreaseWhenCameraRotate = 1f; //When camera rotate, timeToWaitToCreateObstacle will decrease by this value
    public float speedItemMoving = 4f; //Moving speed of item
    public float speedObstacleMoving = 4f;//Moving of obstacle
    public float speedItemMovingIncrease = 1.5f; //When camera rotate, speed of item will increase by this value
    public float speedObstacleMovingIncrease = 1.5f;//When camera rotate, speed of obstacle will increase by this value

    private List<Vector3> listPosition = new List<Vector3>(); //List position to create obstacle 
    private bool enableCheck = true;
    // Use this for initialization
    void Start()
    {


        //Create list position to create obstacle base on number of ground;
        int i = -groundManager.numberOfGround;
        while (i <= groundManager.numberOfGround)
        {
            Vector3 pos = obstaclesManager.transform.position + new Vector3(0, 0, i);
            listPosition.Add(pos);
            i = i + 2;
        }

        StartCoroutine(RandomObstacle(timeToWaitToCreateObstacle));
    }

    // Update is called once per frame
    void Update()
    {

        //Decrease timeToWaitToRandomObstacle every time camera rotate, that mean number of obstacle will increase
        if (cameraController.startToRotateCamera && enableCheck)
        {
            enableCheck = false;
            timeToWaitToCreateObstacle = timeToWaitToCreateObstacle - timeDecreaseWhenCameraRotate;
            speedItemMoving += speedItemMovingIncrease;
            speedObstacleMoving += speedObstacleMovingIncrease;
            if (timeToWaitToCreateObstacle <= 1f)
            {
                timeToWaitToCreateObstacle = 1f;
            }
            StartCoroutine(WaitAndEnableCheck());
        }
    }

    IEnumerator RandomObstacle(float time)
    {
        yield return new WaitForSeconds(time);

        while (true)
        {
            if (PlayerController.hasStarted)
            {
                //If ground is moved, not game over, finished rotate camera --> start to random obstacles
                if (groundManager.finishMoveGround && !PlayerController.gameOver && !cameraController.startToRotateCamera)
                {
                    int indexArrayObstacle = Random.Range(0, arrayObstacles.Length); //Random obstacle in arrayObstacle
                    int indexPositionOfObstacle = Random.Range(0, listPosition.Count); //Random position to create obstacle



                    float itemIndex = Random.Range(0f, 1f); // Random index to create item (currently we only have 1 item: gold)
                    if (itemIndex <= itemFrequency)
                    {
                        int indexPositionOfItem = Random.Range(0, listPosition.Count); //Random position to create item
                        while (indexPositionOfItem == indexPositionOfObstacle)
                        {
                            indexPositionOfItem = Random.Range(0, listPosition.Count); //Random another position to create item
                        }
                        Instantiate(item, listPosition[indexPositionOfItem], Quaternion.identity);
                    }
                    GameObject currentObstacle = (GameObject)Instantiate(arrayObstacles[indexArrayObstacle], listPosition[indexPositionOfObstacle], Quaternion.identity);
                    currentObstacle.transform.SetParent(obstaclesManager.transform);
                    yield return new WaitForSeconds(timeToWaitToCreateObstacle);
                }
            }
            yield return null;
        }
    }

    IEnumerator WaitAndEnableCheck()
    {
        yield return new WaitForSeconds(1.5f);
        enableCheck = true;
    }


}
