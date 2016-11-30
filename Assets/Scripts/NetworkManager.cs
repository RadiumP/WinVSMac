//lobby, join 

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {
    //Force Unity to serialize a private field.
    //You will almost never need this. When Unity serializes your scripts, 
    //it will only serialize public fields.
    //If in addition to that you also want Unity to serialize one of your private fields you can add the SerializeField attribute to the field.

    [SerializeField] Text connectionText;//.UI
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Camera sceneCamera;

    GameObject player;


	// Use this for initialization
	void Start () {
        PhotonNetwork.logLevel = PhotonLogLevel.Full;//get info
        PhotonNetwork.ConnectUsingSettings("0.1");// version

	}
	
	// Update is called once per frame
	void Update () {
        connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();//show connection info
	}

    void OnJoinedLobby()
    {
        
        //PhotonNetwork.JoinRandomRoom();
        RoomOptions ro = new RoomOptions()
        {
            IsVisible = true,
            MaxPlayers = 4
        };
        PhotonNetwork.JoinOrCreateRoom("WinVsMac", ro, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        StartSpawnProcess(0f);
    }


    void StartSpawnProcess(float respawnTime)
    {
        sceneCamera.enabled = true; // lobby camera
        StartCoroutine("SpawnPlayer", respawnTime);

    }

    IEnumerator SpawnPlayer(float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);
        int index = Random.Range(0, spawnPoints.Length);
        player = PhotonNetwork.Instantiate("FPSController", spawnPoints[index].position, spawnPoints[index].rotation, 0);//name in the resources folder

        player.GetComponent<PlayerNetworkMover>().RespawnMe += StartSpawnProcess;
        sceneCamera.enabled = false;
    }
}
