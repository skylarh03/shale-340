using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeOption : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> _borderComponents;
    public TMP_Text _upgradeName;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SelectOption()
    {
        _borderComponents.ForEach(x => x.color = new Color(0.9960785f, 0.9019608f, 0.1058824f));
    }

    public void DeselectOption()
    {
        _borderComponents.ForEach(x => x.color = Color.white);
    }

    public void SetTitle(Utilities.Upgrades upgrade)
    {
        _upgradeName.text = Utilities.UpgradeNames[(int)upgrade];
    }
}
