using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidePanel : MonoBehaviour
{
    [Header("Rects")]
    public RectTransform rect;
    public MainPanel mainPanel;

    [Header("Anchor")]
    public Vector2 anchorFull;
    public Vector2 anchorFolded;
    bool isFolded;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        isFolded = false;
        rect.anchoredPosition = anchorFull;
    }

    public void Fold()
    {
        if(isFolded)
        {
            rect.anchoredPosition = anchorFull;
            mainPanel.rect.anchoredPosition = mainPanel.anchorFull;

        }
        else
        {
            rect.anchoredPosition = anchorFolded;
            mainPanel.rect.anchoredPosition = mainPanel.anchorFolded;
        }
    }
}
