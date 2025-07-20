using System.Collections.Generic;
using UnityEngine;

public class CrystalTrackerPanel : MonoBehaviour
{
    private Dictionary<Crystal.Type,>

    class CrystalState {
        public int count;
        public
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
        
    }
}
