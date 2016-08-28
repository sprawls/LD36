using UnityEngine;
using System.Collections;

public class EyeFollowPlayer : MonoBehaviour {

    private Vector3 headPosition;
    public GameObject target;
    public GameObject paddle;

    private Vector3 baseRotation;

    // Use this for initialization
    void Start () {
        baseRotation = transform.eulerAngles;
        Debug.Log(baseRotation);
    }
	
	// Update is called once per frame
	void Update () {
        headPosition = HMDController.Instance.HeadsetPosition;
        

        transform.LookAt(headPosition, Vector3.up);
        //transform.LookAt(target.transform.position, Vector3.up);

    }
}
