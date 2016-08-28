using UnityEngine;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip paddleSmashSound;
    [SerializeField] private AudioClip brickStompSound;

    void Start () {
	
	}
	
    public AudioClip getPaddleSmashSound() { return paddleSmashSound; }
    public AudioClip getBrickStompSound() { return brickStompSound; }
}
