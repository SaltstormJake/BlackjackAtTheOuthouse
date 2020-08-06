﻿using System.Collections;
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

    private bool isFaceUp;
    

    // Start is called before the first frame update
    void Start()
    {
        isFaceUp = false;
    }

    // Update is called once per frame
    void Update()
    {

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

    public IEnumerator LiftAndFlip(float distance)
    {
        Vector3 originalPos = transform.position;
        float counter = 0;
        while(counter < distance)
        {
            Vector3 currentPos = transform.position;
            currentPos.y += 15 * Time.deltaTime;
            counter += 15 * Time.deltaTime;
            transform.position = currentPos;
            yield return null;
        }
        yield return StartCoroutine(Flip());
        while(counter > 0)
        {
            Vector3 finalPos = transform.position;
            finalPos.y -= 15 * Time.deltaTime;
            counter -= 15 * Time.deltaTime;
            transform.position = finalPos;
            yield return null;
        }
        transform.position = originalPos;
    }

    public IEnumerator RaiseAndLowerCard(float distance)
    {
        Vector3 originalPos = transform.position; //return to this later to fix slight deviations
        //Raises the card above the table
        while (distance > 0)
        {
            Vector3 currentPos = transform.position;
            currentPos.y += 15 * Time.deltaTime;
            distance -= 15 * Time.deltaTime;
            transform.position = currentPos;
            yield return null;
        }
        while(distance < 1)
        {
            Vector3 finalPos = transform.position;
            finalPos.y -= 15 * Time.deltaTime;
            distance += 15 * Time.deltaTime;
            transform.position = finalPos;
            yield return null;
        }
        transform.position = originalPos; //to fix slight discrepencies in positioning
    }

    public IEnumerator Flip()
    {
        //Flips the card by roughly 180 degrees
        float degrees = 180;
        while(degrees > 0)
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.z += 200 * Time.deltaTime;
            degrees -= 200 * Time.deltaTime;
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

    public IEnumerator MoveCard(Vector3 destination)
    {
        float speed = 30;
        while(Vector3.Distance(transform.position, destination) > 0.2f)
        {
            Vector3 direction = (destination - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * speed, Space.World);
            yield return null;
        }
        transform.position = destination;
        //AudioSource.PlayClipAtPoint(hittingTableSound, transform.position, 1.0f);
    }

    public void changeValue(int i)
    {
        value = i;
    }
}
