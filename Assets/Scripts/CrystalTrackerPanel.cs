using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class CrystalTrackerPanel : MonoBehaviour
{
    private Dictionary<Crystal.Type, CrystalState> crystalStates = new();

    class CrystalState
    {
        public int count;
        public CrystalIcon icon;

        public CrystalState(CrystalIcon icon)
        {
            this.icon = icon;
            this.count = 0;
        }
    }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip tweenSfx;
    [SerializeField] private RectTransform rt;
    [SerializeField] private float tweenDuration = 0.7f;
    [SerializeField] private float pauseDuration = 3f;
    [SerializeField] private float winTweenDuration = 1.5f;
    [SerializeField] private Ease winTweenEase = Ease.InOutCubic;
    [SerializeField] private float winScale = 3f;
    [SerializeField] private Ease tweenEase = Ease.OutCubic;
    [SerializeField] private float delayBetweenCrystals = 0.4f;

    [SerializeField] private CanvasGroup gameFadePanel;
    [SerializeField] private float winDelay = 4f;
    [SerializeField] private float gameFadePanelDuration = 3f;


    private Vector2 onScreenPos;
    Vector2 offScreenPos;
    private Vector2 winPos;
    private bool isPanelVisible;

    private bool isProcessing;
    private readonly Queue<CrystalState> crystalQueue = new();

    public Crystal.Type testType;
    [ContextMenu("test")]
    public void Test()
    {
        EnqueueCrystal(testType);
    }

    private void Start()
    {
        gameFadePanel.alpha = 1f;
        gameFadePanel.DOFade(0f, gameFadePanelDuration);

        float panelHalfHeight = rt.rect.height * 0.5f;
        float canvasHalfHeight = ((RectTransform)rt.root).rect.height * 0.5f;

        Debug.Log($"Half height: {panelHalfHeight} {canvasHalfHeight}");
        onScreenPos = new Vector2(0f, -canvasHalfHeight + panelHalfHeight + 50f); 
        offScreenPos = new Vector2(0f, -canvasHalfHeight - panelHalfHeight - 50f);
        winPos = Vector2.zero;

        rt.anchoredPosition = offScreenPos;

        crystalStates = transform.
            GetComponentsInChildren<CrystalIcon>().
            ToDictionary(
                icon => icon.Type,
                icon => new CrystalState(icon)
            );
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
        if (!crystalStates.TryGetValue(type, out var crystalState))
        {
            Debug.LogError($"[{nameof(CrystalTrackerPanel)}.{nameof(EnqueueCrystal)}] No crystal state found for type ({type}).");
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
            var crystalState = crystalQueue.Dequeue();

            crystalState.icon.AddCrystal();
            audioSource.clip = collectClip;
            audioSource.Play();

            await Task.Delay((int)(pauseDuration * 1000));
        }

        if (crystalStates.Values.Sum(state => state.count) >= Crystal.MaxNeededPerType * crystalStates.Count)
        {
            WinGame();
            return;
        }

        await rt.DOAnchorPos(offScreenPos, tweenDuration).SetEase(tweenEase).AsyncWaitForCompletion();
        isPanelVisible = false;
        isProcessing = false;
    }

    [ContextMenu("Win")]
    private void WinTest()
    {
        rt.DOAnchorPos(onScreenPos, tweenDuration).SetEase(tweenEase).OnComplete(() =>
        {
            WinGame();
        });
    }

    private void WinGame()
    {
        audioSource.PlayOneShot(winClip);

        var winSequence = DOTween.Sequence();
        winSequence
            .Append(rt.DOAnchorPos(winPos, winTweenDuration).SetEase(winTweenEase))
            .Join(rt.DOScale(Vector3.one * winScale, winTweenDuration).SetEase(winTweenEase))
            .AppendInterval(winDelay)
            .Append(gameFadePanel.DOFade(1f, gameFadePanelDuration))
            .AppendCallback(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
    }
}
