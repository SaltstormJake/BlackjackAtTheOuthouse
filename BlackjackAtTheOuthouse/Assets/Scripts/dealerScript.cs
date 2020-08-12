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

    [SerializeField] List<AnimationClip> banterLines;
    private int banterIterator = 0;
    [SerializeField] List<AnimationClip> fillerBanter;
    private int fillerIterator = 0;

    private Animation anim;
    private AudioSource voice;
    private Vector3 playerHandPosition = new Vector3(180, 2, 25); //the position of the first card in the player's hand
    private Vector3 dealerHandPosition = new Vector3(205, 2, 40); //the position of the first card in the dealer's hand
    private float cardSpacing = 5; //the distance between each card in one's hand

    private List<GameObject> hand;
    private int handValue;

    bool pointing = false; //essentially a poor man's state machine

    private void Awake()
    {
        anim = gameObject.GetComponent<Animation>();
        voice = mouth.GetComponent<AudioSource>();

        hand = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(Introduction());
    }

    //The intro cutscene, after which the UI is enabled.
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
        Idle();
        UI.SetDeal(true);
    }

    //Deals 2 cards to the player and dealer, with the second dealer card face down.
    //Checks if anyone has a Natural Blackjack and auto-flips if so.
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
        yield return StartCoroutine(DealCardToSelf());
        yield return StartCoroutine(DealCardToPlayer());
        if (player.GetHandValue() > 21)
            player.CheckAces();
        //yield return StartCoroutine(DealCardToSelf());
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

    //Deals the player another card.
    //Checks if the player busted when they were given the new card.
    //Checks if the player hits 21 or 5 cards and ends their turn if so.
    public IEnumerator Hit()
    {
        yield return StartCoroutine(DealCardToPlayer());
        if ((player.GetHandSize() == 5 && !options.GetFiveCardCharlieToggleDisabled()) || player.GetHandValue() == 21)
        {
            if(player.GetHandValue() > 21 && !player.CheckAces())
            {
                StartCoroutine(React(blackjackUIScript.Result.PlayerBust));
            }
            else
                StartCoroutine(Stand());
        }
        else if (player.GetHandValue() > 21)
        {
            if (player.CheckAces())
            {
                UI.SetHitAndStand(true);
                anim.CrossFade("godBossDeckIdleAnimation");
            }
            else
                StartCoroutine(React(blackjackUIScript.Result.PlayerBust));
        }
        else
        {
            UI.SetHitAndStand(true);
            anim.CrossFade("godBossDeckIdleAnimation");
        }
    }

    //Dealer behaves according to standard Blackjack rules.
    //The second card is flipped over. The dealer draws until his hand is above soft 17.
    public IEnumerator Stand()
    {
        cardScript script = hand[1].GetComponent<cardScript>();
        yield return StartCoroutine(script.LiftAndFlip(5));
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

    //After insurance is taken or not, the dealer flips his second card if it's a 10.
    //If not, the hand continues as normal, the player's money wasted if they
    //took insurance that turn.
    public void Insurance(bool tookInsurance)
    {
        player.SetInsurance(tookInsurance);
        cardScript script = hand[1].GetComponent<cardScript>();
        if(script.GetValue() == 10 || player.GetHandValue() == 21)
        {
            UI.SetHitAndStand(false);
            UI.SetDoubleDown(false);
            StartCoroutine(Blackjack());
        }
        else
        {
            UI.SetHitAndStand(true);
            if (UI.GetFunds() > player.GetBetAmount())
                UI.SetDoubleDown(true);
        }
    }

    //Called when either the dealer or the player has a Natural Blackjack.
    //Flips the dealer's second card, then simply compares the hands to determine
    //who has the Blackjack.
    private IEnumerator Blackjack()
    {
        cardScript script = hand[1].GetComponent<cardScript>();
        yield return StartCoroutine(script.LiftAndFlip(5));
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

    //Plays the animation for reshuffling the deck
    //as it calls for the deck to actually shuffle
    //and refill itself.
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

    //Doubles the player's bet, deals one more card,
    //and then stands.
    public IEnumerator DoubleDown()
    {
        player.DoubleBet();
        yield return StartCoroutine(DealCardToPlayer());
        yield return new WaitForSeconds(0.5f);
        if (player.GetHandValue() > 21 && !player.CheckAces())
            StartCoroutine(React(blackjackUIScript.Result.PlayerBust));
        else
            StartCoroutine(Stand());
    }

    //Compares the values of both hands and awards a win accordingly.
    private IEnumerator EvaluateHands()
    {
        yield return new WaitForSeconds(1.5f);
        if (!options.GetFiveCardCharlieToggleDisabled() && player.GetHandSize() == 5 && GetHandSize() < 5)
            StartCoroutine(React(blackjackUIScript.Result.Player5Cards));
        else if (!options.GetFiveCardCharlieToggleDisabled() && GetHandSize() == 5 && player.GetHandSize() < 5)
            StartCoroutine(React(blackjackUIScript.Result.Dealer5Cards));
        else if (GetHandValue() > player.GetHandValue())
            StartCoroutine(React(blackjackUIScript.Result.DealerWins));
        else if (player.GetHandValue() > GetHandValue())
            StartCoroutine(React(blackjackUIScript.Result.PlayerWins));
        else
            StartCoroutine(React(blackjackUIScript.Result.Push));
    }

    //Sets the proper animation to use after a turn ends, and then decides whether to add banter on afterwards.
    //Gives the player more money if they run out.
    private IEnumerator React(blackjackUIScript.Result r)
    {
        TogglePointToDeck();
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        switch (r)
        {
            case blackjackUIScript.Result.PlayerWins:
                anim.CrossFade("godBossSighAnimation");
                break;
            case blackjackUIScript.Result.DealerWins:
                anim.CrossFade("godBossWinGestureAnimation");
                break;
            case blackjackUIScript.Result.PlayerBlackjack:
                anim.CrossFade("godBossHeadInHandsAnimation");
                break;
            case blackjackUIScript.Result.DealerBlackjack:
                if (player.GetInsurance())
                    anim.CrossFade("godBossShrugAnimation");
                else
                    anim.CrossFade("godBossLaughAnimation");
                break;
            case blackjackUIScript.Result.BothHaveBlackjack:
                if (player.GetInsurance())
                    anim.CrossFade("godBossSighAnimation");
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
                anim.CrossFade("godBossCollapseAnimation");
                break;
            case blackjackUIScript.Result.Dealer5Cards:
                anim.CrossFade("godBossSpinHeadAnimation");
                break;
            case blackjackUIScript.Result.Push:
                anim.CrossFade("godBossScratchChinAnimation");
                break;
        }
        player.ToggleTableLean();
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        StartCoroutine(player.EndHand(r));
        yield return new WaitForSeconds(1.0f);
        float banterChance = Random.Range(0f, 1f);
        while (voice.isPlaying)
            yield return new WaitForSeconds(0.01f);
        if (UI.GetFunds() <= 0)
            yield return StartCoroutine(OutOfMoney());
        else if (banterChance > 0.5f)
            yield return StartCoroutine(SayBanter());
        results.SetButtons(true, player.GetBetAmount());
        Idle();
    }

    //Gives banter dialogue when this coroutine is called.
    private IEnumerator SayBanter()
    {
        float substantialOrFiller = Random.Range(0.0f, 1.0f);
        if(substantialOrFiller > 0.5f && banterIterator < banterLines.Count)
        {
            results.DisableText();
            anim.Play(banterLines[banterIterator++].name);
            while (anim.isPlaying) 
                yield return new WaitForSeconds(0.01f);
        }
        else
        {
            if (fillerIterator > fillerBanter.Count - 1)
                fillerIterator = 0;
            anim.Play(fillerBanter[fillerIterator++].name);
            while (anim.isPlaying)
                yield return new WaitForSeconds(0.01f);
        }
    }

    //Pulls a card from the deck and moves it to the player's hand
    //both physically and in the backend.
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

    //Pulls a card from the deck and moves it to the dealer's hand
    //both physically and in the backend.
    private IEnumerator DealCardToSelf()
    {
        GameObject card = deck.DealCard();
        cardScript script = card.GetComponent<cardScript>();
        anim.CrossFade("godBossDealToSelfAnimation");
        StartCoroutine(script.Flip());
        Vector3 cardPos = dealerHandPosition;
        cardPos.z -= hand.Count * cardSpacing;
        cardPos.y += hand.Count * 0.1f;// + 4.0f;
        yield return StartCoroutine(script.MoveCard(cardPos));
        AddCardToHand(card);
        if(options.GetShowOnUIToggle())
            UI.SetDealerHandValueText(GetHandValue());
    }

    //Pulls a card from the deck and moves it to the dealer's hand
    //both physically and in the backend, without flipping it over.
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
        StartCoroutine(candle.EnableLight());
    }

    private void UnlightCandle()
    {
        candle.DisableLight();
    }

    private void Idle()
    {
        anim.CrossFade("godBossIdleAnimation");
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

    //If the player has any aces that are still worth 11, the first one in the
    //hand is converted to a 1 and it returns true. This is used for when a soft hand
    //becomes a hard hand due to an ace's presence.
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

    public bool HasAces()
    {
        return (hand.FirstOrDefault(i => i.GetComponent<cardScript>().GetValue() == 11) != null);
    }

    public void PrintHand()
    {
        foreach (GameObject g in hand)
        {
            cardScript script = g.GetComponent<cardScript>();
            Debug.Log(script.GetFace() + " of " + script.GetSuit() + "(" + script.GetValue() + ")");
        }
    }

    //Called when the player runs out of money.
    //Plays the cutscene that gives them back money.
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
        results.SetSliderMax(200);
        music.PlayMusic();
    }

    //Only used for the animation that plays when the player
    //is out of money.
    private void ChangeUIFunds(int amount)
    {
        UI.ChangeFunds(amount);
    }

    //Called when the player exits the game while at the table.
    //Plays the dealer's animation and fades out the music.
    public IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(1.0f);
        voice.clip = voiceLines[6];
        voice.Play();
        yield return new WaitForSeconds(6.0f);
        anim.Play("godBossTripAnimation");
        while (voice.isPlaying || anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        anim.Play("godBossFarewellAnimation");
        StartCoroutine(music.FadeOut(10));
    }

    private void KnockOverCandle()
    {
        candle.KnockOver();
    }

    //A mini-state machine of sorts where the animation is reversed
    //if the dealer is already pointing at the deck.
    public void TogglePointToDeck()
    {
        if (!pointing)
        {
            anim["godBossPointToDeckAnimation"].normalizedTime = 0.0f;
            anim["godBossPointToDeckAnimation"].speed = 1.0f;
            anim.CrossFade("godBossPointToDeckAnimation");
        }
        else
        {
            anim["godBossPointToDeckAnimation"].normalizedTime = 1.0f;
            anim["godBossPointToDeckAnimation"].speed = -1.0f;
            anim.CrossFade("godBossPointToDeckAnimation");
        }
        pointing = !pointing;
    }

    //This is used to allow the dealer to speak during animations
    //using Unity's animation tool alone.
    private void SayLine(AudioClip sound)
    {
        voice.clip = sound;
        voice.Play();
    }
}

