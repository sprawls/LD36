using UnityEngine;
using System.Collections;

public class Brick : BreakoutPhysicObject {

    [LargeHeader("Brick Properties :")]
    public int startingHP = 1;

    [Header("Death")]
    public float deathAnimTime = 0.5f;
    public AnimationCurve deathAnimAnimCurve;

    [SerializeField]
    private int m_pointsGiven = 1000;

    private AudioSource audioSource;
    private AudioManager.AudioClipWithPitch brickStompSound;
    

    public int Health {get; private set;}

    protected virtual void Internal_OnHit() {}
    protected virtual void Internal_OnDestroy() {
        isDestroyed = true;
        RemoveBeatDetectsChildren();
        RemovePhysicsComponents();
    }

    override protected void Awake() {
        audioSource = gameObject.GetComponentInChildren<AudioSource>();
        base.Awake();
    }

	void Start () {
        
        Health = startingHP;
	}

    public void OnHit(int damage) { 
        if (damage <= 0) Debug.LogWarning("Warning : negative damage number !");
        ModifyHealth(-damage);
        //FX HERE
        AudioManager.AudioClipWithPitch brickStompSound = AudioManager.Instance.getBrickStompSound();
        audioSource.pitch = brickStompSound.pitch; //MAY CAUSE PROBLEMS WITH OTHER SOUNDS ON BRICK
        audioSource.PlayOneShot(brickStompSound.audioClip);
        Internal_OnHit();
    }

    private void ModifyHealth(int change) {
        Health += change;
        if (Health <= 0) {
            StartCoroutine(DestroyBrick());
        }
    }

    private IEnumerator DestroyBrick() {
        Internal_OnDestroy();
        ScoreController.Instance.AddRawScore(m_pointsGiven, transform);
        //FX
        float startScale = _transform.localScale.x;
        for (float i = 0; i < 1f; i += Time.deltaTime / deathAnimTime) {
            transform.localScale = Vector3.one * deathAnimAnimCurve.Evaluate(i) * startScale;
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }


}
