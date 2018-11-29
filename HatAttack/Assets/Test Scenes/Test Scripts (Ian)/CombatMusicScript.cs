using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMusicScript : MonoBehaviour {

    public AudioClip MusicIntro;
    public AudioClip MusicLoop;

    public float IntroDuration = 8.4f;
    public float LoopDuration = 82.3f;

    private AudioSource play;
    bool loop = false;

    // Use this for initialization
    void Start () {
        play = GetComponent<AudioSource>();
        StartCoroutine(Music());
	}
	
	// Update is called once per frame
	void Update () {
		if (loop)
        {
            StartCoroutine(Loop());
        }
	}

    IEnumerator<WaitForSeconds> Music()
    {
        play.PlayOneShot(MusicIntro);
        yield return new WaitForSeconds(8.4f);
        loop = true;

    }
    IEnumerator<WaitForSeconds> Loop()
    {
        loop = false;
        play.PlayOneShot(MusicLoop);
        yield return new WaitForSeconds(82.3f);
        loop = true;
    }
}
