using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2Script : MonoBehaviour
{
    public float speed;
    public int dmg = 2;
    public float destroyTime;

    public PlayerScript _playerScript;
    public GameObject explodePrefab;

    private GameObject lastTriggerGo = null;

    void Start()
    {
        //_playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
        Destroy(gameObject, destroyTime);

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
            GameObject inst = Instantiate(explodePrefab, transform.position, Quaternion.identity);
            inst.tag = this.tag;
            Destroy(this.gameObject);
        }

        if (go.tag == "Player" && this.tag != "Player")
        {            
            GameObject inst = Instantiate(explodePrefab, transform.position, Quaternion.identity);
            ExplodeScript pScript = inst.GetComponent<ExplodeScript>();
            if (pScript != null)
            {
                pScript.dmg = dmg;
                inst.tag = this.tag;
                Destroy(this.gameObject);
            }
        }

        if (go.tag == "Enemy" && this.tag != "Enemy")
        {
            GameObject inst = Instantiate(explodePrefab, transform.position, Quaternion.identity);
            ExplodeScript pScript = inst.GetComponent<ExplodeScript>();
            if (pScript != null)
            {
                pScript.dmg = dmg;
                inst.tag = this.tag;
                Destroy(this.gameObject);
            }
        }

    }

}
