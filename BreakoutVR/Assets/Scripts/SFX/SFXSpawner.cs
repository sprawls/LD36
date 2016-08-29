using UnityEngine;
using System.Collections;

public class SFXSpawner : MonoBehaviour {

    //Moving Light settings
    public GameObject movingLightPrefab;
    private GameObject movingLight;
    private Vector3 spawnPosition;

    void Start()
    {
        StartCoroutine(SpawnMovingLightPeriodically());
    }

    IEnumerator SpawnMovingLightPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 8.0f));
            spawnPosition = new Vector3(0, 1f, -2f);
            movingLight = (GameObject)Instantiate(movingLightPrefab, spawnPosition, Quaternion.identity);
        }

    }

    

}
