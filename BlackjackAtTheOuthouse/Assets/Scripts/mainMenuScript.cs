using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainMenuScript : MonoBehaviour
{
    private GameObject[] menuObjects;

    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private playerScript player;
    void Awake()
    {
        menuObjects = GameObject.FindGameObjectsWithTag("menuOnly");
    }
    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(OnClickPlay);
        optionsButton.onClick.AddListener(OnClickOptions);
        creditsButton.onClick.AddListener(OnClickCredits);
        quitButton.onClick.AddListener(OnClickCredits);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickPlay()
    {
        foreach (GameObject g in menuObjects)
            g.SetActive(false);
        player.WalkUpStairs();
    }

    void OnClickOptions()
    {

    }

    void OnClickCredits()
    {

    }

    void OnClickQuit()
    {
        Application.Quit();
    }
}
