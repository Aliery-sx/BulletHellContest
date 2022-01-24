using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Script : EnemyScript
{
    public bool dodge = true;
    public float startTime;
    int speedV = 1;




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

            _gameMaster.score += 25;

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
            { powerUp.GetComponent<PowerUp>().SetType(WeaponType.shotgun); }

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
            startTime = timeBetweenAttack*2;
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
        GameObject projectile = Instantiate(projectilePrefab, new Vector3(shootDir[0].position.x, shootDir[0].position.y + 0.5f, shootDir[0].position.z), shootDir[0].rotation);
        projectile.tag = "Enemy";
        projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
        projectile = Instantiate(projectilePrefab, new Vector3(shootDir[1].position.x, shootDir[1].position.y + 0.5f, shootDir[1].position.z), shootDir[1].rotation);
        projectile.tag = "Enemy";
        projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
        projectile = Instantiate(projectilePrefab, new Vector3(shootDir[2].position.x, shootDir[2].position.y + 0.5f, shootDir[2].position.z), shootDir[2].rotation);
        projectile.tag = "Enemy";
        projectile.GetComponent<Renderer>().material.color = this.GetComponent<Renderer>().material.color;
        //projectile.transform.Translate(Vector3.forward * (speed+10) * Time.deltaTime);
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
