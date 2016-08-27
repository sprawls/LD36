using UnityEngine;
using System.Collections;

public class Brick : BreakoutPhysicObject {

    [Header("Brick Properties :")]
    public int startingHP = 1;

    public int Health {get; private set;}


    override protected void Awake() {
        base.Awake();
    }

	void Start () {
        Health = startingHP;
	}

    public void OnHit(int damage) {
        if (damage <= 0) Debug.LogWarning("Warning : negative damage number !");
        ModifyHealth(-damage);
        //FX HERE
    }

    private void ModifyHealth(int change) {
        Health += change;
        if (Health <= 0) {
            StartCoroutine(DestroyBrick());
        }
    }

    private IEnumerator DestroyBrick() {
        //FX HERE
        Destroy(gameObject);
        yield return null;
    }


}
