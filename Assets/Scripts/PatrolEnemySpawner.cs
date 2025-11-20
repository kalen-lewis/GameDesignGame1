using UnityEngine;

public class PatrolEnemySpawner : MonoBehaviour
{
    public GameObject patrolEnemyPrefab;
    public Transform[] spawnLocations;
    public int maxEnemies = 5;
    public float spawnInterval = 10f;

    private float timer = 0f;
    private int currentEnemyCount = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnLocations.Length);
        Instantiate(patrolEnemyPrefab, spawnLocations[randomIndex].position, Quaternion.identity);
        currentEnemyCount++;
    }
}
