using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TimelineMediaPlayer : MonoBehaviour
{
    [Header("References")]
    public PlayableDirector timelineDirector;
    public Slider timelineSlider;
    public Button playPauseButton;
    public Button backButton;
    public TextMeshProUGUI playPauseText;
    public TextMeshProUGUI timeText;

    private bool isDragging = false;
    private bool isPlaying = false;

    void Start()
    {
        if (timelineDirector == null)
            timelineDirector = GetComponent<PlayableDirector>();

        if (timelineSlider != null)
        {
            timelineSlider.minValue = 0f;
            timelineSlider.maxValue = 1f;
            timelineSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        if (playPauseButton != null)
            playPauseButton.onClick.AddListener(TogglePlayPause);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonPressed);

        UpdateButtonText();
    }

    void Update()
    {
        if (timelineDirector == null) return;

        if (!isDragging && timelineDirector.duration > 0)
        {
            double current = timelineDirector.time;
            double total = timelineDirector.duration;
            timelineSlider.value = (float)(current / total);
        }

        UpdateTimeText();

        if (Input.GetKeyDown(KeyCode.Space))
            TogglePlayPause();

        if (!isPlaying)
            timelineDirector.Evaluate();
    }

    public void OnSliderBeginDrag() => isDragging = true;

    public void OnSliderEndDrag()
    {
        isDragging = false;
        if (timelineDirector == null) return;

        double targetTime = timelineDirector.duration * timelineSlider.value;
        timelineDirector.time = targetTime;
        timelineDirector.Evaluate();
    }

    void OnSliderValueChanged(float value)
    {
        if (isDragging && timelineDirector != null)
        {
            double targetTime = timelineDirector.duration * value;
            timelineDirector.time = targetTime;
            timelineDirector.Evaluate();
        }
    }

    void TogglePlayPause()
    {
        if (timelineDirector == null) return;

        if (timelineDirector.state == PlayState.Playing)
        {
            timelineDirector.Pause();
            isPlaying = false;
        }
        else
        {
            timelineDirector.Play();
            isPlaying = true;
        }

        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        if (playPauseText != null)
            playPauseText.text = isPlaying ? "Pause" : "Play";
    }

    void UpdateTimeText()
    {
        if (timeText == null || timelineDirector == null) return;

        double current = timelineDirector.time;
        double total = timelineDirector.duration;

        System.TimeSpan cur = System.TimeSpan.FromSeconds(current);
        System.TimeSpan tot = System.TimeSpan.FromSeconds(total);

        timeText.text = $"{cur.Minutes:D2}:{cur.Seconds:D2} / {tot.Minutes:D2}:{tot.Seconds:D2}";
    }

    void OnBackButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
