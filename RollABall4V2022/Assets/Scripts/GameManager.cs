using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {
    Start,
    Play,
    GameOver,
    Victory,
    Ending
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState gameState;
    
    [SerializeField] 
    private GameObject playerAndCameraPrefab;

    [SerializeField] 
    private string locationToLoad;

    [SerializeField] 
    private string guiScene;

    [SerializeField] 
    private GameModeSO gameMode;

    private float _currentTime;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        PlayerObserverManager.OnPlayerCoinsChanged += OnPlayerCoinsChanged;
        gameMode.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        PlayerObserverManager.OnPlayerCoinsChanged -= OnPlayerCoinsChanged;
        gameMode.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Initialization")
        {
            StartGameFromInitialization();
        }
        else
        {
            StartGameFromLevel();
        }
    }

    private void Update()
    {
        if (gameState == GameState.Play)
        {
            _currentTime += Time.deltaTime;
            gameMode.UpdateGameState(0,_currentTime);
        }
    }

    private void StartGameFromLevel()
    {
        SceneManager.LoadScene(guiScene, LoadSceneMode.Additive);
        
        Vector3 startPosition = GameObject.Find("PlayerStart").transform.position;

        Instantiate(playerAndCameraPrefab, startPosition, Quaternion.identity);

        gameState = GameState.Play;
        gameMode.InitializeMode();
        _currentTime = 0;
    }

    private void StartGameFromInitialization()
    {
        SceneManager.LoadScene("Splash");

        gameState = GameState.Start;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
        gameState = GameState.Start;
    }
    
    // Start is called before the first frame update
    public void StartGame()
    {
        SceneManager.LoadScene(guiScene);
        //SceneManager.LoadScene(locationToLoad, LoadSceneMode.Additive);

        SceneManager.LoadSceneAsync(locationToLoad, LoadSceneMode.Additive).completed += operation =>
        {
            Scene locationScene = default;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == locationToLoad)
                {
                    locationScene = SceneManager.GetSceneAt(i);
                    break;
                }
            }

            if (locationScene != default) SceneManager.SetActiveScene(locationScene);
            
            Vector3 startPosition = GameObject.Find("PlayerStart").transform.position;

            Instantiate(playerAndCameraPrefab, startPosition, Quaternion.identity);
        };

        gameState = GameState.Play;
        gameMode.InitializeMode();
        _currentTime = 0;
    }

    public void CallVictory()
    {
        SceneManager.LoadScene("Victory", LoadSceneMode.Additive);

        gameState = GameState.Victory;
    }

    public void CallGameOver()
    {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);

        gameState = GameState.GameOver;
    }

    public void LoadEnding()
    {
        SceneManager.LoadScene("Ending");

        gameState = GameState.Ending;
    }
    
    private void OnPlayerCoinsChanged(int obj)
    {
        gameMode.UpdateGameState(obj);
    }
    
    private void OnGameStateChanged(GameState obj)
    {
        switch (obj)
        {
            case GameState.Victory:
                CallVictory();
                break;
            case GameState.GameOver:
                CallGameOver();
                break;
        }
    }

    public void PlayerReachedFinishDoor()
    {
        gameMode.UpdateGameState(0,0,true);
    }

}
