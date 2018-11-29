using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMusicLoop : MonoBehaviour
{

    public AudioClip MusicIntro;
    public AudioClip MusicLoop;

    public float IntroDuration = 4.5f;
    public float LoopDuration = 128f;

    private AudioSource play;
    bool loop = false;

    // Use this for initialization
    void Start()
    {
        play = GetComponent<AudioSource>();
        StartCoroutine(Music());
    }

    // Update is called once per frame
    void Update()
    {
        if (loop)
        {
            StartCoroutine(Loop());
        }
    }

    IEnumerator<WaitForSeconds> Music()
    {
        play.PlayOneShot(MusicIntro);
        yield return new WaitForSeconds(IntroDuration);
        loop = true;

    }
    IEnumerator<WaitForSeconds> Loop()
    {
        loop = false;
        play.PlayOneShot(MusicLoop);
        yield return new WaitForSeconds(LoopDuration);
        loop = true;
    }
}
