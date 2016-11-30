using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

  
    public Animator anim;
    public GameObject impactPrefab;
    GameObject[] impacts;
    int currentImpact = 0;
    int maxImpacts = 5;

    bool attacking = false;
    float damage = 25f;


	// Use this for initialization
	void Start () {
        impacts = new GameObject[maxImpacts];
        for(int i = 0; i < maxImpacts; i++)
        {
            impacts[i] = (GameObject)Instantiate(impactPrefab);
        }

        anim = GetComponentInChildren<Animator>(); 
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1") && !Input.GetKey(KeyCode.LeftShift)) 
        {
            anim.SetTrigger("Attack");
            attacking = true;
        }
	}

    void FixedUpdate()
    {
        if(attacking)
        {
            attacking = false;

            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, 3f))
            {
                if (hit.transform.tag == "Player")
                {
                    hit.transform.GetComponent<PhotonView>().RPC("GetShot", PhotonTargets.All, damage);                  
                }
                impacts[currentImpact].transform.position = hit.point;
                impacts[currentImpact].GetComponent<ParticleSystem>().Play();

                if (++currentImpact >= maxImpacts)
                    currentImpact = 0;
            }
        }
    }
}
