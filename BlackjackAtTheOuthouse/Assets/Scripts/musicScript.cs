using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicScript : MonoBehaviour
{
    AudioSource sound;
    [SerializeField] AudioClip music;
    [SerializeField] KeyCode testKey;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sound.clip = music;
        //PlayMusic();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayMusic()
    {
        sound.Play();
        sound.loop = true;
    }

    public void Pause()
    {
        if (sound.isPlaying)
            sound.Pause();
        else
            sound.UnPause();
    }
}
