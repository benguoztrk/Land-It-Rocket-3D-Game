using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketController : MonoBehaviour
{
    [SerializeField] private float forceAmount = 10f, rotationAmaount = 40f;
    [SerializeField] private AudioClip rocketSound, winSound, loseSound;
    [SerializeField] private ParticleSystem rocketPartcial, winPartical, losePartical;
    [SerializeField] private float transitionTime = 2f;

    private enum State{WIN, LOSE, ALIVE}

    private State playerState = State.ALIVE;

    private AudioSource audioSource;
    private Rigidbody myBody;

    

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        myBody = GetComponent<Rigidbody>();
    }


    
    
    void Update()
    {
        if (playerState == State.ALIVE)
        {
            HandleRotation();
        }
        
    }

    void FixedUpdate()
    {
        if (playerState == State.ALIVE)
        {
            HandleMovement();
        }

    }


    void HandleRotation()
    {
        myBody.freezeRotation = true;

        float rotationThisFrame = rotationAmaount * Time.deltaTime;

        if (Input.GetAxisRaw("Horizontal") < 0) //if it is less than zero we are going to the left side
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0) //if it is more than zero we are going to the right side
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        


        myBody.freezeRotation = false;
    }


    void HandleMovement()
    {

        if (Input.GetKey(KeyCode.W))
        {
            myBody.AddRelativeForce(Vector3.up * forceAmount);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(rocketSound);
            }
            rocketPartcial.Play();
        }else if (Input.GetKey((KeyCode.S)))
        {
            myBody.AddRelativeForce(Vector3.down * forceAmount);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(rocketSound);
            }
            rocketPartcial.Play();
        }
        else
        {
            rocketPartcial.Stop();
        }


    }


    void LevelFinished()
    {
        playerState = State.WIN;
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        winPartical.Play();

        Invoke("LoadNextLevel", transitionTime);
    }

    void PlayerDied()
    {

        playerState = State.LOSE;
        audioSource.Stop();
        audioSource.PlayOneShot(loseSound);
        losePartical.Play();

        Invoke("RestartLevel", transitionTime);
    }

    void LoadNextLevel()
    {
        int currentScene_Plus_NexScene = SceneManager.GetActiveScene().buildIndex;
        currentScene_Plus_NexScene += 1;
        int count = SceneManager.sceneCountInBuildSettings;  //total number of scene

        if (currentScene_Plus_NexScene == count)   //Meaning If we reach the last scene, go back to first scene.
        {
            SceneManager.LoadScene(0);
        }
        else //otherwise load the next scene.
        {
            SceneManager.LoadScene(currentScene_Plus_NexScene);
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



    void OnCollisionEnter(Collision target)
    {

        if (playerState != State.ALIVE)
            return;

        switch (target.gameObject.tag)
        {

            case "Friendly":
                break;
            case "Finish":
                LevelFinished();
                break;
            default:
                PlayerDied();
                break;

        }



    }


}




    





