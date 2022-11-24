using UnityEngine;
using System.Collections;

public class ObstacleController : MonoBehaviour {

    private Vector3 dir; //Moving direction of player 
    private GameObject temp;
    void Start()
    {
        dir = Vector3.left;
        temp = GameObject.FindGameObjectsWithTag("ObstacleManager")[0];
        StartCoroutine(MoveObstacle());       
    }

    void Update()
    {


        //If gameover, make speed equal to 15
        if (PlayerController.gameOver)
        {
            temp.GetComponent<ObstaclesManager>().speedObstacleMoving = 20f;
        }

        //While camera rotating, destroy all obstacle in scene
        if (!CameraController.isCameraRotateFinish)
        {
            Destroy(gameObject);
        }
        
    }

    //If obstacle run out of screen, destroy it
    void OnBecameInvisible()
    {
        StopCoroutine(MoveObstacle());
        Destroy(gameObject);
    }

    //Move obstacle with dir(direction) and speed
    IEnumerator MoveObstacle()
    {
        while (true)
        {
            if (CameraController.isCameraRotateFinish)
            {
                gameObject.transform.position = gameObject.transform.position + dir * temp.GetComponent<ObstaclesManager>().speedObstacleMoving * Time.deltaTime;
            }
            yield return null;
        }
    }
}
