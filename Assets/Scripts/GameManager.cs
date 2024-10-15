using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static private GameManager instance = null;
    static public GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
    }

    public int EnemyNumber;
    public bool IsPlaying;
    public GameObject GameOverCanvas;
    public TMP_Text title;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsPlaying = true;
    }

    public void PlayerDie()
    {
        title.text = "You Died";
        GameEnd();
    }

    public void GameEnd()
    {
        IsPlaying = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameOverCanvas.SetActive(true);
    }

    public void EnemyDie()
    {
        EnemyNumber--;

        if (EnemyNumber <= 0)
        {
            title.text = "You Win";
            GameEnd();
        }
    }

    public void AgainPressed()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitPressed()
    {
        Application.Quit();
    }
}
