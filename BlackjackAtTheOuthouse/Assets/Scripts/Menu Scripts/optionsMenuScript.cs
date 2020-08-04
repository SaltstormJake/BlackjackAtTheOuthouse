using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class optionsMenuScript : MonoBehaviour
{

    [SerializeField] Button mainMenuButton;

    [SerializeField] mainMenuScript mainMenu;
    GameObject[] menuElements;

    private void Awake()
    {
        menuElements = GameObject.FindGameObjectsWithTag("optionsOnly");
        mainMenuButton.onClick.AddListener(OnClickMainMenu);
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
        foreach (GameObject g in menuElements)
            g.SetActive(enabled);
    }

    void OnClickMainMenu()
    {
        SetScreen(false);
        mainMenu.SetScreen(true);
    }
}
