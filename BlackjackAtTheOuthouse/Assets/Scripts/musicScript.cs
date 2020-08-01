using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicScript : MonoBehaviour
{
    AudioSource sound;
    [SerializeField] AudioClip music;
    [SerializeField] AudioClip recordScratch;
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
        sound.clip = music;
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

    public void RecordScratch()
    {
        sound.Stop();
        sound.loop = false;
        sound.clip = recordScratch;
        sound.Play();
    }

    public IEnumerator FadeOut(float time)
    {
        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            sound.volume = Mathf.Lerp(1, 0, timer / 10);
            yield return null;
        }
        sound.Stop();
    }
}
