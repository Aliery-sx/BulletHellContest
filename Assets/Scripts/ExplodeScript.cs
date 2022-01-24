using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeScript : MonoBehaviour
{
    public int dmg = 2;
    private GameObject lastTriggerGo = null;

    void Start()
    {
        Destroy(gameObject, 2f);
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
