using UnityEngine;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip paddleSmashSound;
    [SerializeField] private AudioClip brickStompSound;

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
	
    public AudioClip getPaddleSmashSound() { return paddleSmashSound; }

    public AudioClipWithPitch getBrickStompSound() {
        float pitchToSend = pitch;
        pitch += pitchDiff;
        if (pitch >= pitchMax) pitch = pitchMax;
        //Debug.Log(pitchToSend);
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
