using UnityEngine;
using System.Collections;

public class SFXSpawner : Singleton<SFXSpawner> {

    //Moving Light settings
    public GameObject movingLightPrefab;
    private GameObject movingLight;
    private Vector3 spawnPosition;

    void Start()
    {
        spawnPosition = new Vector3(0, 1f, -2f);
    }

    public void spawnMovingLight()
    {
        movingLight = (GameObject)Instantiate(movingLightPrefab, spawnPosition, Quaternion.identity);
    }

    IEnumerator SpawnMovingLightPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 8.0f));
            movingLight = (GameObject)Instantiate(movingLightPrefab, spawnPosition, Quaternion.identity);
        }

    }

    

}
