using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevel : MonoBehaviour
{
    public GameObject wallStart;
    public GameObject wallFinish;
    public GameObject _player;
    public GameMaster _gameMaster;
    public GameObject boss;

    public bool isStart;
    public int level;

    private GameObject lastTriggerGo = null;

    void Start()
    {
        _player = GameObject.Find("Player");
        _gameMaster = Camera.main.GetComponent<GameMaster>();

    }

    void LateUpdate()
    {
        if (_gameMaster.newRound)
        {
            _gameMaster.newRound = false;
        }

        if (_gameMaster.round > 3 && _gameMaster.level == level)
        {
            if (wallFinish != null)
            { wallFinish.SetActive(false); }
        }

    }

    void OnTriggerExit(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;

        if (go == _player)
        {
            if (isStart == false)
            {
                isStart = true;
                wallStart.SetActive(true);
                _gameMaster.level = level;
                _gameMaster.round = 1;
                _gameMaster.newRound = true;

                if (_gameMaster.level == 5)
                {
                    _gameMaster.newRound = false;
                    _gameMaster.round = 1;
                    boss = GameObject.Find("HexBoss");
                    StartCoroutine(boss.GetComponent<EnemyHexScript>().StartBossFight());
                }
            }
            
        }
    }
}
