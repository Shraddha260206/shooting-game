using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score_text;
    [SerializeField] private TextMeshProUGUI bestScore_text;
    [SerializeField] private TextMeshProUGUI restart_text;
    [SerializeField] private TextMeshProUGUI endGame_text;

    [SerializeField] private Sprite[] liveSprites;
    [SerializeField] private Image LivesImg;

    [SerializeField] private GameManager gameManager;

    public int score;
    public int best;

    void Start()
    {
        // Load best score first
        best = PlayerPrefs.GetInt("BestScore", 0);
        
        // Initialize score
        updateScore(0);
        
        // Update best score display
        UpdateBestScoreDisplay();

        endGame_text.gameObject.SetActive(false);
        restart_text.gameObject.SetActive(false);

        // Find GameManager if not assigned
        if (gameManager == null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found or not connected");
            }
        }
    }

    public void updateScore(int playerScore)
    {
        score = playerScore;
        if (score_text != null)
        {
            score_text.text = "Score: " + score;
        }

        // Check and update best score in real-time
        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt("BestScore", best);
            PlayerPrefs.Save(); // Force save immediately
            UpdateBestScoreDisplay();
        }
    }

    // Separate method to update best score display
    private void UpdateBestScoreDisplay()
    {
        if (bestScore_text != null)
        {
            bestScore_text.text = "Best: " + best;
        }
        else
        {
            Debug.LogWarning("Best Score Text UI element is not assigned!");
        }
    }

    public void BestScore()
    {
        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt("BestScore", best);
            PlayerPrefs.Save(); // Force save
        }
        UpdateBestScoreDisplay();
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives >= 0 && currentLives < liveSprites.Length && LivesImg != null)
        {
            LivesImg.sprite = liveSprites[currentLives];
        }

        if (currentLives == 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        // Call BestScore to ensure it's saved before game over
        BestScore();
        
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        
        if (endGame_text != null)
        {
            endGame_text.gameObject.SetActive(true);
        }
        
        if (restart_text != null)
        {
            restart_text.gameObject.SetActive(true);
        }
        
        StartCoroutine(endGameFlickerRoutine());
    }

    IEnumerator endGameFlickerRoutine()
    {
        while (endGame_text != null && endGame_text.gameObject.activeInHierarchy)
        {
            endGame_text.text = "The Donut Rolls on....";
            yield return new WaitForSeconds(0.5f);
            
            if (endGame_text != null && endGame_text.gameObject.activeInHierarchy)
            {
                endGame_text.text = "MISSION FAILED!!!";
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void ResumePlay()
    {
        if (gameManager != null)
        {
            gameManager.ResumeGame();
        }
        else
        {
            GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (gm != null)
            {
                gm.ResumeGame();
            }
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
