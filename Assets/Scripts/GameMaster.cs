using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public int score = 0;

    public int level = 1;
    public int round = 0;
    public bool newRound = false;
    public int spawnCountMax = 0;

    public WeaponDefinition[] weaponDefinitions;

    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    // Start is called before the first frame update
    void Awake()
    {
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (spawnCountMax <= 0 && newRound == false && round != 0 && round < 4)
        {
            round += 1;

            newRound = true;
        }

        if (Input.GetKey("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }

        return (new WeaponDefinition());
    }
}
