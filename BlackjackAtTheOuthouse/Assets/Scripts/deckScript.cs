using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class deckScript : MonoBehaviour
{
    //GameObject[] Deck;

    GameObject[] Deck;

    private int iterator;
    private float deckHeight;
    private void Awake()
    {
        Deck = Resources.LoadAll("Playing Cards", typeof(GameObject)).Cast<GameObject>().ToArray();
        iterator = Deck.Length - 1;
        deckHeight = transform.localScale.y;
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
            Vector3 rotation = card.transform.eulerAngles;
            rotation.x += 180;
            rotation.y += -90;
            Vector3 scale = card.transform.localScale;
            scale *= 4;
            Vector3 position = card.transform.position;
            position.y += 10;
            card.transform.position = position;
            card.transform.eulerAngles = rotation;
            card.transform.localScale = scale;
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
        for(int i = 0; i < iterator; i++)
        {
            int j = Random.Range(0, iterator);
            GameObject temp = Deck[j];
            Deck[j] = Deck[i];
            Deck[i] = temp;
        }
    }

    public void ShuffleAndRefill()
    {
        Deck = Resources.LoadAll("Playing Cards", typeof(GameObject)).Cast<GameObject>().ToArray();
        iterator = Deck.Length - 1;
    }

    void OutOfCards()
    {

    }
}
