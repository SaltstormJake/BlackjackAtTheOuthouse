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
    [SerializeField] optionsMenuScript options;

    [SerializeField] List<List<AudioClip>> banterLines;

    private Animation anim;
    private AudioSource voice;
    private Vector3 playerHandPosition = new Vector3(180, 2, 25);
    private Vector3 dealerHandPosition = new Vector3(205, 2, 40);
    private float cardSpacing = 5;

    private List<GameObject> hand;
    private int handValue;

    bool pointing = false;

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
        anim.Play("godBossIntroAnimation");
        deck.Shuffle();
        while (anim.isPlaying || voice.isPlaying)
            yield return new WaitForSeconds(0.01f);
        //while (voice.isPlaying)
        //    yield return new WaitForSeconds(0.01f);
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
        TogglePointToDeck();
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DealCardToPlayer());
        yield return StartCoroutine(DealCardToPlayer());
        if (player.GetHandValue() > 21)
            player.CheckAces();
        yield return StartCoroutine(DealCardToSelf());
        yield return StartCoroutine(DealCardToSelfFaceDown());
        yield return new WaitForSeconds(0.5f);
        anim.CrossFade("godBossDeckIdleAnimation");
        if(hand[0].GetComponent<cardScript>().GetValue() == 11 && UI.GetFunds() >= (int)(player.GetBetAmount() * 0.5)) //gives the player the option to purchase insurance
            UI.SetInsurance(true);
        else if (player.GetHandValue() == 21 || GetHandValue() == 21)
            StartCoroutine(Blackjack());
        else
        {
            UI.SetHitAndStand(true);
            if (UI.GetFunds() >= player.GetBetAmount())
                UI.SetDoubleDown(true);
        }
    }

    public IEnumerator Hit()
    {
        yield return StartCoroutine(DealCardToPlayer());
        if (player.GetHandValue() > 21)
        {
            if (player.CheckAces())
            {
                UI.SetHitAndStand(true);
                anim.CrossFade("godBossDeckIdleAnimation");
            }
            else
                StartCoroutine(React(blackjackUIScript.Result.PlayerBust));
        }
        else if ((player.GetHandSize() == 5 && !options.GetFiveCardCharlieToggleDisabled()) || player.GetHandValue() == 21)
            StartCoroutine(Stand());
        else
        {
            UI.SetHitAndStand(true);
            anim.CrossFade("godBossDeckIdleAnimation");
        }
    }

    public IEnumerator Stand()
    {
        cardScript script = hand[1].GetComponent<cardScript>();
        StartCoroutine(script.RaiseAndLowerCard());
        yield return StartCoroutine(script.Flip());
        if (GetHandValue() > 21)
            CheckAces();
        if (options.GetShowOnUIToggle())
            UI.SetDealerHandValueText(GetHandValue());
        while(GetHandValue() < 17 && (GetHandSize() < 5 || options.GetFiveCardCharlieToggleDisabled()))
        {
            yield return StartCoroutine(DealCardToSelf());
            yield return new WaitForSeconds(0.25f);
            if (GetHandValue() > 21)
                CheckAces();
        }
        if(GetHandValue() == 17 && hand.FirstOrDefault(i => i.GetComponent<cardScript>().GetValue() == 11) != null)
        {
            do
            {
                yield return StartCoroutine(DealCardToSelf());
                if (GetHandValue() > 21)
                    CheckAces();
                yield return new WaitForSeconds(0.25f);
            }
            while (GetHandValue() < 17 && (GetHandSize() < 5 || options.GetFiveCardCharlieToggleDisabled()));
        }
        if (GetHandValue() > 21 && !CheckAces())
            StartCoroutine(React(blackjackUIScript.Result.DealerBust));
        else
            StartCoroutine(EvaluateHands());
    }

    public IEnumerator Insurance(bool tookInsurance)
    {
        if (tookInsurance)
        {
            player.SetInsurance(true);
        }
        cardScript script = hand[1].GetComponent<cardScript>();
        if(script.GetValue() == 10)
        {
            UI.SetHitAndStand(false);
            UI.SetDoubleDown(false);
            StartCoroutine(script.RaiseAndLowerCard());
            yield return StartCoroutine(script.Flip());
            if (options.GetShowOnUIToggle())
                UI.SetDealerHandValueText(GetHandValue());
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(EvaluateHands());
        }
        else if(player.GetHandValue() == 21)
        {
            StartCoroutine(Stand());
        }
        else
        {
            UI.SetHitAndStand(true);
            if (UI.GetFunds() > player.GetBetAmount())
                UI.SetDoubleDown(true);
        }
    }

    private IEnumerator Blackjack()
    {
        cardScript script = hand[1].GetComponent<cardScript>();
        StartCoroutine(script.RaiseAndLowerCard());
        yield return StartCoroutine(script.Flip());
        if (options.GetShowOnUIToggle())
            UI.SetDealerHandValueText(GetHandValue());
        yield return new WaitForSeconds(0.5f);
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
        while (voice.isPlaying)
            yield return new WaitForSeconds(0.01f);
        player.ToggleTableLean();
    }

    public IEnumerator DoubleDown()
    {
        player.DoubleBet();
        yield return StartCoroutine(DealCardToPlayer());
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Stand());
    }

    private IEnumerator EvaluateHands()
    {
        yield return new WaitForSeconds(1.5f);
        if (GetHandValue() == 21 && GetHandSize() == 2)
            StartCoroutine(React(blackjackUIScript.Result.DealerBlackjack));
        else if (!options.GetFiveCardCharlieToggleDisabled() && player.GetHandSize() == 5 && GetHandSize() < 5)
            StartCoroutine(React(blackjackUIScript.Result.Player5Cards));
        else if (!options.GetFiveCardCharlieToggleDisabled() && GetHandSize() == 5 && player.GetHandSize() < 5)
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
        TogglePointToDeck();
        switch (r)
        {
            case blackjackUIScript.Result.PlayerWins:
                anim.CrossFade("godBossSighAnimation");
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
                anim.CrossFade("godBossSpinHeadAnimation");
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
        float banterChance = Random.Range(0f, 1f);
        Debug.Log(banterChance);
        if (UI.GetFunds() <= 0)
            yield return StartCoroutine(OutOfMoney());
        else if (banterChance > 0.9f)
            yield return StartCoroutine(SayBanter());
        results.SetButtons(true, player.GetBetAmount());
        Idle();
    }

    private IEnumerator SayBanter()
    {
        //results.DisableText();
        //voice.clip = voiceLines[3];
        //voice.Play();
        //while (voice.isPlaying)
        //    yield return new WaitForSeconds(0.01f);
        yield return null;
    }

    public IEnumerator DealCardToPlayer()
    {
        GameObject card = deck.DealCard();
        cardScript script = card.GetComponent<cardScript>();
        anim.CrossFade("godBossDealToPlayerAnimation");
        Vector3 cardPos = playerHandPosition;
        cardPos.z -= player.GetHandSize() * cardSpacing;
        cardPos.y += player.GetHandSize() * 0.1f;
        StartCoroutine(script.Flip());
        yield return StartCoroutine(script.MoveCard(cardPos));
        player.addToHand(card);
        if(options.GetShowOnUIToggle())
            UI.SetPlayerHandValueText(player.GetHandValue());
    }

    private IEnumerator DealCardToSelf()
    {
        GameObject card = deck.DealCard();
        cardScript script = card.GetComponent<cardScript>();
        anim.CrossFade("godBossDealToSelfAnimation");
        Vector3 cardPos = dealerHandPosition;
        cardPos.z -= hand.Count * cardSpacing;
        cardPos.y += hand.Count * 0.1f;
        StartCoroutine(script.Flip());
        yield return StartCoroutine(script.MoveCard(cardPos));
        AddCardToHand(card);
        if(options.GetShowOnUIToggle())
            UI.SetDealerHandValueText(GetHandValue());
    }

    private IEnumerator DealCardToSelfFaceDown()
    {
        GameObject card = deck.DealCard();
        cardScript script = card.GetComponent<cardScript>();
        anim.Play("godBossDealToSelfAnimation");
        Vector3 cardPos = dealerHandPosition;
        cardPos.z -= hand.Count * cardSpacing;
        cardPos.y += hand.Count * 0.1f;
        yield return StartCoroutine(script.MoveCard(cardPos));
        AddCardToHand(card);
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
            UI.SetDealerHandValueText(GetHandValue());
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
        yield return new WaitForSeconds(1.5f);
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
        StartCoroutine(music.FadeOut(10));
    }

    private void KnockOverCandle()
    {
        candle.KnockOver();
    }

    public void TogglePointToDeck()
    {
        if (!pointing)
        {
            anim["godBossPointToDeckAnimation"].normalizedTime = 0.0f;
            anim["godBossPointToDeckAnimation"].speed = 1.0f;
            anim.Play("godBossPointToDeckAnimation");
        }
        else
        {
            anim["godBossPointToDeckAnimation"].normalizedTime = 1.0f;
            anim["godBossPointToDeckAnimation"].speed = -1.0f;
            anim.Play("godBossPointToDeckAnimation");
        }
        pointing = !pointing;
    }

    private void SayLine(AudioClip sound)
    {
        voice.clip = sound;
        voice.Play();
    }
}

