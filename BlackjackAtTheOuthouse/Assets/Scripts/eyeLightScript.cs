using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyeLightScript : MonoBehaviour
{
    private Light eye;
    [SerializeField] AudioSource voice;

    float updateStep = 0.1f;
    int sampleDataLength = 1024;

    float currentUpdateTime = 0f;

    float clipLoudness;
    float[] clipSampleData;

    private void Awake()
    {
        eye = gameObject.GetComponent<Light>();

        clipSampleData = new float[sampleDataLength];
    }
    // Start is called before the first frame update
    void Start()
    {
        eye.intensity = 0;
    }

    // Update is called once per frame
    //Sets the illumination of the eyes when the dealer is talking.
    void Update()
    {
        if (voice.isPlaying)
        {
            currentUpdateTime += Time.deltaTime;
            if (currentUpdateTime >= updateStep)
            {
                currentUpdateTime = 0f;
                voice.clip.GetData(clipSampleData, voice.timeSamples);
                clipLoudness = 0f;
                foreach (var sample in clipSampleData)
                    clipLoudness += Mathf.Abs(sample);
                clipLoudness /= sampleDataLength;
            }
        }
        else
            clipLoudness = 0;
        eye.intensity = clipLoudness * 30;
    }
}
