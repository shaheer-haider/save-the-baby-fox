using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundManager : MonoBehaviour
{

    public GameObject firstGround;
    public GameObject groundPrefab;
    public GameObject parentObject;
    public UIManager UIManager;
    public CameraController cameraController;
    public bool finishMoveGround = false;  // When all ground finished move up, it's became true
    public int numberOfGround = 7; //Number of ground for forward side and back side


    private float timeToDestroyGround = 0.15f; //When game over, ground will scale with this time
    private List<GameObject> listGroundForWard = new List<GameObject>(); //List ground on right side of first ground
    private List<GameObject> listGroundBack = new List<GameObject>(); //List ground on left side of first ground
    private Vector3 firstForwardPosition; //First right position
    private Vector3 firstBackPosition; //First left position
    private bool finishedRandomGround; //Check finished create ground
    private bool enableCheck = true;
    private bool stopMoveGround = false; //Check ground have moved
    private bool finishedScaleGroundWhenGameOver = false;

    private float timeToMove = 1f;
    // Use this for initialization
    void Start()
    {

        firstForwardPosition = firstGround.transform.position + Vector3.forward * firstGround.transform.localScale.z + new Vector3(0, -10f, 0);
        firstBackPosition = firstGround.transform.position + Vector3.back * firstGround.transform.localScale.z + new Vector3(0, -10f, 0);
        StartCoroutine(RandomGroundOnForward(firstForwardPosition, numberOfGround, listGroundForWard));
        StartCoroutine(RandomGroundOnBack(firstBackPosition, numberOfGround, listGroundBack));
    }

    // Update is called once per frame
    void Update()
    {

        //When all ground is created, start to move ground
        if (finishedRandomGround && !stopMoveGround)
        {
            for (int i = 0; i < listGroundForWard.Count; i++)
            {
                StartCoroutine(MoveGround(listGroundForWard[i], listGroundForWard[i].transform.position, listGroundForWard[i].transform.position + new Vector3(0, 10f, 0), timeToMove));
            }
            for (int i = 0; i < listGroundBack.Count; i++)
            {
                StartCoroutine(MoveGround(listGroundBack[i], listGroundBack[i].transform.position, listGroundBack[i].transform.position + new Vector3(0, 10f, 0), timeToMove));
            }
            stopMoveGround = true;
        }


        //If game over, start to scale and destroy ground
        if (PlayerController.gameOver && !finishedScaleGroundWhenGameOver)
        {
            finishedScaleGroundWhenGameOver = true;
            StartCoroutine(ScaleGroundSmaller(firstGround, 0));
            if (listGroundForWard != null)
            {
                List<GameObject> newList = ListCopyOf(listGroundForWard);
                for (int i = 0; i < newList.Count; i++)
                {
                    GameObject currentGround = newList[i];
                    StartCoroutine(ScaleGroundSmaller(currentGround, timeToDestroyGround * (float)i));
                }
                listGroundForWard.Clear();
            }
            if (listGroundBack != null)
            {
                List<GameObject> newList = ListCopyOf(listGroundBack);
                for (int i = 0; i < newList.Count; i++)
                {
                    GameObject currentGround = newList[i];
                    StartCoroutine(ScaleGroundSmaller(currentGround, timeToDestroyGround * (float)i));
                }
                listGroundBack.Clear();
            }

        }


        //If score not equals 0 and reached scoreToScaleGround and not game over, scale the last and the first ground
        // Scale ground smaller
        if ((ScoreManager.Instance.Score != 0) && (ScoreManager.Instance.Score % UIManager.scoreToScaleGround == 0) && enableCheck && !PlayerController.gameOver)
        {
            enableCheck = false;
            SoundManager.Instance.PlaySound(SoundManager.Instance.scaleGround);
            if (listGroundForWard[listGroundForWard.Count - 1] != null)
            {
                StartCoroutine(ScaleGroundSmaller(listGroundForWard[listGroundForWard.Count - 1], 0f));
            }

            if (listGroundBack[listGroundBack.Count - 1] != null)
            {
                StartCoroutine(ScaleGroundSmaller(listGroundBack[listGroundBack.Count - 1], 0f));
            }
            StartCoroutine(WaitAndEnableCheck());

        }


        //If camera start to rotate, make 2 ground scale back
        //Scale ground bigger
        if (cameraController.startToRotateCamera && enableCheck)
        {
            enableCheck = false;
            if (listGroundForWard[listGroundForWard.Count - 1] != null)
            {
                StartCoroutine(ScaleGroundBigger(listGroundForWard[listGroundForWard.Count - 1], 0f));
            }

            if (listGroundBack[listGroundBack.Count - 1] != null)
            {
                StartCoroutine(ScaleGroundBigger(listGroundBack[listGroundBack.Count - 1], 0f));
            }
            StartCoroutine(WaitAndEnableCheck());
        }
    }

    //Create ground on forward
    IEnumerator RandomGroundOnForward(Vector3 position, int number, List<GameObject> newList)
    {
        finishedRandomGround = false;
        for (int i = 0; i < number; i++)
        {
            GameObject currentGround = (GameObject)Instantiate(groundPrefab, position, Quaternion.identity);
            currentGround.GetComponent<Animator>().Play("ScaleBigger");
            newList.Add(currentGround);
            currentGround.transform.SetParent(parentObject.transform);
            position = currentGround.transform.position + Vector3.forward * currentGround.transform.localScale.z;
            // change scale from 0 to 1
            yield return new WaitForSeconds(0.1f);
        }
        finishedRandomGround = true;
    }


    //Create ground on back
    IEnumerator RandomGroundOnBack(Vector3 position, int number, List<GameObject> newList)
    {

        for (int i = 0; i < number; i++)
        {
            GameObject currentGround = (GameObject)Instantiate(groundPrefab, position, Quaternion.identity);
            currentGround.GetComponent<Animator>().Play("ScaleBigger");
            newList.Add(currentGround);
            currentGround.transform.SetParent(parentObject.transform);
            position = currentGround.transform.position + Vector3.back * currentGround.transform.localScale.z;
            yield return new WaitForSeconds(0.1f);
        }
    }


    //Moving ground
    IEnumerator MoveGround(GameObject ground, Vector3 startpos, Vector3 endPos, float timeToMove)
    {
        float t = 0;
        while (t < timeToMove)
        {
            float fraction = t / timeToMove;
            ground.transform.position = Vector3.Lerp(startpos, endPos, fraction);
            t += Time.deltaTime;
            yield return null;
        }
        ground.transform.position = endPos;
        finishMoveGround = true;
    }

    // Turn animation ScaleSmaller of ground on
    IEnumerator ScaleGroundSmaller(GameObject ground, float time)
    {
        yield return new WaitForSeconds(time);
        ground.GetComponent<Animator>().Play("ScaleSmaller");
    }


    // Turn animation ScaleBigger of ground on
    IEnumerator ScaleGroundBigger(GameObject ground, float time)
    {
        yield return new WaitForSeconds(time);
        ground.GetComponent<MeshRenderer>().enabled = true;
        ground.GetComponent<Animator>().Play("ScaleBigger");
    }


    IEnumerator WaitAndEnableCheck()
    {
        yield return new WaitForSeconds(3f);
        enableCheck = true;
    }

    // Create a copy list
    List<GameObject> ListCopyOf(List<GameObject> listToCopy)
    {
        List<GameObject> newList = new List<GameObject>();
        for (int i = 0; i < listToCopy.Count; i++)
        {
            newList.Add(listToCopy[i]);
        }
        return newList;
    }
}
