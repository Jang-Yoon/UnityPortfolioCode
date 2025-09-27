using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guided : MonoBehaviour
{
    [SerializeField] private GameObject[] guidedPages;
    [SerializeField] private GameObject buttonPrevious;
    [SerializeField] private GameObject guidedBackground;
    [SerializeField]private int currentPage;

    private void Start()
    {
        guidedPages[currentPage].SetActive(true);
    }

    public void OpenGuided()
    {
        guidedBackground.SetActive(true);
        currentPage = 0;
        guidedPages[currentPage].SetActive(true);
        buttonPrevious.SetActive(false);
    }

    public void Next()
    {
        guidedPages[currentPage].SetActive(false);
        currentPage++;
        if (currentPage == guidedPages.Length)
        {
            guidedBackground.SetActive(false);
            return;
        }
        guidedPages[currentPage].SetActive(true);
        if (currentPage != 0) buttonPrevious.SetActive(true);
    }

    public void Previous()
    {
        guidedPages[currentPage].SetActive(false);
        currentPage--;
        guidedPages[currentPage].SetActive(true);
        if (currentPage == 0) buttonPrevious.SetActive(false);
    }
}
