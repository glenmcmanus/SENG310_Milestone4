using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidePanel : Foldout
{
    public MainPanel mainPanel;
    public Foldout foldoutButton;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        rect.anchorMin.Set(anchorFullMin.x, anchorFullMin.y);
        rect.anchorMax.Set(anchorFullMax.x, anchorFullMax.y);
    }

    public override void Fold()
    {
        base.Fold();
        AdjustMain();
    }

    public void AdjustMain()
    {
        if(isFolded)
        {
            //foldoutButton.Fold();
            foldoutButton.GetComponentInChildren<Text>().text = "<";
            //mainPanel.rect.anchorMin = new Vector2(anchorFoldedMax.x, mainPanel.rect.anchorMin.y);
            //mainPanel.rect.anchoredPosition = Vector2.zero;
        }
        else
        {
            //foldoutButton.Fold();
            foldoutButton.GetComponentInChildren<Text>().text = ">";
            //mainPanel.rect.anchorMin = new Vector2(anchorFullMax.x, mainPanel.rect.anchorMin.y);
            //mainPanel.rect.anchoredPosition = Vector2.zero;
        }

    }
}
