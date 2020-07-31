using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class dealerScript : MonoBehaviour
{
    [SerializeField] candleScript candle;
    [SerializeField] GameObject mouth;
    [SerializeField] AudioClip[] voiceLines;
    [SerializeField] musicScript music;
    [SerializeField] deckScript deck;
    [SerializeField] blackjackUIScript UI;
    [SerializeField] playerScript player;
    [SerializeField] resultsScreenScript results;

    private Animation anim;
    private AudioSource voice;
    private Vector3 playerHandPosition = new Vector3(180, 2, 25);
    private Vector3 dealerHandPosition = new Vector3(205, 2, 35);
    private float cardSpacing = 5;

    private List<GameObject> hand;
    private int handValue;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animation>();
        voice = mouth.GetComponent<AudioSource>();

        hand = new List<GameObject>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Introduction());
    }


    private IEnumerator Introduction()
    {
        voice.clip = voiceLines[0];
        voice.Play();
        while (voice.isPlaying)
            yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(1.0f);
        voice.clip = voiceLines[1];
        voice.Play();
        while (voice.isPlaying)
            yield return new WaitForSeconds(0.01f);
        anim.Play("godBossSnapAnimationCandleOn");
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        voice.clip = voiceLines[2];
        voice.Play();
        music.PlayMusic();
        deck.Shuffle();
        while (voice.isPlaying)
            yield return new WaitForSeconds(0.01f);
        Idle();
        UI.SetDeal(true);
    }

    public IEnumerator Deal()
    {
        player.SetInsurance(false);
        anim.CrossFade("godBossResetArmsAnimation");
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        if (deck.GetCardsRemaining() < 10)
            yield return StartCoroutine(ReshuffleDeck());
        while (anim.isPlaying || voice.isPlaying)
            yield return new WaitForSeconds(0.01f);
        anim.Play("godBossPointToDeckAnimation");
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(0.5f);
        DealCardToPlayer();
        yield return new WaitForSeconds(1.0f);
        DealCardToPlayer();
        yield return new WaitForSeconds(1.0f);
        DealCardToSelf();
        yield return new WaitForSeconds(1.0f);
        DealCardToSelfFaceDown();
        yield return new WaitForSeconds(1.5f);
        if(hand[0].GetComponent<cardScript>().GetValue() == 11 && UI.GetFunds() >= (int)(player.GetBetAmount() * 1.5)) //gives the player the option to purchase insurance
        {
            UI.SetInsurance(true);
            UI.SetHitAndStand(true);
            if (UI.GetFunds() >= player.GetBetAmount() * 2)
                UI.SetDoubleDown(true);
        }
        else if (player.GetHandValue() == 21 || GetHandValue() == 21)
            StartCoroutine(Blackjack());
        else
        {
            UI.SetHitAndStand(true);
            if (UI.GetFunds() >= player.GetBetAmount() * 2)
                UI.SetDoubleDown(true);
        }
    }

    public IEnumerator Hit()
    {
        DealCardToPlayer();
        yield return new WaitForSeconds(1.5f);
        if (player.GetHandValue() > 21)
        {
            if (player.CheckAces())
            {
                UI.SetHitAndStand(true);
            }
            else
                StartCoroutine(React(blackjackUIScript.Result.PlayerBust));
        }
        else if (player.GetHandSize() == 5 || player.GetHandValue() == 21)
            StartCoroutine(Stand());
        else
            UI.SetHitAndStand(true);
        //Debug.Log(player.GetHandSize());
    }

    public IEnumerator Stand()
    {
        cardScript script = hand[1].GetComponent<cardScript>();
        StartCoroutine(script.Flip());
        StartCoroutine(script.RaiseAndLowerCard());
        yield return new WaitForSeconds(1.5f);
        if (hand[0].GetComponent<cardScript>().GetValue() == 11 && hand[1].GetComponent<cardScript>().GetValue() == 11)
            CheckAces();
        while(GetHandValue() < 17 && GetHandSize() < 5)
        {
            DealCardToSelf();
            yield return new WaitForSeconds(1.0f);
            if (GetHandValue() > 21)
                CheckAces();
        }
        if(GetHandValue() == 17 && hand.FirstOrDefault(i => i.GetComponent<cardScript>().GetValue() == 11) != null)
        {
            DealCardToSelf();
            yield return new WaitForSeconds(1.0f);
        }
        if (GetHandValue() > 21)
            StartCoroutine(React(blackjackUIScript.Result.DealerBust));
        else
            StartCoroutine(EvaluateHands());
    }

    public IEnumerator Insurance()
    {
        player.SetInsurance(true);
        cardScript script = hand[1].GetComponent<cardScript>();
        if(script.GetValue() == 10)
        {
            UI.SetHitAndStand(false);
            UI.SetDoubleDown(false);
            StartCoroutine(script.Flip());
            StartCoroutine(script.RaiseAndLowerCard());
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EvaluateHands());
        }
        else if(player.GetHandValue() == 21)
        {
            StartCoroutine(Stand());
        }
    }

    private IEnumerator Blackjack()
    {
        cardScript script = hand[1].GetComponent<cardScript>();
        StartCoroutine(script.Flip());
        StartCoroutine(script.RaiseAndLowerCard());
        yield return new WaitForSeconds(1.5f);
        if (player.GetHandValue() > GetHandValue())
            StartCoroutine(React(blackjackUIScript.Result.PlayerBlackjack));
        else if (GetHandValue() > player.GetHandValue())
            StartCoroutine(React(blackjackUIScript.Result.DealerBlackjack));
        else
            StartCoroutine(React(blackjackUIScript.Result.BothHaveBlackjack));
        
    }


    private IEnumerator ReshuffleDeck()
    {
        yield return new WaitForSeconds(1.0f);
        player.ToggleTableLean();
        voice.clip = voiceLines[3];
        voice.Play();
        yield return new WaitForSeconds(2.0f);
        deck.GetComponent<Animation>().Play("deckShuffleAnimation");
        anim.Play("godBossShuffleAnimation");
        deck.ShuffleAndRefill();
        while (voice.isPlaying)
            yield return new WaitForSeconds(0.01f);
        player.ToggleTableLean();
    }

    public IEnumerator DoubleDown()
    {
        player.DoubleBet();
        Debug.Log(player.GetHandSize());
        DealCardToPlayer();
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(Stand());
    }

    private IEnumerator EvaluateHands()
    {
        yield return new WaitForSeconds(1.5f);
        if (GetHandValue() == 21 && GetHandSize() == 2)
            StartCoroutine(React(blackjackUIScript.Result.DealerBlackjack));
        else if (player.GetHandSize() == 5 && GetHandSize() < 5)
            StartCoroutine(React(blackjackUIScript.Result.Player5Cards));
        else if (GetHandSize() == 5 && player.GetHandSize() < 5)
            StartCoroutine(React(blackjackUIScript.Result.Dealer5Cards));
        else if (GetHandValue() > player.GetHandValue())
        {
            StartCoroutine(React(blackjackUIScript.Result.DealerWins));
        }
        else if (player.GetHandValue() > GetHandValue())
        {
            StartCoroutine(React(blackjackUIScript.Result.PlayerWins));
        }
        else
            StartCoroutine(React(blackjackUIScript.Result.Push));
    }

    private IEnumerator React(blackjackUIScript.Result r)
    {
        switch (r)
        {
            case blackjackUIScript.Result.PlayerWins:
                anim.CrossFade("godBossSlapHeadAnimation");
                break;
            case blackjackUIScript.Result.DealerWins:
                anim.CrossFade("godBossSnapAnimation");
                break;
            case blackjackUIScript.Result.PlayerBlackjack:
                anim.CrossFade("godBossSlapHeadAnimation");
                break;
            case blackjackUIScript.Result.DealerBlackjack:
                if (player.GetInsurance())
                    anim.CrossFade("godBossShrugAnimation");
                else
                    anim.CrossFade("godBossSnapAnimation");
                break;
            case blackjackUIScript.Result.BothHaveBlackjack:
                if (player.GetInsurance())
                    anim.CrossFade("godBossSlapHeadAnimation");
                else
                    anim.CrossFade("godBossShrugAnimation");
                break;
            case blackjackUIScript.Result.PlayerBust:
                anim.CrossFade("godBossSnapAnimation");
                break;
            case blackjackUIScript.Result.DealerBust:
                anim.CrossFade("godBossSlapHeadAnimation");
                break;
            case blackjackUIScript.Result.Player5Cards:
                anim.CrossFade("godBossSlapHeadAnimation");
                break;
            case blackjackUIScript.Result.Dealer5Cards:
                anim.CrossFade("godBossSnapAnimation");
                break;
            case blackjackUIScript.Result.Push:
                anim.CrossFade("godBossShrugAnimation");
                break;
        }
        player.ToggleTableLean();
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.1f);
        StartCoroutine(player.EndHand(r));
        yield return new WaitForSeconds(1.0f);
        if (UI.GetFunds() <= 0)
            yield return StartCoroutine(OutOfMoney());
        results.SetButtons(true, player.GetBetAmount());
        Idle();
    }

    public void DealCardToPlayer()
    {
        GameObject card = deck.DealCard();
        cardScript script = card.GetComponent<cardScript>();
        anim.Play("godBossDealToPlayerAnimation");
        Vector3 cardPos = playerHandPosition;
        cardPos.z -= player.GetHandSize() * cardSpacing;
        cardPos.y += player.GetHandSize() * 0.1f;
        StartCoroutine(script.MoveCard(cardPos));
        StartCoroutine(script.Flip());
        player.addToHand(card);
    }

    private void DealCardToSelf()
    {
        GameObject card = deck.DealCard();
        cardScript script = card.GetComponent<cardScript>();
        anim.Play("godBossDealToSelfAnimation");
        Vector3 cardPos = dealerHandPosition;
        cardPos.z -= hand.Count * cardSpacing;
        cardPos.y += hand.Count * 0.1f;
        StartCoroutine(script.MoveCard(cardPos));
        StartCoroutine(script.Flip());
        AddCardToHand(card);
    }

    private void DealCardToSelfFaceDown()
    {
        GameObject card = deck.DealCard();
        cardScript script = card.GetComponent<cardScript>();
        anim.Play("godBossDealToSelfAnimation");
        Vector3 cardPos = dealerHandPosition;
        cardPos.z -= hand.Count * cardSpacing;
        cardPos.y += hand.Count * 0.1f;
        StartCoroutine(script.MoveCard(cardPos));
        AddCardToHand(card);
    }

    private IEnumerator ReadyToDeal()
    {
        anim.Play("godBossPointToDeckAnimation");
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
    }

    private void LightCandle()
    {
        candle.EnableLight();
    }

    private void UnlightCandle()
    {
        candle.DisableLight();
    }

    private void Idle()
    {
        anim.CrossFade("godBossIdleAnimation");
        //anim.wrapMode = WrapMode.Loop;
    }

    private void AddCardToHand(GameObject card)
    {
        hand.Add(card);
        handValue += card.GetComponent<cardScript>().GetValue();
    }

    public int GetHandValue()
    {
        int handValue = 0;
        foreach (GameObject g in hand)
            handValue += g.GetComponent<cardScript>().GetValue();
        return handValue;
    }

    public int GetHandSize()
    {
        return hand.Count;
    }

    public void ClearHand()
    {
        foreach (GameObject g in hand)
            Destroy(g);
        hand.Clear();
        handValue = 0;
    }

    public bool CheckAces()
    {
        GameObject ace = hand.FirstOrDefault(i => i.GetComponent<cardScript>().GetValue() == 11);
        if (ace != null)
        {
            ace.GetComponent<cardScript>().changeValue(1);
            return true;
        }
        return false;
    }

    public void PrintHand()
    {
        foreach (GameObject g in hand)
        {
            cardScript script = g.GetComponent<cardScript>();
            Debug.Log(script.GetFace() + " of " + script.GetSuit() + "(" + script.GetValue() + ")");
        }
    }

    public IEnumerator OutOfMoney()
    {
        yield return new WaitForSeconds(1.0f);
        results.DisableText();
        music.RecordScratch();
        yield return new WaitForSeconds(1.0f);
        anim.Play("godBossScratchHeadAnimation");
        voice.clip = voiceLines[4];
        voice.Play();
        while (voice.isPlaying || anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        voice.clip = voiceLines[5];
        voice.Play();
        anim.Play("godBossPointUpAnimation");
        while (voice.isPlaying || anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        anim.Play("godBossSnapAnimationMoreMoney");
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        //UI.ChangeFunds(500);
        //results.SetButtons(true, player.GetBetAmount());
        results.SetSliderMax(200);
        music.PlayMusic();
    }

    private void ChangeUIFunds(int amount) //used only for the out of money animation
    {
        UI.ChangeFunds(amount);
    }

    public IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(1.0f);
        voice.clip = voiceLines[6];
        voice.Play();
        yield return new WaitForSeconds(6.0f);
        anim.Play("godBossTripAnimation");
        while (voice.isPlaying || anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        AudioSource volume = music.gameObject.GetComponent<AudioSource>();
        float timer = 0;
        while(timer < 10)
        {
            timer += Time.deltaTime;
            volume.volume = Mathf.Lerp(1, 0, timer / 10);
            //volume.volume -= 0.01f;
            // yield return new WaitForSeconds(0.1f);
            yield return null;
        }
    }

    private void KnockOverCandle()
    {
        candle.KnockOver();
    }
}

