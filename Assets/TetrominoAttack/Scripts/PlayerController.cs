using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public GameObject player;
    public GroundManager groundManager;
    public CameraController cameraController;
    public ParticleSystem particle; //This particle array will corresponding with each item on ObstaclesManager
    public bool touchDisable; // Disable touch
    public float movingSpeedOfPlayer = 7; // Moving speed of player
    public float movingSpeedIncreaseOfPlayer = 0.7f; //When camera rotate, speed of player will increase by this value
    public float speedPlayerFalling = 20f; // When player run out of ground, player will falling down
    public static bool gameOver; // Check game over

    private Vector3 dir; //Moving direction of player 
    private float dirTurn; // If dirTurn < 0, player will run ahead, else player will run back 
    private bool isGameOverSoundPlay = false;
    private bool enableCheck = true;
    public static bool hasStarted;
    private Animator anim;
    public GameObject tapToPlayText; 

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        gameOver = false;
        touchDisable = false;
        dirTurn = 1;
        hasStarted = false;
        StartCoroutine(MovePlayer());
    }

    // Update is called once per frame
    void Update()
    {

        //Touch available if finish move ground and player not die
        // Touch not available when camera rotating
        // Player will redirected every touch
        if (Input.GetMouseButtonDown(0) && groundManager.finishMoveGround && !touchDisable && !InterstitialAd.isAddRunning)
        {
            if (hasStarted)
            {
                anim.SetTrigger("Roll");
            }
            SoundManager.Instance.PlaySound(SoundManager.Instance.click);
            dirTurn = dirTurn * (-1);

            if (dirTurn < 0)
            {
                dir = Vector3.forward;
                player.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                player.transform.rotation = Quaternion.Euler(0, 180, 0);
                dir = Vector3.back;
            }
            if (!hasStarted)
            {
                tapToPlayText.SetActive(false);
                hasStarted = true;
                anim.SetTrigger("Run");
            }
        }


        //Check player is on ground or not, if player not on ground, it will falling down
        RaycastHit hit;
        Ray rayDown = new Ray(player.transform.position, Vector3.down);
        if (!Physics.Raycast(rayDown, out hit, 0.5f))
        {
            if (!isGameOverSoundPlay)
            {
                isGameOverSoundPlay = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
            }
            // GAME OVER--------------------------------
            gameOver = true;
            anim.SetTrigger("Death");
            touchDisable = true;
            player.GetComponent<Rigidbody>().isKinematic = false;
            movingSpeedOfPlayer = speedPlayerFalling;
            dir = Vector3.down;
        }

        //If start rotate camera, increase speed of player
        if (cameraController.startToRotateCamera && enableCheck)
        {
            enableCheck = false;
            movingSpeedOfPlayer += movingSpeedIncreaseOfPlayer;
            StartCoroutine(WaitAndEnableCheck(2f));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        print("I collided");
        //If player hit obstacle, game over
        if (other.tag == "Obstacle")
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.playerHitObstacle);
            touchDisable = true;
            gameOver = true;
            dir = Vector3.left;
        }


        //If player hit item, create particle and play, destroy item
        if (other.tag == "Item")
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.hitItem);
            CoinManager.Instance.AddCoins(1);
            ParticleSystem particleTemp;
            particleTemp = (ParticleSystem)Instantiate(particle, other.gameObject.transform.position, Quaternion.identity);
            particleTemp.Simulate(0.5f, true, false);
            particleTemp.Play();
            Destroy(particleTemp, 0.5f);
            Destroy(other.gameObject);
        }
    }


    // This function make player move by position with speed and deltatime
    IEnumerator MovePlayer()
    {
        while (true)
        {
            if (!cameraController.startToRotateCamera)
            {
                player.transform.position = player.transform.position + dir * movingSpeedOfPlayer * Time.deltaTime;
            }
            yield return null;
        }
    }

    IEnumerator WaitAndEnableCheck(float time)
    {
        yield return new WaitForSeconds(time);
        enableCheck = true;
    }
}
