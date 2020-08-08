using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class candleScript : MonoBehaviour
{
    private Light candlelight;
    private AudioSource sound;
    private Animation anim;
    [SerializeField] AudioClip whoosh;

    private bool isLit;

    private void Awake()
    {
        candlelight = transform.GetChild(3).gameObject.GetComponent<Light>();
        sound = gameObject.GetComponent<AudioSource>();
        anim = gameObject.GetComponent<Animation>();
    }
    // Start is called before the first frame update
    void Start()
    {
        candlelight.gameObject.SetActive(false);
    }

    public IEnumerator EnableLight()
    {
        candlelight.gameObject.SetActive(true);
        float targetIntensity = candlelight.intensity;
        candlelight.intensity *= 2;
        sound.clip = whoosh;
        sound.Play();
        isLit = true;
        while(candlelight.intensity > targetIntensity)
        {
            candlelight.intensity -= 0.005f;
            yield return null;
        }
        candlelight.intensity = targetIntensity;
        StartCoroutine(FlickerLoop());
    }

    private IEnumerator FlickerLoop()
    {
        float originalIntensity = candlelight.intensity;
        while (isLit)
        {
            float maxValue = Random.Range(originalIntensity, originalIntensity + 2f);
            float minValue = Random.Range(originalIntensity, originalIntensity - 1f);
            while(candlelight.intensity < originalIntensity * 1.3f)
            {
                candlelight.intensity += 0.005f;
                yield return null;
            }
            while(candlelight.intensity > originalIntensity * 0.9f)
            {
                candlelight.intensity -= 0.005f;
                yield return null;
            }
        }
    }

    public void DisableLight()
    {
        //put light turning off sound here
        isLit = false;
        candlelight.gameObject.SetActive(false);
    }

    public void KnockOver()
    {
        anim.Play("candleKnockedOverAnimation");
    }
}
