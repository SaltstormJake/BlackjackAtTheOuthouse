using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class resultsScreenScript : MonoBehaviour
{
    [SerializeField] GameObject winText;
    [SerializeField] GameObject loseText;
    [SerializeField] GameObject pushText;
    [SerializeField] Text winNumber;
    [SerializeField] Text loseNumber;
    [SerializeField] Button dealAgainButton;
    [SerializeField] Button quitButton;

    [SerializeField] blackjackUIScript UI;

    private void Awake()
    {
        dealAgainButton.onClick.AddListener(DealButtonOnClick);
        quitButton.onClick.AddListener(QuitButtonOnClick);
    }
    // Start is called before the first frame update
    void Start()
    {
        DisableAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Win(int amount)
    {
        winText.SetActive(true);
        winNumber.gameObject.SetActive(true);
        winNumber.text = amount.ToString();
        dealAgainButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    public void Lose(int amount)
    {
        loseText.SetActive(true);
        loseNumber.gameObject.SetActive(true);
        loseNumber.text = amount.ToString();
        dealAgainButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    public void Push()
    {
        pushText.SetActive(true);
        dealAgainButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    void DisableAll()
    {
        winText.SetActive(false);
        loseText.SetActive(false);
        pushText.SetActive(false);
        dealAgainButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    void DealButtonOnClick()
    {
        DisableAll();
        UI.OnDealClick();
    }

    void QuitButtonOnClick()
    {
        Application.Quit();
    }
}
