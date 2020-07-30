using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class resultsScreenScript : MonoBehaviour
{
    [SerializeField] Text resultText;
    [SerializeField] Text resultTextNumber;
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

    public void ShowResults(blackjackUIScript.Result r, int amount)
    {
        resultText.gameObject.SetActive(true);
        switch (r)
        {
            case (blackjackUIScript.Result.PlayerWins):
                resultText.text = "Win: +$";
                break;
            case (blackjackUIScript.Result.DealerWins):
                resultText.text = "Lose: -$";
                break;
            case (blackjackUIScript.Result.PlayerBlackjack):
                resultText.text = "Blackjack: +$";
                break;
            case (blackjackUIScript.Result.DealerBlackjack):
                resultText.text = "Dealer Blackjack: -$";
                break;
            case (blackjackUIScript.Result.PlayerBust):
                resultText.text = "Bust: -$";
                break;
            case (blackjackUIScript.Result.DealerBust):
                resultText.text = "Dealer Bust: +$";
                break;
            case (blackjackUIScript.Result.Player5Cards):
                resultText.text = "Player has 5 cards: +$";
                break;
            case (blackjackUIScript.Result.Dealer5Cards):
                resultText.text = "Dealer has 5 cards: -$";
                break;
            case (blackjackUIScript.Result.Push):
                resultText.text = "Push. +$";
                break;
        }

        resultTextNumber.text = Mathf.Abs(amount).ToString();
        dealAgainButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);

        betSlider.gameObject.SetActive(true);
        betSlider.maxValue = (UI.GetFunds() + amount) / 50;
        betText.SetActive(true);
    }

    void DisableAll()
    {
        resultText.gameObject.SetActive(false);
        winText.SetActive(false);
        loseText.SetActive(false);
        pushText.SetActive(false);
        dealAgainButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        betSlider.gameObject.SetActive(false);
        betText.SetActive(false);
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
