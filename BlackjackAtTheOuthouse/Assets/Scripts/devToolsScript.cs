using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class devToolsScript : MonoBehaviour
{
    [SerializeField] KeyCode speedUpKey;
    [SerializeField] float speedUpTime;
    [SerializeField] AudioSource voiceSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(speedUpKey))
            SpeedUp();
    }

    void SpeedUp()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = speedUpTime;
            voiceSource.pitch = speedUpTime;
        }
        else
        { 
            Time.timeScale = 1;
            voiceSource.pitch = 1;
        }
    }
}
