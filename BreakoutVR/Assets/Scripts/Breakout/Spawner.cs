using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public enum SpawnType
    {
        Ball,
    }

	[SerializeField]
    private GameObject m_objectToSpawn;

    [SerializeField]
    private SpawnType m_spawnType;

    public SpawnType Type { get { return m_spawnType; } }

    public GameObject Spawn()
    {
        return Instantiate(m_objectToSpawn, transform.position, Quaternion.identity) as GameObject;
    }
}
