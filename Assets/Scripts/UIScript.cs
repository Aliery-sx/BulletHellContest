using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    [Header("UI игры")]
    public GameObject _player;
    public PlayerScript _playerScript;
    public GameMaster _gameMaster;

    //public Canvas UICanvas;
    public TextMeshProUGUI hpBarText;
    public TextMeshProUGUI hpBossText;
    public TextMeshProUGUI levelRound;


    public GameObject[] blinkPoints;
    public GameObject hpBossObj;
    public GameObject restartObj;
    public GameObject helpObj;
    public EnemyHexScript _bossScript;
    public Image barPlayer;
    public Image barBoss;
    public float fillPlayer;
    public float fillBoss;
    public float exit = 0;


    void Start()
    {

        _player = GameObject.Find("Player");
        _playerScript = _player.GetComponent<PlayerScript>();
        //UICanvas = GameObject.Find("UI").GetComponent<Canvas>();
        _gameMaster = Camera.main.GetComponent<GameMaster>();
    }

    void Update()
    {
        fillPlayer = _playerScript.health / 50;
        barPlayer.fillAmount = fillPlayer;
        hpBarText.text = _playerScript.health.ToString();

        fillBoss = (float)_bossScript.health / (float)_bossScript.healthMax;
        barBoss.fillAmount = fillBoss;
        hpBossText.text = _bossScript.health.ToString();

        levelRound.text = "Уровень: " + _gameMaster.level + "\nРаунд: " + _gameMaster.round + "\nОчки: " + _gameMaster.score;

        if (_gameMaster.level == 5)
        { hpBossObj.SetActive(true); }
        
        if (_playerScript.blinkCountHave >= 1)
        { blinkPoints[0].SetActive(true);}
        else { blinkPoints[0].SetActive(false); }
        if (_playerScript.blinkCountHave >= 2)
        { blinkPoints[1].SetActive(true); }
        else { blinkPoints[1].SetActive(false); }
        if (_playerScript.blinkCountHave >= 3)
        { blinkPoints[2].SetActive(true); }
        else { blinkPoints[2].SetActive(false); }

        if (_player == null)
        {
            restartObj.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (helpObj.activeSelf)
            {
                helpObj.SetActive(false);
            }
            else
            {
                exit += 1;
                restartObj.SetActive(true);
                Invoke("NoExit", 3f);
            }

            if (exit > 1)
            {
                Application.Quit();
            }
        }

        if (_playerScript.playerControl==false)
        {
            restartObj.SetActive(true);
        }
    }

    void NoExit()
    {
        exit -= 1f;
        restartObj.SetActive(false);
    }
}
