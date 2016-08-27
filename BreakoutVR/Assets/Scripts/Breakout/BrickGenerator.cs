using UnityEngine;
using System.Collections;

public class BrickGenerator : MonoBehaviour {

    public GameObject BrickGameObject;

    public int rowsToSpawn = 0;
    public int bricksPerRows = 0;
    public float brickScale = 1;

	void Start () {
        SpawnBricks();
	}
	
	void SpawnBricks () {
        Vector3 brickPosition = transform.position;
        for (int row = 0; row < rowsToSpawn; ++row) {
            brickPosition += transform.up * brickScale * row;
            for (int brick = 0; brick < bricksPerRows; ++brick) {
                GameObject spawnedBrick = (GameObject)Instantiate(BrickGameObject, brickPosition, Quaternion.identity); 
                spawnedBrick.transform.localScale = new Vector3(brickScale, brickScale, brickScale);
                spawnedBrick.transform.rotation = transform.rotation;
                brickPosition += transform.right * brickScale * 2f;
                spawnedBrick.transform.parent = transform;
            }
            brickPosition = transform.position;
           
        }
	}
}
