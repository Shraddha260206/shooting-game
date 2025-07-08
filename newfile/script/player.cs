using System.Collections;
using UnityEngine;

public class player : MonoBehaviour
{
    
    public static player Instance { get; private set; }

    [SerializeField] private GameObject _LaserPrefab;
    [SerializeField] private int score;
    [SerializeField] private GameObject shieldVisualiser;
    [SerializeField] private float firerate = 0.5f;
    [SerializeField] private int lives = 3;
    [SerializeField] private AudioClip laseraudio;
    [SerializeField] private AudioSource laserAudioSource;

    private float speed = 5.0f;
    private float speedMultiplier = 2.0f;
    private bool _isShieldActive = false;
    private float canfire = -1f;
    private float _originalFireRate;
    private SpawnManager spawnmanager;
    private UI_Manager uiManager;

    void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        transform.position = Vector3.zero;
        spawnmanager = GameObject.Find("spawnManager")?.GetComponent<SpawnManager>();
        uiManager = GameObject.Find("Canvas")?.GetComponent<UI_Manager>();
        laserAudioSource = GetComponent<AudioSource>();
        _originalFireRate = firerate;

        if (shieldVisualiser != null)
        {
            shieldVisualiser.SetActive(false);
        }
        if (laserAudioSource == null)
        {
            Debug.LogError("audio source is not connected");
        }
        else
        {
            laserAudioSource.clip = laseraudio;
        }
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > canfire)
        {
            fireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * speed * Time.deltaTime);

        // Screen boundaries
        if (transform.position.x < -9.3627f)
            transform.position = new Vector3(9, transform.position.y, 0);
        else if (transform.position.x > 9)
            transform.position = new Vector3(-9.3627f, transform.position.y, 0);
        else if (transform.position.y > 6.241f)
            transform.position = new Vector3(transform.position.x, -6.24f, 0);
        else if (transform.position.y < -6.321f)
            transform.position = new Vector3(transform.position.x, 6.241f, 0);
    }

    void fireLaser()
    {
        canfire = Time.time + firerate;
        Instantiate(_LaserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
        laserAudioSource.Play();
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            shieldVisualiser?.SetActive(false);
            return;
        }

        lives--;
        uiManager?.UpdateLives(lives);

        if (lives < 1)
        {
            spawnmanager?.onplayerDeath();
            Destroy(gameObject);
            uiManager.BestScore();
        }
    }

    public void speedBoastActive()
    {
        speed *= speedMultiplier;
        firerate /= speedMultiplier;
        StartCoroutine(speedBoastPowerDownRoutine());
    }

    IEnumerator speedBoastPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        speed /= speedMultiplier;
        firerate = _originalFireRate;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        shieldVisualiser?.SetActive(true);
    }

    public void Addscore(int points)
    {
        score += points;
        uiManager?.updateScore(score);
        if (score >= 200 && score - points < 200)
        {
            Debug.Log("Score reached 200! Strong enemies will now spawn!");
        }
    }

    public int GetScore()
    {
        return score;
    }

    void OnDestroy()
    {
        // Clear the instance when the object is destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
