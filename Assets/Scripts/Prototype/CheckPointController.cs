using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    public bool checkpointReached;
    public PlayerManager playerManager;
    public PlayerController playerController;
    private HealthBar healthBar;
    public static CheckPointController instance;
    private CheckPointMenuScript checkPointMenuScript;
    public GameObject currentVCam;

    //Animations
    private Animator animator;
    private string currentState;
    const string idle = "CheckPoint";
    const string activated = "CheckPointActivated";

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("Bar").GetComponent<HealthBar>();
        animator = GetComponent<Animator>();
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        checkPointMenuScript = GameObject.FindGameObjectWithTag("CheckPointMenuManager").GetComponent<CheckPointMenuScript>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeAnimations();
        SetCheckpoint();
    }

    void ChangeAnimations()
    {

        if (checkpointReached)
        {
            if (Input.GetKeyDown(KeyCode.C) && !playerController.isGuarding)
            {
                ChangeAnimationState(activated);
                healthBar.RevertHealthBar();
            }

        }
    }


    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }

    void SetCheckpoint()
    {
        if (checkpointReached)
        {
            if (Input.GetKeyDown(KeyCode.C) && !playerController.isGuarding)
            {
                playerManager.currentCheckPoint = gameObject;
                playerManager.lives=4;
                playerManager.CurrentHealth += 100;
                Actions.OnHealthChanged();
                Debug.Log("Checkpoint Degisti");
                playerManager.lives = 4;
                playerManager.CurrentHealth = 100;
                healthBar.SetHealth(playerManager.CurrentHealth);
                Potion.instance.CheckPoint(); 
            }
        }
        if(!checkpointReached)
        {
            if(playerManager.currentCheckPoint != gameObject)
            {
                ChangeAnimationState(idle);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            checkpointReached = true;
            playerController.inCheckpointRange = true;
            checkPointMenuScript.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            checkpointReached = false ;
            playerController.inCheckpointRange = false;
            checkPointMenuScript.enabled = false;
        }
    }

}
