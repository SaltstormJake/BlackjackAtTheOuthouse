using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class resultsScreenScript : MonoBehaviour
{
    [SerializeField] Text resultText;
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
                resultText.text = "You win the hand, " + UI.GetPlayerHandValue() + " to the dealer's " + UI.GetDealerHandValue() + ". Winnings: $" + amount.ToString() + ".";
                break;
            case (blackjackUIScript.Result.DealerWins):
                resultText.text = "The dealer wins the hand, " + UI.GetDealerHandValue() + " to your " + UI.GetPlayerHandValue() + ".";
                break;
            case (blackjackUIScript.Result.PlayerBlackjack):
                resultText.text = "You win with a Natural Blackjack. Winnings: $" + amount.ToString() + ".";
                break;
            case (blackjackUIScript.Result.DealerBlackjack):
                if (UI.GetPlayerInsurance())
                    resultText.text = "The dealer has a blackjack. Since you took insurance, you break even this hand. Winnings: $" + amount.ToString() + ".";
                else
                    resultText.text = "The dealer wins the hand with a Blackjack.";
                break;
            case (blackjackUIScript.Result.PlayerBust):
                resultText.text = "You bust with a " + UI.GetPlayerHandValue() + ".";
                break;
            case (blackjackUIScript.Result.DealerBust):
                resultText.text = "The dealer busts with a " + UI.GetDealerHandValue() + ".";
                break;
            case (blackjackUIScript.Result.Player5Cards):
                resultText.text = "You win with a 5 Card Charlie. Winnings: $" + amount.ToString() + ".";
                break;
            case (blackjackUIScript.Result.Dealer5Cards):
                resultText.text = "The dealer wins with a 5 Card Charlie. Winnings: $" + amount.ToString() + ".";
                break;
            case (blackjackUIScript.Result.Push):
                resultText.text = "The hand is a tie. No winnings are awarded.";
                break;
            case (blackjackUIScript.Result.BothHaveBlackjack):
                if (UI.GetPlayerInsurance())
                    resultText.text = "The hand is a tie. No warnings would be awarded, but you took insurance. Winnings: $" + amount.ToString() + ".";
                else
                    resultText.text = "The hand is a tie. No winnings are awarded.";
                break;
        }
        dealAgainButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);

        betSlider.gameObject.SetActive(true);
        betSlider.maxValue = (UI.GetFunds() + amount) / 50;
        betText.SetActive(true);
    }

    void DisableAll()
    {
        resultText.gameObject.SetActive(false);
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
