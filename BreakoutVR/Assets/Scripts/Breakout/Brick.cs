using UnityEngine;
using System.Collections;

public class Brick : BreakoutPhysicObject {

    [LargeHeader("Brick Properties :")]
    public int startingHP = 1;

    [SerializeField]
    private int m_pointsGiven = 1000;

    public int Health {get; private set;}

    protected virtual void Internal_OnHit() {}
    protected virtual void Internal_OnDestroy() {}

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
        Internal_OnHit();
    }

    private void ModifyHealth(int change) {
        Health += change;
        if (Health <= 0) {
            StartCoroutine(DestroyBrick());
        }
    }

    private IEnumerator DestroyBrick() {
        //FX HERE
        Internal_OnDestroy();
        ScoreController.Instance.AddRawScore(m_pointsGiven, transform);
        Destroy(gameObject);
        yield return null;
    }


}
