using UnityEngine;
using System.Collections;

public abstract class PowerUpAbstract : MonoBehaviour {

    abstract protected void Awake();
    abstract protected void Start();
    abstract protected void Update();
    abstract public void Activate(HMDController.ControllerIndex cIndex);
}
