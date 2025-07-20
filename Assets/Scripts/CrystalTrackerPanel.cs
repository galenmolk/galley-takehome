using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class CrystalTrackerPanel : MonoBehaviour
{
    private Dictionary<Crystal.Type, CrystalIcon> crystalIcons = new();

    [Header("Audio Clips")]
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip tweenSfx;

    [Header("Panel Tween Settings")]
    [SerializeField] private float tweenDuration = 0.7f;
    [SerializeField] private float pauseDuration = 3f;
    [SerializeField] private Ease tweenEase = Ease.OutCubic;


    [Header("Win Flow Settings")]
    [SerializeField] private float winTweenDuration = 1.5f;
    [SerializeField] private Ease winTweenEase = Ease.InOutCubic;
    [SerializeField] private float winScale = 3f;
    [SerializeField] private float winDelay = 4f;

    [Header("General Settings")]
    [SerializeField] private float gamefadeInDelay = 2f;
    [SerializeField] private float gameFadePanelDuration = 3f;

    [Header("References")]
    [SerializeField] private CanvasGroup gameFadePanel;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private RectTransform rt;

    private Vector2 onScreenPos;
    Vector2 offScreenPos;
    private Vector2 winPos;
    private bool isPanelVisible;
    private bool isProcessing;
    private readonly Queue<CrystalIcon> crystalQueue = new();

    private void Start()
    {
        // Start with a black screen
        gameFadePanel.alpha = 1f;

        // Calculate panel positions for later.
        float panelHalfHeight = rt.rect.height * 0.5f;
        float canvasHalfHeight = ((RectTransform)rt.root).rect.height * 0.5f;
        onScreenPos = new Vector2(0f, -canvasHalfHeight + panelHalfHeight + 50f);
        offScreenPos = new Vector2(0f, -canvasHalfHeight - panelHalfHeight - 50f);
        winPos = Vector2.zero;

        // Ensure panel is hidden.
        rt.anchoredPosition = offScreenPos;

        crystalIcons = transform.
            GetComponentsInChildren<CrystalIcon>().
            ToDictionary(
                icon => icon.Type,
                icon => icon
            );

        DOTween.Sequence()
            .AppendInterval(gamefadeInDelay)
            .Append(gameFadePanel.DOFade(0f, gameFadePanelDuration));
    }

    private void OnEnable()
    {
        Crystal.OnCrystalAcquired += EnqueueCrystal;
    }

    private void OnDisable()
    {
        Crystal.OnCrystalAcquired -= EnqueueCrystal;
    }

    private void EnqueueCrystal(Crystal.Type type)
    {
        if (!crystalIcons.TryGetValue(type, out var crystalState))
        {
            Debug.LogError($"[{nameof(CrystalTrackerPanel)}.{nameof(EnqueueCrystal)}] " +
             "No crystal state found for type ({type}).");
            return;
        }

        crystalQueue.Enqueue(crystalState);
    }

    private void Update()
    {
        if (!isProcessing && crystalQueue.Count > 0)
        {
            _ = ProcessCrystalQueue();
        }
    }

    private async Task ProcessCrystalQueue()
    {
        isProcessing = true;

        if (!isPanelVisible)
        {
            await rt.DOAnchorPos(onScreenPos, tweenDuration).SetEase(tweenEase).AsyncWaitForCompletion();
            isPanelVisible = true;
        }

        while (crystalQueue.Count > 0)
        {
            var crystalIcon = crystalQueue.Dequeue();

            crystalIcon.AddCrystal();
            audioSource.Play();

            await Task.Delay((int)(pauseDuration * 1000));
        }

        if (crystalIcons.Values.Sum(icon => icon.CrystalCount) >= Crystal.MaxNeededPerType * crystalIcons.Count)
        {
            WinGame();
            return;
        }

        await rt.DOAnchorPos(offScreenPos, tweenDuration).SetEase(tweenEase).AsyncWaitForCompletion();

        foreach (var icon in crystalIcons.Values)
        {
            icon.HideBorder();
        }

        isPanelVisible = false;
        isProcessing = false;
    }

    private void WinGame()
    {
        audioSource.PlayOneShot(winClip);

        DOTween.Sequence()
            .Append(rt.DOAnchorPos(winPos, winTweenDuration).SetEase(winTweenEase))
            .Join(rt.DOScale(Vector3.one * winScale, winTweenDuration).SetEase(winTweenEase))
            .AppendInterval(winDelay)
            .Append(gameFadePanel.DOFade(1f, gameFadePanelDuration))
            .Join(musicSource.DOFade(0f, gameFadePanelDuration))
            .AppendCallback(ReloadScene);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
