using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4Script : EnemyScript
{
    public bool dodge = true;
    public float startTime;
    int speedV = 1;
    float g = Physics.gravity.y;


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

            _gameMaster.score += 50;

            GameObject powerUp = Instantiate(powerUpObject, transform.position, Quaternion.identity);
            int rand = Random.Range(0, 101);
            if (rand < 40)
            {
                powerUp.GetComponent<PowerUp>().SetType(WeaponType.none);
                Destroy(powerUp);
            }
            else if (rand < 65)
            { powerUp.GetComponent<PowerUp>().SetType(WeaponType.hpbox); }
            else if (rand < 101)
            { powerUp.GetComponent<PowerUp>().SetType(WeaponType.mortyr); }
            Destroy(gameObject);
        }

        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        startTime -= Time.deltaTime;

        if (dodge)
        {
            if (startTime <= timeBetweenAttack)
            { speedV = Random.Range(-1, 2); }
            if (_player != null)
            {
                transform.RotateAround(_player.transform.position, Vector3.up, speedV * 10 * Time.deltaTime);
            }
        }
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

            Attack();
            startTime = timeBetweenAttack * 2;
            dodge = true;


            yield return new WaitForSeconds(timeBetweenAttack);
            mayFire = true;
            dodge = false;


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

    public override void OnDestroy()
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
}
