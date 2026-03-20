using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class OptionsMenu : MonoBehaviour
{
    [Header("Menu")]
    public GameObject MenuPanel; //main menu lub ESC menu
    public GameObject optionsPanel;
    public GameObject areYouSurePrompt;

    [Header("Audio")]
    public Slider volumeSlider;
    public AudioMixer audioMixer;
    public TextMeshProUGUI volumeValueText;
    private float savedVolumeValue;
    private AudioSource audioSource;
    //TODO: change comments to english
    public void OnEnable() //ta sekcja będzie się robić strasznie długa jak będziemy dodawać kolejne opcje
    {
        if (!PlayerPrefs.HasKey("volume"))
        {
            PlayerPrefs.SetFloat("volume", 1f);
            Debug.Log("Default volume loaded");
        }
        // Bufor = wartość suwaka = PlayerPrefs
        savedVolumeValue = PlayerPrefs.GetFloat("volume");
        volumeSlider.value = savedVolumeValue;
        audioSource = GetComponent<AudioSource>();
    }
    public void SetVolume(float value)
    {
        savedVolumeValue = value;
        volumeValueText.text = Mathf.RoundToInt(value * 100) + "%";
    }
    public void SaveChanges() //zapisz ustawienia
    {
        //dźwięki. Ustawienie logarytmiczne. Słyszałem w internecie, że tak jest naturalniej dla człowieka
        //ten kod robi, że technicznie to nie da się da końca wyłączyć dźwięku (min. 0.0001f), żeby uniknąć błędu. Całkowite wyłączenie dźwięku może psuć dalszy kod jeśli będę coś
        //mieszał przy audioMixer. Niech tak zostanie, tak będzie prościej.
        float dB = Mathf.Log10(Mathf.Max(savedVolumeValue, 0.0001f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);

        //cała reszta
        PlayerPrefs.SetFloat("volume", savedVolumeValue);
        volumeSlider.value = savedVolumeValue;
        Debug.Log("Volume set to: " + savedVolumeValue + " (dB: " + dB + ")");
        PlaySound();
    }
    //----------- Are you sure prompt -----------
    public void ConfirmChanges()
    {
        SaveChanges();
        BackToMenu();
        areYouSurePrompt.SetActive(false);
        PlaySound();
    }
    public void DiscardChanges()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        optionsPanel.SetActive(false);
        MenuPanel.SetActive(true);
        areYouSurePrompt.SetActive(false);
        PlaySound();
    }
    public void CancelDecision()
    {
        areYouSurePrompt.SetActive(false);
        optionsPanel.SetActive(true);
        PlaySound();
    }
    //------------------------------------
    //language button is handled in localization script
    public void BackToMenu()
    {
        if(ChangesMade())
        {
            areYouSurePrompt.SetActive(true);
            optionsPanel.SetActive(false);
        }
        else
        {
            optionsPanel.SetActive(false);
            MenuPanel.SetActive(true);
        }
        PlaySound();
    }
    public bool ChangesMade()
    {
        return savedVolumeValue != PlayerPrefs.GetFloat("volume");
    }
    public void ResetAllSettings()
    {
        SetVolume(1f);
        ConfirmChanges();
        PlaySound();
    }

    //----------- SOUND -----------
    private void PlaySound()
    {
        audioSource.Play();
    }
}
