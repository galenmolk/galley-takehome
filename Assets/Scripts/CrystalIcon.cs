using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CrystalIcon : MonoBehaviour
{
    private readonly int RevealPropId = Shader.PropertyToID("_RevealPercentage");

    [field: SerializeField] public Crystal.Type Type { get; private set; }

    public int CrystalCount { get; private set; }

    [SerializeField] private Image crystalImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private float tweenDuration = 1f;
    [SerializeField] private Ease tweenEase = Ease.OutSine;
    [SerializeField] private float borderTweenDuration = 0.3f;
    [SerializeField] private Ease borderTweenEase = Ease.OutCubic;

    private void Awake()
    {
        borderImage.color = Color.clear;
        crystalImage.material = new Material(crystalImage.material);
        crystalImage.material.SetFloat(RevealPropId, 0f);
    }

    public void AddCrystal()
    {
        CrystalCount++;

        crystalImage.material.DOKill();
        DOTween.Sequence()
            .Append(borderImage.DOColor(Color.white, borderTweenDuration).SetEase(tweenEase))
            .Append(crystalImage.material.DOFloat(CrystalCount / (float)Crystal.MaxNeededPerType, RevealPropId, tweenDuration).SetEase(tweenEase));
    }

    public void HideBorder()
    {
        borderImage.color = Color.clear;
    }
}
