using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class playerScript : MonoBehaviour
{
    [SerializeField] dealerScript dealer;
    [SerializeField] blackjackUIScript UI;
    [SerializeField] resultsScreenScript results;
    [SerializeField] endingScreenScript endingScreen;

    private List<GameObject> hand;
    private int handValue;
    private Animation anim;
    private bool leaning = false;

    private int betAmount = 0;

    [HideInInspector] public bool insurance = false;

    private void Awake()
    {
        hand = new List<GameObject>();
        anim = gameObject.GetComponent<Animation>();
    }

    public IEnumerator EndHand(blackjackUIScript.Result r)
    {

        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        int winnings = 0;
        switch (r)
        {
            case blackjackUIScript.Result.PlayerWins:
                winnings = (int)(betAmount * 1.5);
                break;
            case blackjackUIScript.Result.DealerWins:
                //no winnings
                break;
            case blackjackUIScript.Result.PlayerBlackjack:
                winnings = betAmount * 3;
                break;
            case blackjackUIScript.Result.DealerBlackjack:
                if (insurance)
                    winnings = betAmount;
                //no winnings otherwise
                break;
            case blackjackUIScript.Result.BothHaveBlackjack:
                if (insurance)
                    winnings = betAmount * 4;
                else
                    winnings = betAmount;
                break;
            case blackjackUIScript.Result.PlayerBust:
                //no winnings
                break;
            case blackjackUIScript.Result.DealerBust:
                winnings = (int)(betAmount * 1.5);
                break;
            case blackjackUIScript.Result.Player5Cards:
                winnings = betAmount * 3;
                break;
            case blackjackUIScript.Result.Dealer5Cards:
                //no winnings
                break;
            case blackjackUIScript.Result.Push:
                winnings = betAmount;
                break;
        }
        results.ShowResults(r, winnings);
        UI.ChangeFunds(winnings);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addToHand(GameObject card)
    {
        if (card.tag == "Card")
        {
            hand.Add(card);
            handValue += card.GetComponent<cardScript>().GetValue();
        }
        else
            Debug.Log("Not a card!");
    }

    public int GetHandValue()
    {
        int handValue = 0;
        foreach(GameObject g in hand)
            handValue += g.GetComponent<cardScript>().GetValue();
        return handValue;
    }

    public int GetHandSize()
    {
        return hand.Count;
    }

    public void ToggleTableLean()
    {
        if (!leaning)
        {
            anim["playerLookAtTableAnimation"].normalizedTime = 0.0f;
            anim["playerLookAtTableAnimation"].speed = 1.0f;
            anim.Play("playerLookAtTableAnimation");
        }
        else
        {
            anim["playerLookAtTableAnimation"].normalizedTime = 1.0f;
            anim["playerLookAtTableAnimation"].speed = -1.0f;
            anim.Play("playerLookAtTableAnimation");
        }
        leaning = !leaning;
    }

    public bool CheckAces()
    {
        GameObject ace = hand.FirstOrDefault(i => i.GetComponent<cardScript>().GetValue() == 11);
        if(ace != null){
            ace.GetComponent<cardScript>().changeValue(1);
            return true;
        }
        return false;
    }

    public void WalkUpStairs()
    {
        anim.Play("playerWalkUpStairsAnimation");
    }

    public void ClearHand()
    {
        foreach (GameObject g in hand)
            Destroy(g);
        hand.Clear();
        handValue = 0;
    }

    public void PrintHand()
    {
        foreach (GameObject g in hand)
        {
            cardScript script = g.GetComponent<cardScript>();
            Debug.Log(script.GetFace() + " of " + script.GetSuit() + "(" + script.GetValue() + ")");
        }
    }

    public void DoubleBet()
    {
        betAmount *= 2;
    }

    public int GetBetAmount()
    {
        return betAmount;
    }

    public void SetBetAmount(int x)
    {
        betAmount = x;
    }

    public bool GetInsurance()
    {
        return insurance;
    }

    public void SetInsurance(bool set)
    {
        insurance = set;
    }

    public IEnumerator QuitGame()
    {
        anim.Play("playerLeaveShackAnimation");
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        endingScreen.SetScreen(true);
    }
}
