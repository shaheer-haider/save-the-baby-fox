using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public GroundManager groundManager;
    public CameraController cameraController;
    public TextMeshProUGUI score;
    public TextMeshProUGUI bestScore;
    public TextMeshProUGUI gold;

    public Image muteButton;
    public Image unMuteButton;
    public GameObject replayButton;
    public int scoreToScaleGroundIncrease = 15;
    public int scoreToRotateCameraIncrease = 20;


    /* IMPORTANT: You cant make two this values are equal or divisible by the same values ( for example: 10 and 20, 15 and 25, 40 and 40), it will cause errors in the game*/
    public int scoreToScaleGround = 15; //When player reached this score, the last ground and the first will be scale and destroy
    public int scoreToRotateCamera = 20; //When you reached this score, camare will rotate

    [SerializeField] InterstitialAd interstitialAd;

    private bool enableCheck = true;
    // Use this for initialization
    void Start()
    {
        ScoreManager.Instance.Reset();
        muteButton.enabled = false;
        unMuteButton.enabled = false;
        // replayButton.enabled = false;
        replayButton.SetActive(false);
        StartCoroutine(CountScore());
    }

    // Update is called once per frame
    void Update()
    {
        score.text = ScoreManager.Instance.Score.ToString();
        bestScore.text = ScoreManager.Instance.HighScore.ToString();
        gold.text = CoinManager.Instance.Coins.ToString();
        if (cameraController.startToRotateCamera && enableCheck) //If start rotate camera, increase score to scale ground and score to rotate camera
        {
            enableCheck = false;
            scoreToScaleGround += scoreToScaleGroundIncrease;
            scoreToRotateCamera += scoreToRotateCameraIncrease;
            StartCoroutine(WaitAndEnableCheck());
        }

        if (PlayerController.gameOver)
        {
            Invoke("EnableButton", 1.5f);
        }
    }


    //If ground is created and moved, start to count score
    IEnumerator CountScore()
    {
        while (true)
        {
            if (groundManager.finishMoveGround && !PlayerController.gameOver && !cameraController.startToRotateCamera && PlayerController.hasStarted)
            {
                ScoreManager.Instance.AddScore(1);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator WaitAndEnableCheck()
    {
        yield return new WaitForSeconds(2f);
    }

    public void SoundClick()
    {
        if (SoundManager.Instance.IsMuted())
        {
            unMuteButton.enabled = true;
            muteButton.enabled = false;
            SoundManager.Instance.ToggleMute();
        }
        else
        {
            unMuteButton.enabled = false;
            muteButton.enabled = true;
            SoundManager.Instance.ToggleMute();
        }
        SoundManager.Instance.PlaySound(SoundManager.Instance.hitButton);
    }

    void EnableButton()
    {
        replayButton.SetActive(true);
        if (SoundManager.Instance.IsMuted())
        {
            muteButton.enabled = true;
            unMuteButton.enabled = false;
        }
        else
        {
            muteButton.enabled = false;
            unMuteButton.enabled = true;
        }
    }

    public void ReplayButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.hitButton);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
