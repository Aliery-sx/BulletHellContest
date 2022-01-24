using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(0.25f, 2);
    public float lifeTime = 30f;

    public PlayerScript _playerScript;
    public WeaponType type;

    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rigid;
    private Renderer cubeRend;

    void Awake()
    {
        cube = transform.Find("Cube").gameObject;
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        cubeRend = cube.GetComponent<Renderer>();

        /*Vector3 vel = Random.onUnitSphere;
        vel.z = 0;
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;*/

        transform.rotation = Quaternion.identity;
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }


    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        float u = (Time.time - (birthTime + lifeTime));

        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        /*if (u>0)
        {
            Color c = cubeRend.material.color;
            c = letter.color;
        }*/

    }

    public void SetType(WeaponType wt)
    {
        WeaponDefinition def = GameMaster.GetWeaponDefinition(wt);
        cubeRend.material.color = def.color;
        letter.text = def.letter;
        type = wt;
    }


    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;


        if (go.tag == "Player")
        {
            PlayerScript pScript = go.GetComponent<PlayerScript>();
            if (pScript != null)
            {
                switch (type)
                {
                    case WeaponType.hpbox:
                        pScript.health += 2;
                        break;
                    default:
                        if (pScript.type == type)
                        {
                            if (pScript.weaponLevel < 3)
                            { pScript.weaponLevel += 1; }
                        }
                        else 
                        {
                            pScript.SetType(type);
                        }
                        break;
                }
                Destroy(this.gameObject);
            }
        }
    }
}
