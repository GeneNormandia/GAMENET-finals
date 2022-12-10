using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerPrefab;

    public GameObject aiPrefab;
    public GameObject[] spawnPoints;
    public GameObject[] actorSpawnPoints;

    public GameObject gameOverUi;
    public int maxKills;


    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            /*int randomPointX = Random.Range(-10, 10);
            int randomPointZ = Random.Range(-10, 10);*/

            PhotonNetwork.Instantiate(playerPrefab.name,  spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
            
        }
        for(int i = 0; i < actorSpawnPoints.Length; i++)
        {
            Instantiate(aiPrefab, actorSpawnPoints[i].transform.position, Quaternion.identity);
        }
        //Instantiate(aiPrefab, actorSpawnPoints[Random.Range(0, actorSpawnPoints.Length)].transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 setSpawnPoint()
    {
        //PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
        return spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
    }

    public void QuitRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }
}
