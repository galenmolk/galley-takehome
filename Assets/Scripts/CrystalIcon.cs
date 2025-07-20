using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CrystalIcon : MonoBehaviour
{
    private const int MaximumCrystals = 3;
    private readonly int RevealPropId = Shader.PropertyToID("_RevealPercentage");

    [field: SerializeField] public Crystal.Type Type { get; private set; }

    public int CrystalCount { get; private set; }

    [SerializeField] private Image crystalImage;
    [SerializeField] private float tweenDuration = 1f;
    [SerializeField] private Ease tweenEase = Ease.OutSine;

    private void Awake()
    {
        crystalImage.material = new Material(crystalImage.material);
        crystalImage.material.SetFloat(RevealPropId, 0f);
    }

    public void AddCrystal()
    {
        CrystalCount++;

        crystalImage.material.DOKill();
        crystalImage.material.DOFloat(CrystalCount / (float)MaximumCrystals, RevealPropId, tweenDuration).SetEase(tweenEase);
    }
}
