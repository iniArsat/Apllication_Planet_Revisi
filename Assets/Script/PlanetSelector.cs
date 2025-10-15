using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;
using System.Linq;

public class PlanetSelector : MonoBehaviour
{
    [Header("UI Elements")]
    public Button buttonLeft;
    public Button buttonRight;
    public Button buttonJelajahi;
    public TextMeshProUGUI planetNameText;
    public TextMeshProUGUI buttonJelajahiText;

    [Header("Planet Settings")]
    public GameObject[] planets;
    private int currentIndex = 0;
    private int lastIndex = -1;

    [Header("Timeline Settings")]
    public PlayableDirector[] planetTimelines;

    [Header("Planet Access Settings")]
    public int[] accessiblePlanetIndices = { 0, 3 };

    [Header("Object Activation")]
    public GameObject objectToActivatefalse;
    public GameObject[] additionalObjectsToShow;
    public GameObject[] additionalPlanet;
    public GameObject[] vlcCanvasList;

    [Header("Voice Settings")]
    public AudioSource voiceSource;
    public AudioClip[] voiceClips;

    [Header("Animation Settings")]
    public GameObject[] objectsWithAnimation;

    void Start()
    {
        for (int i = 0; i < planets.Length; i++)
            planets[i].SetActive(i == currentIndex);

        UpdatePlanetDisplay();

        buttonLeft.onClick.AddListener(PreviousPlanet);
        buttonRight.onClick.AddListener(NextPlanet);
        buttonJelajahi.onClick.AddListener(OnExplore);
    }

    void PreviousPlanet()
    {
        currentIndex = (currentIndex - 1 + planets.Length) % planets.Length;
        UpdatePlanetDisplay();
    }

    void NextPlanet()
    {
        currentIndex = (currentIndex + 1) % planets.Length;
        UpdatePlanetDisplay();
    }

    void UpdatePlanetDisplay()
    {
        if (currentIndex == lastIndex) return;
        lastIndex = currentIndex;

        for (int i = 0; i < planets.Length; i++)
            planets[i].SetActive(i == currentIndex);

        if (planetNameText != null)
            planetNameText.text = planets[currentIndex].name;

        UpdateButtonStatus();
    }

    void UpdateButtonStatus()
    {
        bool canExplore = accessiblePlanetIndices.Contains(currentIndex);
        buttonJelajahi.interactable = canExplore;
        buttonJelajahiText.text = canExplore ? "ZOOM" : "COMING SOON";
    }

    void OnExplore()
    {
        Debug.Log("Explore: " + planets[currentIndex].name);

        if (planetTimelines != null && currentIndex < planetTimelines.Length && planetTimelines[currentIndex] != null)
        {
            planetTimelines[currentIndex].Play();

            if (objectToActivatefalse != null)
                objectToActivatefalse.SetActive(false);

            if (vlcCanvasList != null && vlcCanvasList.Length > 0)
            {
                for (int i = 0; i < vlcCanvasList.Length; i++)
                {
                    if (vlcCanvasList[i] != null)
                        vlcCanvasList[i].SetActive(i == currentIndex);
                }
            }

            for (int i = 0; i < additionalObjectsToShow.Length; i++)
            {
                if (additionalObjectsToShow[i] != null)
                    additionalObjectsToShow[i].SetActive(i == currentIndex);
            }

            for (int i = 0; i < additionalPlanet.Length; i++)
            {
                if (additionalPlanet[i] != null)
                    additionalPlanet[i].SetActive(i == currentIndex);
            }

            if (objectsWithAnimation != null && objectsWithAnimation.Length > 0)
            {
                foreach (GameObject obj in objectsWithAnimation)
                {
                    if (obj != null)
                    {
                        Animation anim = obj.GetComponent<Animation>();
                        if (anim != null)
                            anim.enabled = false;
                    }
                }
            }

            if (voiceSource != null && voiceClips != null && currentIndex < voiceClips.Length)
            {
                AudioClip clip = voiceClips[currentIndex];
                if (clip != null)
                {
                    voiceSource.clip = clip;
                    voiceSource.Play();
                }
            }
        }
        else
        {
            Debug.LogWarning("Timeline untuk planet ini belum di-assign di Inspector!");
        }
    }
}
