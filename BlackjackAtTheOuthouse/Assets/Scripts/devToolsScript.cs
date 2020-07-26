using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class devToolsScript : MonoBehaviour
{
    [SerializeField] KeyCode speedUpKey;
    [SerializeField] float speedUpTime;
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
            Time.timeScale = speedUpTime;
        else
            Time.timeScale = 1;
    }
}
