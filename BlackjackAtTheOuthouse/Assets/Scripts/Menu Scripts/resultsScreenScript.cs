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
            betTextNumber.text = betSlider.value.ToString();
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
                    resultText.text = "The dealer has a blackjack. Since you took insurance, you break even this hand.";
                else
                    resultText.text = "The dealer wins the hand with a blackjack.";
                break;
            case (blackjackUIScript.Result.PlayerBust):
                resultText.text = "You bust with a " + UI.GetPlayerHandValue() + ".";
                break;
            case (blackjackUIScript.Result.DealerBust):
                resultText.text = "The dealer busts with a " + UI.GetDealerHandValue() + ". Winnings: $" + amount.ToString() + ".";
                break;
            case (blackjackUIScript.Result.Player5Cards):
                resultText.text = "You win with a 5 Card Charlie. Winnings: $" + amount.ToString() + ".";
                break;
            case (blackjackUIScript.Result.Dealer5Cards):
                resultText.text = "The dealer wins with a 5 Card Charlie.";
                break;
            case (blackjackUIScript.Result.Push):
                resultText.text = "The hand is a tie. You receive your bet back.";
                break;
            case (blackjackUIScript.Result.BothHaveBlackjack):
                if (UI.GetPlayerInsurance())
                    resultText.text = "The hand is a tie. No winnings would be awarded, but you took insurance. Winnings: $" + amount.ToString() + ".";
                else
                    resultText.text = "The hand is a tie. No winnings are awarded.";
                break;
        }
    }

    public void SetButtons(bool enabled, int amount)
    {
        dealAgainButton.gameObject.SetActive(enabled);
        quitButton.gameObject.SetActive(enabled);

        betSlider.gameObject.SetActive(enabled);
        if (UI.GetFunds() < 100)
        {
            betSlider.maxValue = UI.GetFunds();
        }
        else
        {
            betSlider.maxValue = 100;
        }
        betText.SetActive(enabled);
    }

    public void DisableText()
    {
        resultText.gameObject.SetActive(false);
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
        UI.OnDealAgainClick((int)betSlider.value);
        DisableAll();
    }

    void QuitButtonOnClick()
    {
        UI.QuitGame();
        DisableAll();
    }

    public void SetSliderMax(int max)
    {
        betSlider.maxValue = max;
    }
}
