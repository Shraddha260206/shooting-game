using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyprefab;
    [SerializeField] private GameObject enemycontainer;
    [SerializeField] private GameObject[] powerups;

    [SerializeField] private GameObject marshmallowFullPrefab;  // Optional: used if you want access elsewhere

    private bool stopspawning = false;
    private player _player;

    private const float SCREEN_LEFT = -9.3627f;
    private const float SCREEN_RIGHT = 9f;
    private const float SCREEN_TOP = 6.241f;

    void Start()
    {
        _player = GameObject.Find("player")?.GetComponent<player>();
        if (_player == null)
        {
            Debug.LogError("Player not found! Make sure the player GameObject is named 'player'");
        }

        StartCoroutine(spawnRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(BossSpawnRoutine());
    }

    IEnumerator spawnRoutine()
    {
        while (!stopspawning)
        {
            Vector3 pos = new Vector3(Random.Range(SCREEN_LEFT + 0.1f, SCREEN_RIGHT - 0.1f), SCREEN_TOP - 0.05f, 0);

            int enemyIndex = 0;

            if (_player != null)
            {
                int score = _player.GetScore();

                if (score > 200)
                {
                    int[] spawnIndices = new int[] { 0, 1 };
                    System.Collections.Generic.List<int> valid = new System.Collections.Generic.List<int>();
                    foreach (int i in spawnIndices)
                    {
                        if (enemyprefab != null && enemyprefab.Length > i && enemyprefab[i] != null)
                        {
                            valid.Add(i);
                        }
                    }
                    if (valid.Count > 0)
                    {
                        enemyIndex = valid[Random.Range(0, valid.Count)];
                    }
                }
            }

            if (enemyprefab != null && enemyprefab.Length > enemyIndex && enemyprefab[enemyIndex] != null)
            {
                GameObject newEnemy = Instantiate(enemyprefab[enemyIndex], pos, Quaternion.identity);

                if (enemycontainer != null)
                {
                    newEnemy.transform.parent = enemycontainer.transform;
                }

                // Marshmallow is already set up in prefab via Inspector.
            }

            yield return new WaitForSeconds(3.0f);
        }
    }

    IEnumerator BossSpawnRoutine()
    {
        while (!stopspawning)
        {
            if (_player != null && _player.GetScore() >= 300)
            {
                float bossSpawnChance = 0.05f;
                if (Random.value < bossSpawnChance)
                {
                    Vector3 pos = new Vector3(Random.Range(SCREEN_LEFT + 0.1f, SCREEN_RIGHT - 0.1f), SCREEN_TOP - 0.05f, 0);
                    int bossEnemyIndex = 2;

                    if (enemyprefab != null && enemyprefab.Length > bossEnemyIndex && enemyprefab[bossEnemyIndex] != null)
                    {
                        GameObject boss = Instantiate(enemyprefab[bossEnemyIndex], pos, Quaternion.identity, enemycontainer != null ? enemycontainer.transform : null);
                        Enemy enemyComponent = boss.GetComponent<Enemy>();
                        if (enemyComponent != null)
                        {
                            enemyComponent.SetAsBoss();
                        }
                    }
                }
            }

            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (!stopspawning)
        {
            Vector3 pos = new Vector3(Random.Range(SCREEN_LEFT + 0.5f, SCREEN_RIGHT - 0.5f), SCREEN_TOP - 0.05f, 0);

            if (powerups != null && powerups.Length > 0)
            {
                int powerupIndex = Random.Range(0, powerups.Length);
                if (powerups[powerupIndex] != null)
                {
                    Instantiate(powerups[powerupIndex], pos, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(Random.Range(15f, 30f));
        }
    }

    public void onplayerDeath()
    {
        stopspawning = true;
    }

    public void UpdateEnemyPrefabs(GameObject[] newEnemyPrefabs)
    {
        if (newEnemyPrefabs != null && newEnemyPrefabs.Length > 0)
        {
            enemyprefab = newEnemyPrefabs;
        }
    }
}
