using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour {

    public AudioClip Intro;
    public AudioClip Loop;

    public float introDuration;
    public float loopDuration;

    private AudioSource audio;
    private bool loop = false;

    // Use this for initialization
    void Start () {
        audio = GetComponent<AudioSource>();
        StartCoroutine(playIntro());
	}
	
	// Update is called once per frame
	void Update () {
        if (loop)
        {
            StartCoroutine(playLoop());
        }
    }

    IEnumerator<WaitForSeconds> playIntro()
    {
        audio.PlayOneShot(Intro);
        yield return new WaitForSeconds(introDuration);
        loop = true;
    }

    IEnumerator<WaitForSeconds> playLoop()
    {
        loop = false;
        audio.PlayOneShot(Loop);
        yield return new WaitForSeconds(loopDuration);
        loop = true;
    }
}
