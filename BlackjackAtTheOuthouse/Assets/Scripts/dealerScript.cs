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
    private Animation anim;
    private AudioSource voice;
    private Vector3 playerHandPosition = new Vector3(180, 2, 25);
    private Vector3 dealerHandPosition = new Vector3(205, 2, 35);
    private float cardSpacing = 5;

    private List<GameObject> hand;
    private int handValue;

    private bool firstDeal;

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
        anim.Play("godBossIdleAnimation");
        UI.SetDeal(true);
    }

    public IEnumerator Deal()
    {
        anim.Stop();
        Debug.Log(deck.GetCardsRemaining());
        if (deck.GetCardsRemaining() < 10)
            yield return StartCoroutine(ReshuffleDeck());
        //anim.wrapMode = WrapMode.Once;
        while (anim.isPlaying || voice.isPlaying)
            yield return new WaitForSeconds(0.01f);
        anim.Play("godBossPointToDeckAnimation");
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        DealCardToPlayer();
        yield return new WaitForSeconds(1.0f);
        DealCardToPlayer();
        yield return new WaitForSeconds(1.0f);
        DealCardToSelf();
        yield return new WaitForSeconds(1.0f);
        DealCardToSelfFaceDown();
        yield return new WaitForSeconds(1.5f);
        if (player.GetHandValue() == 21)
            StartCoroutine(Stand());
        else
        {
            UI.SetHitAndStand(true);
            UI.SetDoubleDown(true);
        }
    }
    
    public IEnumerator Hit()
    {
        Debug.Log(player.GetHandSize());
        DealCardToPlayer();
        yield return new WaitForSeconds(1.5f);
        if (player.GetHandValue() > 21)
        {
            if (player.CheckAces())
            {
                UI.SetHitAndStand(true);
            }
            else
                StartCoroutine(DealerWins());
        }
        else if (player.GetHandSize() == 5 || player.GetHandValue() == 21)
            StartCoroutine(Stand());
        else
            UI.SetHitAndStand(true);
    }

    public IEnumerator Stand()
    {
        cardScript script = hand[1].GetComponent<cardScript>();
        StartCoroutine(script.Flip());
        StartCoroutine(script.RaiseAndLowerCard());
        yield return new WaitForSeconds(1.5f);
        while(GetHandValue() < 17 && GetHandSize() < 5)
        {
            DealCardToSelf();
            yield return new WaitForSeconds(1.0f);
            if (GetHandValue() > 21)
                CheckAces();
        }
        if (GetHandValue() > 21)
            StartCoroutine(PlayerWins());
        else
            StartCoroutine(EvaluateHands());
    }

    private IEnumerator ReshuffleDeck()
    {
        yield return new WaitForSeconds(1.0f);
        player.ToggleTableLean();
        voice.clip = voiceLines[3];
        voice.Play();
        while (voice.isPlaying)
            yield return new WaitForSeconds(0.01f);
        player.ToggleTableLean();
    }

    public void DoubleDown()
    {
        player.DoubleBet();
        StartCoroutine(Hit());
    }

    private IEnumerator EvaluateHands()
    {
        yield return new WaitForSeconds(1.5f);
        if (player.GetHandSize() == 5 && GetHandSize() < 5)
            StartCoroutine(PlayerWins());
        else if (GetHandSize() == 5 && player.GetHandSize() < 5)
            StartCoroutine(DealerWins());
        else if (GetHandValue() > player.GetHandValue())
        {
            StartCoroutine(DealerWins());
        }
        else if (player.GetHandValue() > GetHandValue())
        {
            StartCoroutine(PlayerWins());
        }
        else
            StartCoroutine(Push());
    }

    private IEnumerator PlayerWins()
    {
        StartCoroutine(player.WinHand());
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        Idle();
    }

    private IEnumerator DealerWins()
    {
        StartCoroutine(player.LoseHand());
        anim.Play("godBossSnapAnimation");
        while (anim.isPlaying)
            yield return new WaitForSeconds(0.01f);
        Idle();
    }

    private IEnumerator Push()
    {
        StartCoroutine(player.Push());
        Idle();
        yield return null;
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
        anim.Play("godBossIdleAnimation");
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
}

