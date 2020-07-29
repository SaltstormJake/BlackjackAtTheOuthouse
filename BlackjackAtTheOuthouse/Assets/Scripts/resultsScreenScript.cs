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

    [SerializeField] Slider betSlider;
    [SerializeField] GameObject betText;
    [SerializeField] Text betTextNumber;

    [SerializeField] blackjackUIScript UI;

   // public enum Result { Win, Lose, Push };

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
        if (betTextNumber.IsActive())
        {
            betTextNumber.text = (betSlider.value * 50).ToString();
        }
    }

    public void Win(int amount)
    {
        winText.SetActive(true);
        winNumber.gameObject.SetActive(true);
        winNumber.text = amount.ToString();
        dealAgainButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);

        betSlider.gameObject.SetActive(true);
        betSlider.maxValue = (UI.GetFunds() + amount) / 50;
        betText.SetActive(true);
        betTextNumber.gameObject.SetActive(true);
    }

    public void Lose(int amount)
    {
        loseText.SetActive(true);
        loseNumber.gameObject.SetActive(true);
        loseNumber.text = amount.ToString();
        dealAgainButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);

        betSlider.gameObject.SetActive(true);
        betSlider.maxValue = (UI.GetFunds() - amount) / 50;
        betText.SetActive(true);
        betTextNumber.gameObject.SetActive(true);
    }

    public void Push()
    {
        pushText.SetActive(true);
        dealAgainButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);

        betSlider.gameObject.SetActive(true);
        betSlider.maxValue = UI.GetFunds() / 50;
        betText.SetActive(true);
        betTextNumber.gameObject.SetActive(true);
    }

    void DisableAll()
    {
        winText.SetActive(false);
        loseText.SetActive(false);
        pushText.SetActive(false);
        dealAgainButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        betSlider.gameObject.SetActive(false);
        betText.SetActive(false);
        betTextNumber.gameObject.SetActive(false);
    }

    void DealButtonOnClick()
    {
        UI.OnDealAgainClick((int)betSlider.value * 50);
        DisableAll();
    }

    void QuitButtonOnClick()
    {
        Application.Quit();
    }
}
