using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardScript : MonoBehaviour
{
    public enum Suit { Hearts, Diamonds, Spades, Clubs };
    public enum Face { Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King };

    [SerializeField] private Face face;
    [SerializeField] private Suit suit;
    [SerializeField] private int value;
    [SerializeField] AudioClip hittingTableSound;

    private bool isFaceUp; //purely for determining final flip position; could be used as actual information if need be, but not necessary for this game
    

    // Start is called before the first frame update
    void Start()
    {
        isFaceUp = false;
    }

    public int GetValue()
    {
        return value;
    }

    public Suit GetSuit()
    {
        return suit;
    }

    public Face GetFace()
    {
        return face;
    }

    public IEnumerator LiftAndFlip(float distance) //raises the card, flips it, and lowers it again
    {
        Vector3 originalPos = transform.position; //save this to prevent deviation
        float counter = 0;
        while(counter < distance)
        {
            Vector3 currentPos = transform.position; //moves the card upward by the amount specified in the parameter
            currentPos.y += 15 * Time.deltaTime;
            counter += 15 * Time.deltaTime;
            transform.position = currentPos;
            yield return null;
        }
        yield return StartCoroutine(Flip());
        while(counter > 0) //moves the card downward by the amount specified in the parameter
        {
            Vector3 finalPos = transform.position;
            finalPos.y -= 15 * Time.deltaTime;
            counter -= 15 * Time.deltaTime;
            transform.position = finalPos;
            yield return null;
        }
        transform.position = originalPos; //snaps back to original position to prevent deviation
    }

    public IEnumerator Flip()
    {
        //Flips the card by roughly 180 degrees
        float degrees = 180;
        while(degrees > 0)
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.z += 400 * Time.deltaTime;
            degrees -= 400 * Time.deltaTime;
            transform.eulerAngles = currentRotation;
            yield return null;
        }
        //Updates the status of the direction the card is facing
        isFaceUp = !isFaceUp;
        //The previous flip can cause small discrepencies, this will correct them
        Vector3 finalRotation = transform.eulerAngles;
        if (isFaceUp)
            finalRotation.z = 0;
        else
            finalRotation.z = 180;
        transform.eulerAngles = finalRotation;
        
    }

    public IEnumerator MoveCard(Vector3 destination) //moves the card in the direction of the destination until the destination is reached
    {
        float speed = 30;
        while(Vector3.Distance(transform.position, destination) > 1.0f)
        {
            Vector3 direction = (destination - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * speed, Space.World);
            yield return null;
        }
        transform.position = destination;
    }

    public void changeValue(int i)
    {
        value = i;
    }
}
