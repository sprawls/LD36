using UnityEngine;
using System.Collections;

public class EyeFollowPlayer : MonoBehaviour {

    private Vector3 headPosition;
    //public GameObject target;
    public GameObject paddle;
  
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        headPosition = HMDController.Instance.HeadsetPosition;

        transform.LookAt(headPosition, Vector3.up);

    }
}
