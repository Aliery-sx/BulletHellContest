using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject spawnPrefab;
    public GameMaster _gameMaster;
    public int level = 0;
    public int spawnCountMax = 0;
    public bool newRound;


    private bool maySpawn = true;
    private bool canSpawn;
    private bool _canSpawn
    {
        get { return canSpawn; }
        set
        {
            canSpawn = value;
            if (value == true)
            {
                StartCoroutine(SpawnLogic());
            }
        }
    }

    [System.Serializable]
    public class SpawnerAreas
    {
        public GameObject spawnPrefab;
        public int spawnCountMax = 3;//может быть вызвано всего
        public int spawnCountEach = 1;//может быть вызвано за раз
        public int spawnCountHave = 0;//сколько есть в данный момет
        public int spawnCount = 0;//сколько всего уже вызвано
        public float timeToSpawn = 2f;
        public int round = 0;
    }

    public SpawnerAreas[] spawnRounds;

    void Start()
    {
        _gameMaster = Camera.main.GetComponent<GameMaster>();
    }


    void Update()
    {
        if (level == _gameMaster.level)
        {
            for (int i=0; i< spawnRounds.Length; i++)
            {
                if (spawnRounds[i].round == _gameMaster.round)
                {
                    if (spawnRounds[i].spawnCount < spawnRounds[i].spawnCountMax)
                    {
                        if (spawnRounds[i].spawnCountHave < spawnRounds[i].spawnCountEach)
                        {
                            _canSpawn = true;
                        }
                        else { _canSpawn = false; }
                    }
                    else { _canSpawn = false; }
                }
            }
        }

        
    }

    IEnumerator SpawnLogic()
    {
        if (!maySpawn)
            yield break;
        else
        {
            maySpawn = false;
            if (!_canSpawn)
            {
                maySpawn = true;
                yield break;
            }
            for (int i = 0; i < spawnRounds.Length; i++)
            {
                if (spawnRounds[i].round == _gameMaster.round)
                {
                    GameObject spawnGO = Instantiate(spawnRounds[i].spawnPrefab, transform.position, Quaternion.identity);
                    EnemyScript scrEnemy = spawnGO.GetComponent<EnemyScript>();
                    scrEnemy.spawner = this.gameObject;

                    spawnRounds[i].spawnCountHave += 1;
                    spawnRounds[i].spawnCount += 1;
                    _gameMaster.spawnCountMax += 1;
                    yield return new WaitForSeconds(spawnRounds[i].timeToSpawn);
                }

            }
            
                
            maySpawn = true;
            if (_canSpawn)
                StartCoroutine(SpawnLogic());
            yield break;

        }
    }
}
