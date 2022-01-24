using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    none,
    gun,
    shotgun,
    mortyr,
    hpbox
}

[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;
    public Color color = Color.white;
    public GameObject projectilePrefab;
    public int damageOnHit = 0;
    public float timeBetweenAttack = 0;
}

public class Weapon : MonoBehaviour
{
    
}
