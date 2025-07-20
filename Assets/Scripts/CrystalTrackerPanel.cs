using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private void Awake()
    {
        crystalStates = transform.
            GetComponentsInChildren<CrystalIcon>().
            ToDictionary(
                icon => icon.Type,
                icon => new CrystalState(icon)
            );
    }

    private void OnEnable()
    {
        Crystal.OnCrystalAcquired += UpdateCrystalCount;
    }

    private void OnDisable()
    {
        Crystal.OnCrystalAcquired -= UpdateCrystalCount;
    }

    private void UpdateCrystalCount(Crystal.Type type)
    {
        if (!crystalStates.TryGetValue(type, out var crystalState))
        {
            Debug.LogError($"[{nameof(CrystalTrackerPanel)}.{nameof(UpdateCrystalCount)}] No crystal state found for type ({type}).");
            return;
        }

        crystalState.icon.AddCrystal();
    }
}
