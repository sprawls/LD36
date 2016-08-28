using UnityEngine;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip brickStompSound;
    [SerializeField] private AudioClip generalBallHitSound;
    [SerializeField] private AudioClip paddleHitSound;
    [SerializeField] private AudioClip paddleHitLouderSound;

    private float timeBetweenBrickHitBonus = 2.0f;
    private float pitch = 1.0f;
    private float pitchDiff = 0.03f;
    private float pitchMax = 1.3f;

    public struct AudioClipWithPitch{
        public AudioClip audioClip;
        public float pitch;

        public AudioClipWithPitch(AudioClip _audioClip, float _pitch)
        {
            audioClip = _audioClip;
            pitch = _pitch;
        }
       
    }

    void Start () {
	
	}

    public AudioClip getGeneralBallHitSound() { return generalBallHitSound; }

    public AudioClip getPaddleHitSound() { return paddleHitSound; }

    public AudioClip getPaddleHitLouderSound() { return paddleHitLouderSound; }

    public AudioClipWithPitch getBrickStompSound() {
        float pitchToSend = pitch;
        pitch += pitchDiff;
        if (pitch >= pitchMax) pitch = pitchMax;
        StopCoroutine(resetPitchIfTimeOut(timeBetweenBrickHitBonus));
        StartCoroutine(resetPitchIfTimeOut(timeBetweenBrickHitBonus));
        return new AudioClipWithPitch(brickStompSound, pitchToSend);
    }

    IEnumerator resetPitchIfTimeOut(float timeout)
    {
        yield return new WaitForSeconds(timeout);
        pitch = 1.0f;
    }
}
