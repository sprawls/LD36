using UnityEngine;
using System.Collections;

public class BallGenerator : MonoBehaviour {

    public GameObject BallPrefab;
    public float spawnInterval = 5f;

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnBalls());
	}

    IEnumerator SpawnBalls() {
        while (true) {
            yield return new WaitForSeconds(spawnInterval);
            SpawnBall();
        }
    }

    void SpawnBall() {
        Instantiate(BallPrefab, transform.position, Quaternion.identity);
    }
}
