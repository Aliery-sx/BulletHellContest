using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHexScript : EnemyScript
{
    float g = Physics.gravity.y;
    public GameObject hexEye;
    public GameObject[] hexGuns;
    public GameObject hexGunProjectile;
    public int healthMax;
    public int phase = 1;

    public bool battleStart = false;

    void Start()
    {
        _player = GameObject.Find("Player");
        _playerScript = _player.GetComponent<PlayerScript>();
        _gameMaster = Camera.main.GetComponent<GameMaster>();
        healthMax = health;
        Invoke("AttackHexGun", 0.12f);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (((float)healthMax / (float)health) > 3f)
        {
            phase = 2;
        }
        else if (((float)healthMax / (float)health) > 6f)
        {
            phase = 3;
        }
        
        if (health <= 0)
        {
            GameObject deathIt = Instantiate(deathObject, transform.position, transform.rotation);


            _gameMaster.score += 5000;
            _playerScript.playerControl = false;

            Destroy(gameObject);
        }

        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }
    }

    public override void FixedUpdate()
    {
        if (phase >= 2)
        { transform.Rotate(0.0f, 60.0f * Time.deltaTime, 0.0f, Space.Self); }

        
        
        if (_player != null && battleStart)
        {
            direction = _player.transform.position - transform.position;
            float timeToHit = direction.magnitude / 10f;
            pointNewPos = _player.transform.position + _playerScript.movement * _playerScript.speed * timeToHit;

            Quaternion targetRotation = Quaternion.LookRotation(pointNewPos - transform.position);
            hexEye.transform.rotation = Quaternion.Slerp(hexEye.transform.rotation, targetRotation, speed * Time.fixedDeltaTime);


            if (direction.magnitude < shotRange)
            { _fire = true; }
            else { _fire = false; }
        }
        else { _fire = false; }
    }

    

    public override IEnumerator FireLogic()
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


            int rand = Random.Range(0, 101);
            if (rand < 60)
            {
                Attack();
            }
            else if (rand < 101)
            {
                if (phase == 1)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        GameObject projectile = Instantiate(hexGunProjectile, hexGuns[0].transform.position, hexGuns[0].transform.rotation);
                        projectile.tag = "Enemy";
                        projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
                        projectile = Instantiate(hexGunProjectile, hexGuns[1].transform.position, hexGuns[1].transform.rotation);
                        projectile.tag = "Enemy";
                        projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
                        projectile = Instantiate(hexGunProjectile, hexGuns[2].transform.position, hexGuns[2].transform.rotation);
                        projectile.tag = "Enemy";
                        projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
                        transform.rotation = transform.rotation * Quaternion.Euler(0, 60, 0);
                        hexEye.transform.rotation = hexEye.transform.rotation * Quaternion.Euler(0, -60, 0);
                        yield return new WaitForSeconds(timeBetweenAttack / 4);
                    }
                }
                else { Attack(); }
            }


            if (phase == 3)
            {
                yield return new WaitForSeconds(timeBetweenAttack*0.7f);
            }
            else
            {
                yield return new WaitForSeconds(timeBetweenAttack);
            }
            mayFire = true;



            if (_fire)
                StartCoroutine(FireLogic());
            yield break;
        }
    }

    public override void Attack()
    {
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);

        float x = directionXZ.magnitude;
        float y = direction.y;

        float angleToRadians = -45f * Mathf.PI / 180;
        float v2 = (g * x * x) / (2 * (y - Mathf.Tan(angleToRadians) * x) * Mathf.Pow(Mathf.Cos(angleToRadians), 2));
        float v = Mathf.Sqrt(Mathf.Abs(v2));

        GameObject projectile = Instantiate(projectilePrefab, shootDir[0].position, shootDir[0].rotation);
        projectile.tag = "Enemy";
        projectile.GetComponent<Projectile2Script>().speed = v;
        projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
    }

    void AttackHexGun()
    {
        if (phase == 2)
        {
            GameObject projectile = Instantiate(hexGunProjectile, hexGuns[0].transform.position, hexGuns[0].transform.rotation);
            projectile.tag = "Enemy";
            projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
            projectile = Instantiate(hexGunProjectile, hexGuns[1].transform.position, hexGuns[1].transform.rotation);
            projectile.tag = "Enemy";
            projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
            projectile = Instantiate(hexGunProjectile, hexGuns[2].transform.position, hexGuns[2].transform.rotation);
            projectile.tag = "Enemy";
            projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
        }
        Invoke("AttackHexGun", 0.12f);
    }

    public IEnumerator StartBossFight()
    {
        Vector3 destination = new Vector3(0f, 0f, 659f);
        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime);
            yield return null;
        }

        destination = new Vector3(0f, 4.6f, 659f);
        while (hexEye.transform.position != destination)
        {
            hexEye.transform.position = Vector3.MoveTowards(hexEye.transform.position, destination, 6f * Time.deltaTime);
            yield return null;
        }

        Vector3 pointPos = new Vector3(-22f, 0f, 627f);
        float t = 0;
        
        while (t < 1)
        {
            Quaternion targetRotation = Quaternion.LookRotation(pointPos - transform.position);
            hexEye.transform.rotation = Quaternion.Slerp(hexEye.transform.rotation, targetRotation, t*t*t);
            t += 1f*Time.deltaTime;
            yield return null;
        }
        
        t = 0;
        pointPos = new Vector3(31f, 0f, 618f);
        while (t < 1)
        {
            Quaternion targetRotation = Quaternion.LookRotation(pointPos - transform.position);
            hexEye.transform.rotation = Quaternion.Slerp(hexEye.transform.rotation, targetRotation, t * t);
            t += 2f * Time.deltaTime;
            yield return null;
        }

        battleStart = true;
    }
}
