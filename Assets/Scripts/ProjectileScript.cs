using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float speed;
    public int dmg = 1;
    public float destroyTime;
    
    public PlayerScript _playerScript;
    private GameObject lastTriggerGo = null;

    void Start()
    {
        //_playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();

    }


    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;

        if (go.tag == "isWall")
        {
            Destroy(this.gameObject);
        }

        if (go.tag == "Player" && this.tag != "Player")
        {
            PlayerScript pScript = go.GetComponent<PlayerScript>();
            if (pScript != null)
            {
                pScript.ShowDamage();
                pScript.health -= dmg;
                Destroy(this.gameObject);
            }
        }

        if (go.tag == "Enemy" && this.tag != "Enemy")
        {
            
            EnemyScript pScript = go.GetComponent<EnemyScript>();
            if (pScript != null)
            {
                pScript.health -= dmg;
                pScript.ShowDamage();
                Destroy(this.gameObject);
            }
            
        }
        
    }

}
