using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildSlot : MonoBehaviour
{
    [SerializeField] private Image craftImage;
    [SerializeField] private TMPro.TextMeshProUGUI craftName;
    [SerializeField] private TMPro.TextMeshProUGUI craftDescription;
    private BuildSO buildSO;

    public void SetBuildSlot(BuildSO item)
    {
        buildSO = item;
        craftImage.sprite = item.requiredItems[0].itemSprite;
        craftName.text = buildSO.buildName;
        craftDescription.text = "";
        for (int i = 0; i < item.requiredItems.Length; i++)
        {
            craftDescription.text += item.requiredItems[i].itemName + " x" + item.requiredAmounts[i] + "\n";
        }
    }

    public void OnClick()
    {
        FindObjectOfType<BuildManager>().CanBuild(buildSO);
    }
}
