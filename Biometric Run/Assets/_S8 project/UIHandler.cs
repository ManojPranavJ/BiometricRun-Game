using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject hideUI;

    [Header("Toggles")]
    [SerializeField] private Toggle[] toggles; // 4 toggles in an array
    [SerializeField] private Button Bookhallbtn;
    [SerializeField] private Button Cancelhallbtn;

    [Space]
    [SerializeField] private FirestoreManager firebaseManager;

    public int hallcode01 = 0;
    public int hallcode02 = 0;
    public int hallcode03 = 0;
    public int hallcode04 = 0;

    public void Start()
    {
        Bookhallbtn.onClick.AddListener(BookHall);
        Cancelhallbtn.onClick.AddListener(CancelHall);
    }

    public void OnEnable()
    {
        
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            mainUI.SetActive(!mainUI.activeSelf);
            firebaseManager.GetLatestDataFromFirestore();
            updateUIToggle();
        }
    }

    public void updateUIToggle()
    {
        toggles[0].isOn = hallcode01==1 ? true : false;
        toggles[1].isOn = hallcode02 == 1 ? true : false;
        toggles[2].isOn = hallcode03 == 1 ? true : false;
        toggles[3].isOn = hallcode04 == 1 ? true : false;
    }

    public IEnumerator ButtonControl()
    {
        /*if(Bookhallbtn.gameObject.activeSelf == true)
        {
            Bookhallbtn.gameObject.SetActive(false);
        }
        else
        {
            Bookhallbtn.gameObject.SetActive(true);
        }

        if(Cancelhallbtn.gameObject.activeSelf == true)
        {
            Cancelhallbtn.gameObject.SetActive(false);
        }
        else
        {
            Cancelhallbtn.gameObject.SetActive(true);
        }*/
        

        Bookhallbtn.interactable = false;
        Cancelhallbtn.interactable = false;

        yield return new WaitForSeconds(5);

        Bookhallbtn.interactable = true;
        Cancelhallbtn.interactable = true;
    }

    public void BookHall()
    {
        /*// Assign values based on toggle state
        for (int i = 0; i < toggles.Length; i++)
        {
            hallCodes[i] = toggles[i].isOn ? 1 : 0;
        }*/
        hideUI.SetActive(true);
        StartCoroutine(ButtonControl());

        hallcode01 = toggles[0].isOn ? 1 : 0;
        hallcode02 = toggles[1].isOn ? 1 : 0;
        hallcode03 = toggles[2].isOn ? 1 : 0;
        hallcode04 = toggles[3].isOn ? 1 : 0;

        // Send updated data to Firebase
        firebaseManager.SendDataToFirestore(hallcode01, hallcode02, hallcode03, hallcode04, "Sanjay");
    }

    public void CancelHall()
    {
        hideUI.SetActive(false);
        ButtonControl();

        hallcode01 = 0;
        hallcode02 = 0;
        hallcode03 = 0;
        hallcode04 = 0;

        foreach(var item in toggles)
        {
            item.isOn = false;
        }

        // Send reset data to Firebase
        firebaseManager.SendDataToFirestore(hallcode01, hallcode02, hallcode03, hallcode04, "Sanjay");
    }
}
