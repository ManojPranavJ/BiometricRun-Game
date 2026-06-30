using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReGameManager : MonoBehaviour
{

    public GameObject[] panels;
    // 1. splash 2. Main 3. booking 4. slidepanel

    public TextMeshProUGUI text;

    public bool[] bookStatus;
    public GameObject[] BookVisual;

    public int currentPage;

    public GameObject Approved;
    public bool mainBool;

    public GameObject MainControls;

    public void ChooseButton(int num)
    {
        currentPage = num;

        foreach (GameObject go in panels)
        {
            go.SetActive(false);
        }

        panels[2].SetActive(true);

        switch (num)
        {
            case 0:
                text.text = "Classroom 1";
                mainBool = bookStatus[0];
                break;

             case 1:
                text.text = "Classroom 2";
                mainBool = bookStatus[1];
                break;
            case 2:
                text.text = "Seminar hall 2";
                mainBool = bookStatus[2];
                break;

        }

        if (mainBool)
        {
            Approved.gameObject.SetActive(true);
        }
        else
        {
            Approved.gameObject.SetActive(false);
        }
    }

    public void Booked()
    {
        if(currentPage ==  0)
        {
            bookStatus[0] = true;
            BookVisual[0].SetActive(true);
        }
        else if(currentPage == 1)
        {
            bookStatus[1] = true;
            BookVisual[1].SetActive(true);
        }
        else if (currentPage == 2)
        {
            bookStatus[2] = true;
            BookVisual[2].SetActive(true);
        }
        Approved.gameObject.SetActive(true);
    }

    public void HomeButton()
    {
        mainBool = false;

        foreach (GameObject go in panels)
        {
            go.SetActive(false);
        }

        panels[1].SetActive(true);

        MainControls.SetActive(false);
    }

    public void VRButton()
    {
        foreach (GameObject go in panels)
        {
            go.SetActive(false);
        }

        MainControls.SetActive(true);
        //panels[1].SetActive(true);
    }

    public void logOut()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
