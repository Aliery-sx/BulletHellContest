using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameMaster _gameMaster;
    public GameObject _player;
    public PlayerScript _playerScript;
    public GameObject spawner;
    public GameObject deathObject;
    public GameObject powerUpObject;
    public Vector3 direction;
    public Vector3 pointNewPos;

    //Attributes
    public WeaponType type;
    public float speed = 4f;
    public float shotRange = 16f;
    public int health = 10;

    //Materials to show damage
    public float showDamageDuration = 0.1f;
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedOfDestruction = false;

    //Fire
    public Transform[] shootDir;
    public GameObject projectilePrefab;
    public float timeBetweenAttack = 1f;

    public bool mayFire = true;
    public bool fire;
    public bool _fire
    {
        get { return fire; }
        set
        {
            fire = value;
            if (value == true)
            {
                StartCoroutine(FireLogic());
            }
        }
    }
    // Start is called before the first frame update

    public void Awake()
    {
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    void Start()
    {
        _player = GameObject.Find("Player");
        _playerScript = _player.GetComponent<PlayerScript>();
        _gameMaster = Camera.main.GetComponent<GameMaster>();

    }

    void Update()
    {
        if (health <= 0)
        {

            GameObject deathIt = Instantiate(deathObject, transform.position, transform.rotation);

            Destroy(deathIt, 2f);

            _gameMaster.score += 10;

            GameObject powerUp = Instantiate(powerUpObject, transform.position, Quaternion.identity);
            int rand = Random.Range(0, 101);
            if (rand < 50)
            { 
                powerUp.GetComponent<PowerUp>().SetType(WeaponType.none);
                Destroy(powerUp);
            }
            else if (rand < 85)
            { powerUp.GetComponent<PowerUp>().SetType(WeaponType.hpbox); }
            else if (rand < 101)
            { powerUp.GetComponent<PowerUp>().SetType(WeaponType.gun); }

            Destroy(gameObject);
        }

        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        if (_player != null)
        {
            direction = _player.transform.position - transform.position;
            float timeToHit = direction.magnitude / 20f;
            pointNewPos = _player.transform.position + _playerScript.movement * _playerScript.speed * timeToHit;

            Quaternion targetRotation = Quaternion.LookRotation(pointNewPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.fixedDeltaTime);
            //transform.LookAt(pointNewPos);


            if (direction.magnitude > shotRange)
            { Movement(); }

            if (direction.magnitude < shotRange)
            { _fire = true; }
            else { _fire = false; }
        }
        else { _fire = false; }



    }

    public void Movement()
    {
        //direction.Normalize();
        //transform.Translate(direction * Time.deltaTime * speed);

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    public virtual IEnumerator FireLogic()
    {
        if (!mayFire)
            yield break;
        else
        {
            mayFire = false;
            if (!_fire)
            {
                mayFire = true;
                yield break;
            }

            Attack();



            yield return new WaitForSeconds(timeBetweenAttack);
            mayFire = true;
            if (_fire)
                StartCoroutine(FireLogic());
            yield break;
        }
    }

    public virtual void Attack()
    {
        GameObject projectile = Instantiate(projectilePrefab, new Vector3(shootDir[0].position.x, shootDir[0].position.y + 0.5f, shootDir[0].position.z), transform.rotation);
        projectile.tag = "Enemy";
        projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
    }

    public virtual void OnDestroy()
    {
        if (spawner != null)
        {
            EnemySpawner spawnerScript = spawner.GetComponent<EnemySpawner>();
            for (int i = 0; i < spawnerScript.spawnRounds.Length; i++)
            {
                if (spawnerScript.spawnRounds[i].round == _gameMaster.round)
                {
                    spawnerScript.spawnRounds[i].spawnCountHave -= 1;
                    _gameMaster.spawnCountMax -= 1;
                }
            }
        }     
    }

    public void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    public void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
