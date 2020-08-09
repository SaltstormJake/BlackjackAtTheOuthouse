using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class candleScript : MonoBehaviour
{
    private Light candlelight;
    private AudioSource sound;
    private Animation anim;
    private ParticleSystem flame;
    [SerializeField] AudioClip whoosh;
    [SerializeField] AudioClip burning;
    [SerializeField] AudioClip blowOut;

    private bool isLit;

    private void Awake()
    {
        candlelight = transform.GetChild(3).gameObject.GetComponent<Light>();
        sound = candlelight.gameObject.GetComponent<AudioSource>();
        anim = gameObject.GetComponent<Animation>();
        flame = candlelight.gameObject.GetComponent<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start()
    {
        candlelight.gameObject.SetActive(false);
        flame.Stop();
    }

    public IEnumerator EnableLight()
    {
        candlelight.gameObject.SetActive(true);
        isLit = true;
        float targetIntensity = candlelight.intensity;
        candlelight.intensity *= 2;
        StartCoroutine(FlameDanceLoop());
        sound.PlayOneShot(whoosh);
        sound.clip = burning;
        sound.Play();
        flame.Play();
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
            while(candlelight.intensity < maxValue)
            {
                candlelight.intensity += 0.005f;
                yield return null;
            }
            while(candlelight.intensity > minValue)
            {
                candlelight.intensity -= 0.005f;
                yield return null;
            }
        }
    }

    private IEnumerator FlameDanceLoop()
    {
        Vector3 originalPos = candlelight.gameObject.transform.position;
        while (isLit)
        {
            float speed = Random.Range(0.1f, 2f);
            Vector3 newPos = new Vector3(Random.Range(originalPos.x - 0.5f, originalPos.x + 0.5f), Random.Range(originalPos.y, originalPos.y + 0.5f), Random.Range(originalPos.z - 0.5f, originalPos.z + 0.5f));
            while(Vector3.Distance(candlelight.gameObject.transform.position, newPos) > 0.02f)
            {
                Vector3 direction = (newPos - candlelight.gameObject.transform.position).normalized;
                candlelight.gameObject.transform.Translate(direction * Time.deltaTime * speed, Space.World);
                yield return null;
            }
        }
    }

    public void DisableLight()
    {
        sound.Stop();
        sound.PlayOneShot(blowOut);
        isLit = false;
        candlelight.gameObject.SetActive(false);
        flame.Stop();
    }

    public void KnockOver()
    {
        anim.Play("candleKnockedOverAnimation");
    }
}
