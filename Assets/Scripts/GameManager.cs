using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject EnemyPrefeb;
    void Start()
    {
        StartCoroutine(SpawnEnemys());
    }

    private IEnumerator SpawnEnemys()
    {
        var randx = Random.Range(-9.7f, 9.7f);
        var randz = Random.Range(-9.7f, 9.7f);

        Instantiate(EnemyPrefeb, new Vector3(randx, 14, randz), Quaternion.identity);

        yield return new WaitForSeconds(1f);

        yield return SpawnEnemys();
    }
}
