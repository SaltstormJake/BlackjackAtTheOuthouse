using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class deckScript : MonoBehaviour
{
    [SerializeField] List<GameObject> Deck;

    private int iterator;
    private float deckHeight;
   // private Vector3 originalPosition;
    private void Awake()
    {
        iterator = Deck.Count - 1;
        deckHeight = transform.localScale.y;
        //originalPosition = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject DealCard()
    {
        if (iterator >= 0) //draws cards from the top
        {
            GameObject card = Deck[iterator--];
            Vector3 cardPos = transform.position;
            cardPos.y += 1;
            card = Instantiate(card, cardPos, Quaternion.identity);
            Vector3 cardRotation = card.transform.eulerAngles;
            cardRotation.x += 180;
            cardRotation.y += -90;
            Vector3 cardScale = card.transform.localScale;
            cardScale *= 4;
            Vector3 cardPosition = card.transform.position;
            cardPosition.y += 10;
            card.transform.position = cardPosition;
            card.transform.eulerAngles = cardRotation;
            card.transform.localScale = cardScale;

            //shrink the deck by the width of 1 card
            Vector3 deckScale = transform.localScale;
            deckScale.y -= (deckHeight / 52);
            transform.localScale = deckScale;

            Vector3 deckPosition = transform.position;
            deckPosition.y -= ((4.63f - 1.73f) / 52);
            transform.position = deckPosition;

            return card;
        }
        else
        {
            OutOfCards();
            return null;
        }
    }

    public void Shuffle()
    {
        for(int i = 0; i <= iterator; i++)
        {
            int j = Random.Range(0, iterator);
            GameObject temp = Deck[j];
            Deck[j] = Deck[i];
            Deck[i] = temp;
        }
    }

    public void ShuffleAndRefill()
    {
        Shuffle();
        iterator = Deck.Count - 1;
        StartCoroutine(ReturnScale());
    }

    private IEnumerator ReturnScale()
    {
        while(transform.localScale.y < deckHeight)
        {
            Vector3 deckScale = transform.localScale;
            deckScale.y += 0.1f;
            transform.localScale = deckScale;
            yield return new WaitForSeconds(0.01f);
        }
    }

    void OutOfCards()
    {
        Debug.Log("Out of cards. (This shouldn't be possible.)");
    }

    public int GetCardsRemaining()
    {
        return iterator;
    }
}
