using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;

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

    [SerializeField] private Vector2 onScreenPos;
    [SerializeField] private Vector2 offScreenPos;
    [SerializeField] private float tweenDuration = 0.7f;
    [SerializeField] private float pauseDuration = 3f;
    [SerializeField] private Ease tweenEase = Ease.OutCubic;
    [SerializeField] private float delayBetweenCrystals = 0.4f;

    private bool isPanelVisible;

    private bool isProcessing;
    private Queue<CrystalState> crystalQueue = new();

    public Crystal.Type testType;
    [ContextMenu("test")]
    public void Test()
    {
        EnqueueCrystal(testType);
    }

    private void Awake()
    {
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

        await rt.DOAnchorPos(offScreenPos, tweenDuration).SetEase(tweenEase).AsyncWaitForCompletion();
        isPanelVisible = false;
        isProcessing = false;
    }
}
