using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class optionsMenuScript : MonoBehaviour
{

    [SerializeField] Button mainMenuButton;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider voiceSlider;
    [SerializeField] Toggle valuesInUIToggle;
    bool valuesInUI;
    [SerializeField] Toggle fiveCardCharlieToggle;
    bool disableFiveCardCharlie;
    [SerializeField] Text fiveCardCharlieWarning;

    [SerializeField] List<AudioSource> soundSources;
    List<float> originalSoundValues;
    [SerializeField] List<AudioSource> musicSources;
    List<float> originalMusicValues;
    [SerializeField] List<AudioSource> voiceSources;
    List<float> originalVoiceValues;

    [SerializeField] mainMenuScript mainMenu;
    GameObject[] menuElements;


    private void Awake()
    {
        originalSoundValues = new List<float>();
        originalMusicValues = new List<float>();
        originalVoiceValues = new List<float>();
        menuElements = GameObject.FindGameObjectsWithTag("optionsOnly");
        mainMenuButton.onClick.AddListener(OnClickMainMenu);
        masterVolumeSlider.onValueChanged.AddListener(OnValueChangedMasterSlider);
        soundSlider.onValueChanged.AddListener(OnValueChangedSoundSlider);
        musicSlider.onValueChanged.AddListener(OnValueChangedMusicSlider);
        voiceSlider.onValueChanged.AddListener(OnValueChangedVoiceSlider);

        valuesInUIToggle.onValueChanged.AddListener(OnValueChangedValuesInUIToggle);
        fiveCardCharlieToggle.onValueChanged.AddListener(OnValueChangedFiveCardCharlieToggle);

        foreach (AudioSource a in soundSources)
            originalSoundValues.Add(a.volume);
        foreach (AudioSource a in musicSources)
            originalMusicValues.Add(a.volume);
        foreach (AudioSource a in voiceSources)
            originalVoiceValues.Add(a.volume);

        valuesInUI = true;
        disableFiveCardCharlie = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetScreen(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScreen(bool enabled)
    {
        foreach (GameObject g in menuElements)
            g.SetActive(enabled);
        if (!disableFiveCardCharlie)
            fiveCardCharlieWarning.gameObject.SetActive(false);
    }

    void OnClickMainMenu()
    {
        SetScreen(false);
        mainMenu.SetScreen(true);
    }

    void OnValueChangedMasterSlider(float value)
    {
        AudioListener.volume = value;
    }

    void OnValueChangedSoundSlider(float value)
    {
        foreach (AudioSource a in soundSources)
            a.volume = originalSoundValues[soundSources.IndexOf(a)] * value;

    }

    void OnValueChangedMusicSlider(float value)
    {
        foreach (AudioSource a in musicSources)
            a.volume = originalMusicValues[musicSources.IndexOf(a)] * value;
    }

    void OnValueChangedVoiceSlider(float value)
    {
        foreach (AudioSource a in voiceSources)
            a.volume = originalVoiceValues[voiceSources.IndexOf(a)] * value;
    }

    void OnValueChangedValuesInUIToggle(bool isOn)
    {
        valuesInUI = isOn;
    }

    void OnValueChangedFiveCardCharlieToggle(bool isOn)
    {
        disableFiveCardCharlie = isOn;
        if (isOn)
            fiveCardCharlieWarning.gameObject.SetActive(true);
        else
            fiveCardCharlieWarning.gameObject.SetActive(false);
    }

    public bool GetShowOnUIToggle()
    {
        return valuesInUI;
    }

    public bool GetFiveCardCharlieToggleDisabled()
    {
        return disableFiveCardCharlie;
    }

}
