using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gestor de spawn de enemigos
/// Controla cuándo y dónde aparecen los enemigos en cada ronda
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnDelay = 0.5f;

    [Header("Target")]
    [SerializeField] private Transform playerTargetTransform;

    [Header("Área de Spawn")]
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(10, 0, 10);
    [SerializeField] private bool useSpawnPoints = true;

    private List<GameObject> activeEnemies = new List<GameObject>();

    /// <summary>
    /// Spawnea la cantidad especificada de enemigos para la ronda
    /// </summary>
    public void SpawnEnemiesForRound(int enemyCount)
    {
        activeEnemies.Clear();
        StartCoroutine(SpawnCoroutine(enemyCount));
    }

    private IEnumerator SpawnCoroutine(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnSingleEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: No se asigno enemyPrefab");
            return;
        }

        Vector3 spawnPosition = GetSpawnPosition();
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Asignar target si es posible
        AssignTarget(newEnemy);
        /*  EnemyCarNavMesh enemyNav = newEnemy.GetComponent<EnemyCarNavMesh>();
         if (enemyNav == null)
         {
             Debug.LogError("EnemySpawner: El prefab no tiene EnemyCarNavMesh");
         }
         else if (playerTargetTransform == null)
         {
             Debug.LogWarning("EnemySpawner: targetTransform no esta asignado");
         }
         else
         {
             enemyNav.SetTarget(playerTargetTransform);
         } */

        activeEnemies.Add(newEnemy);

        Debug.Log($"Enemigo spawneado en {spawnPosition}");
    }

    /*  public void SpawnBoss(GameObject bossPrefab)
     {
         if (bossPrefab == null)
         {
             Debug.LogWarning("BossPrefab es null");
             return;
         }

         AssignTarget(bossPrefab);
         Vector3 spawnPos = GetSpawnPosition(); // o el método que ya uses
         Instantiate(bossPrefab, spawnPos, Quaternion.identity);
     } */

    public void SpawnBoss(GameObject bossPrefab)
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("BossPrefab es null");
            return;
        }

        Vector3 spawnPos = GetSpawnPosition();
        GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        AssignTarget(boss);
        activeEnemies.Add(boss);

        Debug.Log("Boss spawneado y target asignado");
    }


    private void AssignTarget(GameObject enemy)
    {
        EnemyCarNavMesh enemyNav = enemy.GetComponent<EnemyCarNavMesh>();

        if (enemyNav == null)
        {
            Debug.LogError("EnemySpawner: El prefab no tiene EnemyCarNavMesh");
            return;
        }

        if (playerTargetTransform == null)
        {
            Debug.LogWarning("EnemySpawner: targetTransform no esta asignado");
            return;
        }

        enemyNav.SetTarget(playerTargetTransform);
    }


    private Vector3 GetSpawnPosition()
    {
        if (useSpawnPoints && spawnPoints != null && spawnPoints.Length > 0)
        {
            // Elegir un punto de spawn al azar
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return randomSpawnPoint.position;
        }
        else
        {
            // Generar posición aleatoria en el área de spawn
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            return spawnAreaCenter + randomOffset;
        }
    }

    public int GetActiveEnemyCount()
    {
        // Limpiar referencias nulas
        activeEnemies.RemoveAll(enemy => enemy == null);
        return activeEnemies.Count;
    }

    public List<GameObject> GetActiveEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
        return new List<GameObject>(activeEnemies);
    }

    private void OnDrawGizmos()
    {
        if (useSpawnPoints)
        {
            // Dibujar puntos de spawn
            if (spawnPoints != null)
            {
                Gizmos.color = Color.green;
                foreach (Transform point in spawnPoints)
                {
                    if (point != null)
                    {
                        Gizmos.DrawWireSphere(point.position, 0.5f);
                    }
                }
            }
        }
        else
        {
            // Dibujar área de spawn
            Gizmos.color = Color.cyan;
            DrawWireCube(spawnAreaCenter, spawnAreaSize);
        }
    }

    private void DrawWireCube(Vector3 center, Vector3 size)
    {
        Vector3 halfSize = size / 2;

        // Dibujar líneas
        Vector3[] corners = new Vector3[8];
        corners[0] = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
        corners[1] = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
        corners[2] = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
        corners[3] = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);
        corners[4] = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
        corners[5] = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
        corners[6] = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
        corners[7] = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);

        // Tapa superior
        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);

        // Tapa inferior
        Gizmos.DrawLine(corners[4], corners[5]);
        Gizmos.DrawLine(corners[5], corners[6]);
        Gizmos.DrawLine(corners[6], corners[7]);
        Gizmos.DrawLine(corners[7], corners[4]);

        // Pilares
        Gizmos.DrawLine(corners[0], corners[4]);
        Gizmos.DrawLine(corners[1], corners[5]);
        Gizmos.DrawLine(corners[2], corners[6]);
        Gizmos.DrawLine(corners[3], corners[7]);
    }
}
