using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private player _player;
    private int _hitCount = 0;
    private bool _isStrongEnemy = false;
    private bool _isRareEnemy = false;
    private bool _isBossEnemy = false;
    private bool _isFrozen = false;

    [SerializeField] private float _moveSpeed = 5f;
    private float _bossMoveSpeed = 2f;
    private int _bossScoreValue = 100;

    public void SetAsStrongEnemy()
    {
        _isStrongEnemy = true;
        Debug.Log("Enemy set to require 2 hits to destroy.");
    }

    public void SetAsRareEnemy()
    {
        _isRareEnemy = true;
        Debug.Log("Rare enemy spawned! 10 hits in 5 seconds required or player dies!");
        StartCoroutine(RareEnemyTimer());
    }

    public void SetAsBoss()
    {
        _isBossEnemy = true;
        gameObject.tag = "boss";
        _moveSpeed = _bossMoveSpeed;
        
        Debug.Log("Boss donut spawned! Must hit 10 times to clear level!");
        GameManager.Instance?.StartBossFight();
        
        // Boss fight timer - if boss reaches bottom, player loses
        StartCoroutine(BossFailureTimer());
    }

    public void FreezeEnemy()
    {
        _isFrozen = true;
    }

    public void UnfreezeEnemy()
    {
        _isFrozen = false;
    }

    void Start()
    {
        _player = player.Instance;
    }

    void Update()
    {
        // Don't move if frozen (unless it's the boss)
        if (_isFrozen && !_isBossEnemy)
            return;

        // Boss always moves during boss fight
        float currentSpeed = _isBossEnemy ? _bossMoveSpeed : _moveSpeed;
        transform.Translate(Vector3.down * Time.deltaTime * currentSpeed);

        // Check if enemy reached bottom of screen
        if (transform.position.y < -6.3f)
        {
            if (_isBossEnemy)
            {
                // Boss reached bottom - player loses
                Debug.Log("Boss donut reached bottom! Game Over!");
                _player?.Damage();
                _player?.Damage();
                _player?.Damage();
                GameManager.Instance?.EndBossFight();
                Destroy(gameObject);
                return;
            }
            
            if (_isRareEnemy && !_isBossEnemy)
            {
                _player?.Damage();
                _player?.Damage();
                _player?.Damage();
                Destroy(gameObject);
                return;
            }

            // Regular enemy - respawn at top
            transform.position = new Vector3(Random.Range(-9f, 9f), 6.3f, 0);
        }
    }

    private IEnumerator RareEnemyTimer()
    {
        yield return new WaitForSeconds(5f);

        if (_isRareEnemy && !_isBossEnemy && gameObject != null)
        {
            _player?.Damage();
            _player?.Damage();
            _player?.Damage();
            Destroy(gameObject);
        }
    }

    private IEnumerator BossFailureTimer()
    {
        // Give player reasonable time to defeat boss
        yield return new WaitForSeconds(15f);

        if (gameObject != null && _isBossEnemy)
        {
            Debug.Log("Boss fight timed out! Player loses!");
            _player?.Damage();
            _player?.Damage();
            _player?.Damage();
            GameManager.Instance?.EndBossFight();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            _player?.Damage();
            
            if (_isBossEnemy)
            {
                GameManager.Instance?.EndBossFight();
            }
            
            Destroy(gameObject);
        }

        if (other.CompareTag("laser"))
        {
            Destroy(other.gameObject);
            _hitCount++;

            // Handle boss hits
            if (_isBossEnemy)
            {
                GameManager.Instance?.BossHit();
                
                // Check if boss is defeated (GameManager handles this)
                if (GameManager.Instance.GetBossHitCount() >= GameManager.Instance.GetBossRequiredHits())
                {
                    Debug.Log("Boss defeated! Adding score...");
                    _player?.Addscore(_bossScoreValue);
                    Destroy(gameObject);
                }
                return;
            }

            // Handle rare enemy hits
            if (_isRareEnemy)
            {
                Debug.Log($"Rare enemy hit {_hitCount}/10");

                if (_hitCount >= 10)
                {
                    Debug.Log("Rare enemy destroyed!");
                    _player?.Addscore(50);
                    Destroy(gameObject);
                }
                return;
            }

            // Handle strong enemy hits
            if (_isStrongEnemy && _hitCount < 2)
            {
                Debug.Log($"Strong enemy hit {_hitCount}/2");
                return;
            }

            // Regular enemy or strong enemy with enough hits
            int scoreValue = _isStrongEnemy ? 20 : 10;
            _player?.Addscore(scoreValue);
            Destroy(gameObject);
        }
    }
}
