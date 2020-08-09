using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainMenuScript : MonoBehaviour
{
    private GameObject[] menuElements;

    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] optionsMenuScript options;
    [SerializeField] mainMenuCreditsScript credits;

    [SerializeField] private playerScript player;


    void Awake()
    {
        menuElements = GameObject.FindGameObjectsWithTag("menuOnly");
    }

    void Start()
    {
        playButton.onClick.AddListener(OnClickPlay);
        optionsButton.onClick.AddListener(OnClickOptions);
        creditsButton.onClick.AddListener(OnClickCredits);
        quitButton.onClick.AddListener(OnClickQuit);
    }


    void OnClickPlay()
    {
        SetScreen(false);
        player.WalkUpStairs();
    }

    void OnClickOptions()
    {
        SetScreen(false);
        options.SetScreen(true);
    }

    void OnClickCredits()
    {
        SetScreen(false);
        credits.SetScreen(true);
    }

    void OnClickQuit()
    {
        Application.Quit();
    }

    public void SetScreen(bool enabled)
    {
        foreach (GameObject g in menuElements)
            g.SetActive(enabled);
    }
}
