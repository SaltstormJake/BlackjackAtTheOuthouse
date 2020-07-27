using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class blackjackUIScript : MonoBehaviour
{
    GameObject[] UIObjects;
    [SerializeField] Button DealButton;
    [SerializeField] Button HitButton;
    [SerializeField] Button StandButton;
    [SerializeField] Button DoubleDownButton;
    [SerializeField] Button SplitButton;
    [SerializeField] Button InsuranceButton;
    [SerializeField] Text fundsTextNumber;
    [SerializeField] dealerScript dealer;
    [SerializeField] playerScript player;
    int funds = 1000;


    private void Awake()
    {
        UIObjects = GameObject.FindGameObjectsWithTag("UIOnly");
        DealButton.onClick.AddListener(OnDealClick);
        HitButton.onClick.AddListener(OnHitClick);
        StandButton.onClick.AddListener(OnStandClick);
        DoubleDownButton.onClick.AddListener(OnDoubleDownClick);
        SplitButton.onClick.AddListener(OnSplitClick);
        InsuranceButton.onClick.AddListener(OnInsuranceClick);
    }

    // Start is called before the first frame update
    void Start()
    {
        fundsTextNumber.text = funds.ToString();
        SetUI(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearTable()
    {
        dealer.ClearHand();
        player.ClearHand();
    }


    public void SetDeal(bool enabled)
    {
        DealButton.gameObject.SetActive(enabled);
    }

    public void SetHitAndStand(bool enabled)
    {
        HitButton.gameObject.SetActive(enabled);
        StandButton.gameObject.SetActive(enabled);
    }

    public void SetDoubleDown(bool enabled)
    {
        DoubleDownButton.gameObject.SetActive(enabled);
    }

    public void SetSplit(bool enabled)
    {
        SplitButton.gameObject.SetActive(enabled);
    }

    public void SetInsurance(bool enabled)
    {
        InsuranceButton.gameObject.SetActive(enabled);
    }

    public void SetUI(bool enabled)
    {
        foreach (GameObject g in UIObjects)
            g.SetActive(enabled);
    }

    public void ChangeFunds(int i)
    {
        funds += i;
        fundsTextNumber.text = funds.ToString();
    }

    public void OnDealClick()
    {
        ClearTable();
        SetDeal(false);
        StartCoroutine(dealer.Deal());
        player.ToggleTableLean();
    }

    private void OnHitClick()
    {
        StartCoroutine(dealer.Hit());
        SetHitAndStand(false);
        SetDoubleDown(false);
    }

    private void OnStandClick()
    {
        StartCoroutine(dealer.Stand());
        SetHitAndStand(false);
        SetDoubleDown(false);
    }

    private void OnDoubleDownClick()
    {
        dealer.DoubleDown();
        SetHitAndStand(false);
        SetDoubleDown(false);
    }

    private void OnSplitClick()
    {

    }
    
    private void OnInsuranceClick()
    {

    }

}
