using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class ShootingScript : MonoBehaviourPunCallbacks
{
    public Camera camera;

    public GameObject hitEffectPrefab;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    private Animator animator;
    public int kills = 1;


    GameManager gameManager;
    public float fireRate = 0.1f;
    protected float fireTimer = 0;

    public int rolePicker;
    private float lives = 5;
    public MeshRenderer firstPersonCharacter;
    public int actorCount;


    public enum RaiseEventsCode
    {
        ActorsDiedEventCode = 0
    }

    private  void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private  void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.ActorsDiedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0];
            int viewId = (int)data[1];

            GameObject gameOverText = GameManager.instance.gameOverUi;
           gameOverText.SetActive(true);

            Debug.Log("Successfully called the event");

            gameOverText.GetComponent<Text>().text = ("Game Over! Guessers win!");
        }
    }


    void Wake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        animator = this.GetComponent<Animator>();

        rolePicker = Random.Range(1, 3);
        //rolePicker = 2;

        
    }

    // Update is called once per frame
    void Update()
    {
       

        GameObject roleText = GameObject.Find("Role Text");
        GameObject lifeText = GameObject.Find("Life Text");
        Debug.Log("Lives: " + lives);
        lifeText.GetComponent<Text>().text = ("Lives: " + lives + "/" + "5");

        if (rolePicker == 1)
        {
            roleText.GetComponent<Text>().text = ("You are the GUESSER! Find the actors!");
            ExitGames.Client.Photon.Hashtable playerCustomProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_GUESSER_ROLE, true}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);

        }
        else if (rolePicker == 2)
        {
            actorCount++;
            roleText.GetComponent<Text>().text = ("You are the ACTOR! Try not to get spotted!");
            ExitGames.Client.Photon.Hashtable playerCustomProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_ACTOR_ROLE, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
            //firstPersonCharacter.enabled = false;

        }


    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log("hitting" + hit.collider.gameObject.name);

            
            //Debug.Log("hitting an actor");

            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            this.gameObject.GetComponent<PhotonView>().RPC("LivesChecker", RpcTarget.AllBuffered);

            // AllBuffered means current and future players in room will get this broadcast function\
            //If the player is shooting the enemy actors
            if (hit.collider.gameObject.CompareTag("Actor") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 100);
                lives -= 5;
                if (hit.collider.gameObject.GetComponent<ShootingScript>().health <= 0)
                {
                    this.gameObject.GetComponent<PhotonView>().RPC("ActorCountChecker", RpcTarget.AllBuffered);
                }
            }
            //If the guesser is shooting an NPC
            else if (hit.collider.gameObject.CompareTag("NPC") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                this.lives--;

                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 100);
                lives -= 5;
                if (hit.collider.gameObject.GetComponent<ShootingScript>().health <= 0)
                {
                    this.gameObject.GetComponent<PhotonView>().RPC("ActorCountChecker", RpcTarget.AllBuffered);
                }
            }

        }
    }

    public void FixedUpdate()
    {


            /*if (fireTimer < fireRate)
            {
                fireTimer += Time.deltaTime;
            }

            if (Input.GetButton("Fire1") && fireTimer > fireRate)
            {
                Debug.Log("Firing");
                Fire();
                fireTimer = 0;
                //laserLineRenderer.enabled = photonView.IsMine;
            }
            else
            {
                //laserLineRenderer.enabled = false;
            }*/
        
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }


    public void Die()
    {
        StartCoroutine(gameOverScreen());
    }

    IEnumerator gameOverScreen()
    {
        GameObject respawnText = GameObject.Find("Respawn Text");
        float respawnTime = 5.0f;

        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            respawnText.GetComponent<Text>().text = "Game Over! Guessers win!";
        }

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }

    IEnumerator guesserLose()
    {
        GetComponent<PlayerMovementController>().enabled = false;

        GameObject respawnText = GameObject.Find("Respawn Text");
        float timer = 5.0f;

        

        while (timer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timer--;

            respawnText.GetComponent<Text>().text = ("Game Over! Actors win!");


        }

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }

    public IEnumerator KillFeedCounter (string killer, string dead)
    {
        GameObject killFeedText = GameObject.Find("Kill Feed Text");
        killFeedText.GetComponent<Text>().text = killer + " killed " + dead;

        float killfeedTime = 3.0f;

        while (killfeedTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            killfeedTime--;
        }
        killFeedText.GetComponent<Text>().text = "";
    }

    [PunRPC]
    public void ActorCountChecker() //Check if there are any actors left in the scene
    {
        if (actorCount <= 0)
        {
            StartCoroutine(gameOverScreen());

            string nickName = photonView.Owner.NickName;
            int viewId = photonView.ViewID;

            // event data
            object[] data = new object[] { nickName, viewId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOption = new SendOptions
            {
                Reliability = false
            };

            PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.ActorsDiedEventCode, data, raiseEventOptions, sendOption);
            //Debug.Log("Calling event");

        }
    }

    [PunRPC]
    public void LivesChecker()
    {
        //Checks the guesser's lives
        if (this.lives <= 0)
        {
            StartCoroutine(guesserLose());
        }
    }
       
}
