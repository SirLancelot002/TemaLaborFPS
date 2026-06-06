using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private float spawnInterval = 5f;

    private IEnumerator SpawnEnemies(float interval, GameObject enemy)
    {
        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)), Quaternion.identity);
        StartCoroutine(SpawnEnemies(interval, enemy));
    }
    void Start()
    {
        StartCoroutine(SpawnEnemies(spawnInterval, enemyPrefab));
    }

    void Update()
    {
        
    }
}
