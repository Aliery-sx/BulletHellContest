using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //Общие переменные
    private Rigidbody _rB;
    public Vector3 movement;
    public Transform shootDir;
    public GameObject projectilePrefab;
    public GameObject deathObject;

    [Header("Свойства")]
    public float speed = 10f;
    public int weaponDmg = 1;
    public int weaponLevel = 1;
    public float timeBetweenAttack = 1f;
    public float health = 10;
    private float timeShot;
    public bool playerControl = true;

    float g = Physics.gravity.y;

    public WeaponType type;

    [Header("Скачок")]
    private bool isBlink = false;
    public GameObject blinkEffectGO;
    public int blinkCount = 3;
    public int blinkCountHave;
    public float blinkRange = 5f;

    //Materials to show damage
    public float showDamageDuration = 0.1f;
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedOfDestruction = false;

    //Стрельба
    private bool mayFire = true;
    private bool fire;
    public bool _fire
    {
        get {return fire;}
        set 
        { 
            fire = value;
            if (value==true)
            {
                StartCoroutine(FireLogic());
            }
        }
    }

    public Vector3 direction;

    void Awake()
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
        _rB = GetComponent<Rigidbody>();
        blinkCountHave = blinkCount;
        Invoke("BlinkReload",1f);
    }

    
    void Update()
    {
        if (health <= 0)
        {
            GameObject deathIt = Instantiate(deathObject, new Vector3(transform.position.x, 0 , transform.position.z), transform.rotation);

            //Destroy(deathIt, 5f);

            Destroy(gameObject);
        }

        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        Debug.DrawRay(transform.position, direction, Color.red);
    }

    void FixedUpdate()
    {
        MovementLogic();

        //Скачок
        if (Input.GetKey("space") && !isBlink && (blinkCountHave > 0))
        {
            isBlink = true;
            Blink();
        }


        //Стрельба
        if (Input.GetMouseButton(0))
        {
            _fire = true;
        }
        if (!Input.GetMouseButton(0))
        {
            _fire = false;
        }
    }


    void MovementLogic()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        float moveVertical = Input.GetAxisRaw("Vertical");

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        _rB.velocity = movement * speed;
    }

    void Blink()
    {
        GameObject effectGO = Instantiate(blinkEffectGO, transform.position, Quaternion.identity);
        
        _rB.velocity = movement * blinkRange * 50;
        
        Destroy(effectGO, 1f);

        blinkCountHave -= 1;

        //Time.timeScale = 0.2f;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;
        //speed = speed * 5f;

        Invoke("TakeControl", 0.5f);
    }

    void BlinkReload()
    {
        if (blinkCountHave<blinkCount)
            { blinkCountHave += 1; }
        
        Invoke("BlinkReload", 2f);
    }

    public void SetType(WeaponType wt)
    {
        WeaponDefinition def = GameMaster.GetWeaponDefinition(wt);
        weaponLevel = 1;
        weaponDmg = def.damageOnHit;
        timeBetweenAttack = def.timeBetweenAttack;
        projectilePrefab = def.projectilePrefab;
        type = wt;
    }

    IEnumerator FireLogic()
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

            switch (type)
            {
                case WeaponType.gun:
                    AttackGun();
                    break;
                case WeaponType.shotgun:
                    AttackShotgun();
                    break;
                case WeaponType.mortyr:
                    AttackMortyr();
                    break;
            }

            yield return new WaitForSeconds(timeBetweenAttack);
            mayFire = true;
            if (_fire)
                StartCoroutine(FireLogic());
            yield break;
        }
    }

    void AttackGun()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootDir.position, transform.rotation);
        projectile.tag = "Player";
        projectile.GetComponent<ProjectileScript>().dmg = weaponDmg * weaponLevel;
    }

    void AttackShotgun()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootDir.position, transform.rotation);
        projectile.tag = "Player";
        projectile.GetComponent<ProjectileScript>().dmg = weaponDmg * weaponLevel;
        
        projectile = Instantiate(projectilePrefab, shootDir.position, transform.rotation * Quaternion.Euler(0, 30, 0));
        projectile.tag = "Player";
        projectile.GetComponent<ProjectileScript>().dmg = weaponDmg * weaponLevel;

        projectile = Instantiate(projectilePrefab, shootDir.position, transform.rotation * Quaternion.Euler(0, -30, 0));
        projectile.tag = "Player";
        projectile.GetComponent<ProjectileScript>().dmg = weaponDmg * weaponLevel;

    }

    void AttackMortyr()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float hitdist = 0.0f;

        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);


            direction = targetPoint - transform.position;
            Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);

            float x = directionXZ.magnitude;
            float y = direction.y;

            float angleToRadians = -45f * Mathf.PI / 180;
            float v2 = (g * x * x) / (2 * (y - Mathf.Tan(angleToRadians) * x) * Mathf.Pow(Mathf.Cos(angleToRadians), 2));
            float v = Mathf.Sqrt(Mathf.Abs(v2));


            GameObject projectile = Instantiate(projectilePrefab, shootDir.position, shootDir.rotation * Quaternion.Euler(-45, 0, 0));
            projectile.tag = "Player";
            projectile.GetComponent<Projectile2Script>().speed = v;
            projectile.GetComponent<Projectile2Script>().dmg = weaponDmg * weaponLevel;
        }
    }

    void TakeControl()
    {
        isBlink = false;
        //Time.timeScale = 1f;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;
        //speed = speed / 10f;

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
