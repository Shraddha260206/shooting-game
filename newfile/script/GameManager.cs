using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private bool isGameOver;
    
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel; // Direct assignment in Inspector
    private bool isPaused = false;
    
    [Header("Boss Fight")]
    private bool _bossFightActive = false;
    private int bossHitCount = 0;
    [SerializeField] private int bossRequiredHits = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to find pause panel if not assigned in Inspector
        if (pauseMenuPanel == null)
        {
            pauseMenuPanel = GameObject.Find("pause_menuPanel");
            if (pauseMenuPanel == null)
            {
                Debug.LogWarning("pause_menuPanel not found in scene! Please assign it in the Inspector.");
            }
        }
        
        // Ensure pause menu is hidden on scene load
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f; // Ensure normal speed on scene load
        isPaused = false; // Reset pause state
    }

    private void Update()
    {
        // Pause/Resume with P key (now toggles)
        if (Input.GetKeyDown(KeyCode.P) && !_bossFightActive)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // Restart with R key (if game is over)
        if (Input.GetKeyDown(KeyCode.R) && isGameOver)
        {
            RestartCurrentLevel();
        }

        // Quit with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    private void PauseGame()
    {
        if (pauseMenuPanel == null)
        {
            Debug.LogError("Cannot pause: Pause Menu Panel is not assigned!");
            return;
        }

        Debug.Log("Game Paused");
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        
        // Set cursor to be visible and unlocked
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel == null)
        {
            Debug.LogError("Cannot resume: Pause Menu Panel is not assigned!");
            return;
        }

        Debug.Log("Game Resumed");
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        
        // Lock cursor again if needed for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartCurrentLevel()
    {
        Debug.Log("Restarting Scene...");
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(0); // Assuming main menu is scene 0
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over called!");
    }

    public int GetScore()
    {
        if (player.Instance != null)
        {
            return player.Instance.GetScore();
        }
        return 0;
    }

    // Public getters
    public bool IsGameOver() => isGameOver;
    public bool IsPaused() => isPaused;
    public bool BossFightActive => _bossFightActive;

    public void StartBossFight()
    {
        _bossFightActive = true;
        bossHitCount = 0;

        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            // Fixed tag consistency - using lowercase
            if (obj.CompareTag("player") || obj.CompareTag("laser") || obj.CompareTag("boss")) continue;

            if (obj.TryGetComponent<Rigidbody2D>(out var rb))
                rb.simulated = false;

            if (obj.TryGetComponent<Animator>(out var anim))
                anim.enabled = false;

            var allMonoBehaviours = obj.GetComponents<MonoBehaviour>();
            foreach (var mb in allMonoBehaviours)
            {
                if (mb != null)
                    mb.enabled = false;
            }
        }

        GameObject boss = GameObject.FindWithTag("boss");
        if (boss != null)
        {
            Rigidbody2D rb = boss.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = new Vector2(0, -0.5f);
        }
    }

    public void EndBossFight()
    {
        _bossFightActive = false;

        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            // Fixed tag consistency - using proper case
            if (obj.CompareTag("player") || obj.CompareTag("laser") || obj.CompareTag("boss")) continue;

            if (obj.TryGetComponent<Rigidbody2D>(out var rb))
                rb.simulated = true;

            if (obj.TryGetComponent<Animator>(out var anim))
                anim.enabled = true;

            var allMonoBehaviours = obj.GetComponents<MonoBehaviour>();
            foreach (var mb in allMonoBehaviours)
            {
                if (mb != null)
                    mb.enabled = true;
            }
        }
    }

    public void BossHit()
    {
        if (!_bossFightActive) return;

        bossHitCount++;
        Debug.Log($"Boss hit {bossHitCount}/{bossRequiredHits}");

        if (bossHitCount >= bossRequiredHits)
        {
            Debug.Log("Boss defeated!");
            EndBossFight();
            GameObject boss = GameObject.FindWithTag("boss");
            if (boss != null)
                Destroy(boss);
        }
    }

    public int GetBossHitCount() => bossHitCount;
    public int GetBossRequiredHits() => bossRequiredHits;

    public void LoadSceneIndex1()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(1);
    }
}
