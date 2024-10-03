using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameMode{
    idle,
    playing,
    levelEnd
}
public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;

    [Header("Inscribed")]
    public TextMeshProUGUI uitLevel;
    public TextMeshProUGUI uitShots;
    public TextMeshProUGUI uitTime;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";
    public float timeLimit = 60f;
    private float timeRemaining;

    [Header("UI PAnels")]
    public GameObject gameOverPanel;

    void Start()
    {
        S = this;

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        StartLevel();
    }

    void StartLevel(){
        if(castle != null){
            Destroy(castle);
        }

        Projectile.DESTROY_PROJECTILES();

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        Goal.goalMet = false;
        timeRemaining = timeLimit;

        UpdateGUI();

        mode = GameMode.playing;

        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI(){
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots taken: " + shotsTaken;
        uitTime.text = "Time Remaining: " + Mathf.Ceil(timeRemaining).ToString();
    }

    void Update()
    {
        UpdateGUI();

        if(mode == GameMode.playing){
            timeRemaining -= Time.deltaTime;
            if(timeRemaining <= 0){
                timeRemaining = 0;
                GameOver();
            }
        }

        if (uitTime != null) {
            uitTime.text = "Time Remaining: " + Mathf.Ceil(timeRemaining).ToString(); 
        } else {
            Debug.LogError("Timer UI Text (uitTime) is not assigned!"); 
        }

        if((mode == GameMode.playing)&&Goal.goalMet){
            mode = GameMode.levelEnd;

            FollowCam.SWITCH_VIEW(FollowCam.eView.both);

            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel(){
        if(level == levelMax - 1){
            GameOver();
            return;
        }
        level++;
        StartLevel();
    }

    static public void SHOT_FIRED(){
        S.shotsTaken++;
    }

    static public GameObject GET_CASTLE(){
        return S.castle;
    }

    void GameOver(){
        if (gameOverPanel != null){
            gameOverPanel.SetActive(true);
        }
        mode = GameMode.idle;
    }

    public void RestartGame(){
        level = 0;
        shotsTaken = 0;
        StartLevel();

        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }
    }
}
