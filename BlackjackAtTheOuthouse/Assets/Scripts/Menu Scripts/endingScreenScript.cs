using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class endingScreenScript : MonoBehaviour
{
    [SerializeField] Button QuitButton;

    GameObject[] creditsElements;
    private void Awake()
    {
        QuitButton.onClick.AddListener(OnClickQuit);
        creditsElements = GameObject.FindGameObjectsWithTag("endingCreditsOnly");
    }
    // Start is called before the first frame update
    void Start()
    {
        SetScreen(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScreen(bool enabled)
    {
        foreach (GameObject g in creditsElements)
            g.SetActive(enabled);
    }

    void OnClickQuit()
    {
        Application.Quit();
    }
}
