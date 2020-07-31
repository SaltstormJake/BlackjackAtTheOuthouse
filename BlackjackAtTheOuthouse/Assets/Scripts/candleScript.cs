using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class candleScript : MonoBehaviour
{
    private GameObject candlelight;
    private AudioSource sound;
    private Animation anim;
    [SerializeField] AudioClip whoosh;
    private void Awake()
    {
        candlelight = transform.GetChild(3).gameObject;
        sound = gameObject.GetComponent<AudioSource>();
        anim = gameObject.GetComponent<Animation>();
    }
    // Start is called before the first frame update
    void Start()
    {
        candlelight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableLight()
    {
        candlelight.SetActive(true);
        sound.clip = whoosh;
        sound.Play();
    }

    public void DisableLight()
    {
        //put light turning off sound here
        candlelight.SetActive(false);
    }

    public void KnockOver()
    {
        anim.Play("candleKnockedOverAnimation");
    }
}
