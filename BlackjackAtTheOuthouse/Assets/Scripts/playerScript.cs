using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class playerScript : MonoBehaviour
{
    [SerializeField] dealerScript dealer;
    [SerializeField] blackjackUIScript UI;
    [SerializeField] resultsScreenScript results;

    private List<GameObject> hand;
    private int handValue;
    private Animation anim;
    private bool leaning = false;

    private int betAmount = 0;

    private void Awake()
    {
        hand = new List<GameObject>();
        anim = gameObject.GetComponent<Animation>();
    }

    public IEnumerator EndHand(blackjackUIScript.Result r)
    {
        ToggleTableLean();
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        switch (r)
        {
            case blackjackUIScript.Result.PlayerWins:
                betAmount = betAmount;
                break;
            case blackjackUIScript.Result.DealerWins:
                betAmount = -betAmount;
                break;
            case blackjackUIScript.Result.PlayerBlackjack:
                betAmount = betAmount * 2;
                break;
            case blackjackUIScript.Result.DealerBlackjack:
                betAmount = -betAmount;
                break;
            case blackjackUIScript.Result.PlayerBust:
                betAmount = -betAmount;
                break;
            case blackjackUIScript.Result.DealerBust:
                betAmount = betAmount;
                break;
            case blackjackUIScript.Result.Player5Cards:
                betAmount = betAmount * 2;
                break;
            case blackjackUIScript.Result.Dealer5Cards:
                betAmount = -betAmount;
                break;
            case blackjackUIScript.Result.Push:
                betAmount = 0;
                break;
        }
        results.ShowResults(r, betAmount);
        UI.ChangeFunds(betAmount);
    }

 /*   public IEnumerator WinHand()
    {
        ToggleTableLean();
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        results.Win(betAmount);
        UI.ChangeFunds(betAmount);
    }

    public IEnumerator LoseHand()
    {
        ToggleTableLean();
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        results.Lose(betAmount);
        UI.ChangeFunds(-betAmount);
    }

    public IEnumerator Push()
    {
        ToggleTableLean();
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        results.Push();
    }
    */
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
}
