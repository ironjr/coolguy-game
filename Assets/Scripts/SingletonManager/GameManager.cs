using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public uint Score = 0u;
    public uint PersonalHighScore = 0u;
    public int CurrentThemeID = 0;

    public enum GameState
    {
        Running,
        Over
    }

    private GameState _state = GameState.Running;
    private static JsonManager _jsonManager;
    private static LayoutManager _layoutManager;

    public bool IsOver
    {
        get
        {
            return _state == GameState.Over;
        }
    }

    public bool IsRunning
    {
        get
        {
            return _state == GameState.Running;
        }
    }

    protected override void OnAwake()
    {
        _jsonManager = JsonManager.Instance;
        _layoutManager = LayoutManager.Instance;
    }

    void Start()
    {
        _jsonManager.LoadData();
        PersonalHighScore = _jsonManager.PersonalHighScore;
        CurrentThemeID = _jsonManager.StartThemeID;
        _layoutManager.DisplayScore((int)Score);
	}

    void Update()
    {
        switch (_state)
        {
            case GameState.Running:
                if (Input.GetKey(KeyCode.Escape))
                {
                    Application.Quit();
                }
                break;
            case GameState.Over:
                if (Input.GetKey(KeyCode.Escape))
                {
                    Application.Quit();
                }
                bool restart = Input.GetKey(KeyCode.R);
                if (restart)
                {
                    RestartGame();
                }
                bool reset = Input.GetKey(KeyCode.Home);
                if (reset)
                {
                    ResetHighScore();
                }
                break;
        }
    }

    public void GainScore(int gainAmount)
    {
        // Update local player score.
        Score = (uint)((int)Score + gainAmount);

        // Display updated score.
        _layoutManager.DisplayScore((int)Score);
    }

    public void RestartGame()
    {
        // Reloads current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        _state = GameState.Over;
        if (PersonalHighScore < Score)
        {
            PersonalHighScore = Score;
            _jsonManager.PersonalHighScore = PersonalHighScore;
            _jsonManager.SaveData();
        }
        StartCoroutine(WaitedDisplaySummary());
        Debug.Log("Game is over!");
    }

    private void ResetHighScore()
    {
        Score = 0u;
        PersonalHighScore = 0u;
        _jsonManager.PersonalHighScore = 0u;
        _jsonManager.SaveData();
    }

    public IEnumerator WaitedDisplaySummary()
    {
        yield return new WaitForSeconds(0.9f);
        _layoutManager.DisplayGameOver(Score, PersonalHighScore);
    }
}
