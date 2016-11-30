
using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;//for getComponent
public class PlayerNetworkMover : Photon.MonoBehaviour {

    public delegate void Respawn(float time);
    //Delegates are used to pass methods as arguments to other methods. 
    //Event handlers are nothing more than methods that are invoked through delegates. 
    public event Respawn RespawnMe;



    //observ player network; OR obs tranform which is working but not good
    Vector3 position;
    Quaternion rotation;
    float smoothing = 10f;
    float health = 100f;

    //if isMyplayer Enable all the components, else dummy
	void Start () {
	    if(photonView.isMine)
        {
            //enable
            GetComponent<Rigidbody>().useGravity = true;            
            GetComponent<FirstPersonController>().enabled = true;
            GetComponent<CharacterController>().enabled = true;
            GetComponentInChildren<PlayerAttack>().enabled = true;
            foreach (Camera cam in GetComponentsInChildren<Camera>())
                cam.enabled = true;
            transform.Find("FirstPersonCharacter/WeaponCam/Weapon_10").gameObject.layer = 8;
        }
        else
        {
            StartCoroutine("UpdateData");
        }
	}
	IEnumerator UpdateData()
    {
        while(true)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);    // not that accurate
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing); // to smooth the movement data updating
            yield return null;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            //current
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(health);
        }
        else//read
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
            health = (float)stream.ReceiveNext();
        }

    }

    //RPC remote procesure call
    [PunRPC]
    public void GetShot(float damage)
    {
        health -= damage;
        if (health <= 0 && photonView.isMine)//avoid multiple removal
        {
            if (RespawnMe != null)
                RespawnMe(3f);
            PhotonNetwork.Destroy(gameObject); // remove from network
        }
    }
	
}
