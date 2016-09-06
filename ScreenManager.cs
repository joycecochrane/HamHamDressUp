using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System.IO;

public class ScreenManager : MonoBehaviour {

    public GameObject root;
    // Introduction Screen
    public GameObject introScreen;
    public GameObject optionScreen;
    public GameObject loadingScreen;
    public GameObject dressUpScreen;
    public GameObject storeScreen;

    private GameObject ratingScreen;
    private GameObject popUpMenu;

    // Boolean
    private List<GameObject> screens;
	private bool fromIntro;

    void Awake() {
        ratingScreen = dressUpScreen.transform.GetChild(4).gameObject;
        popUpMenu = dressUpScreen.transform.GetChild(5).gameObject;
    }

    void Start() {
        // Set up screen visibility
		introScreen.SetActive(true);
		optionScreen.SetActive(false);
		loadingScreen.SetActive(false);
		dressUpScreen.SetActive(false);
		ratingScreen.SetActive(false);
		storeScreen.SetActive(false);
		popUpMenu.SetActive(false);

		fromIntro = true;
    }

    // Display Dress up screen
    public void StartDressUp() {
		fromIntro = false;
        introScreen.SetActive(false);
        ToggleLoadingScreen();
        // Wait for 3 seconds
        Invoke("ToggleLoadingScreen", 2);
        Invoke("ToggleDressUpScreen", 2);
    }
 

    // Toggle visibility of the Option Screen
    public void ToggleOptionScreen() {
        optionScreen.SetActive(!optionScreen.activeSelf);

		if (fromIntro) {
            introScreen.SetActive(!introScreen.activeSelf);
        } else {
            dressUpScreen.SetActive(!dressUpScreen.activeSelf);
        }
    }

    // Toggle visiblity of the Intro Screen
    public void ToggleIntroScreen() {
        introScreen.SetActive(!introScreen.activeSelf);
        dressUpScreen.SetActive(!dressUpScreen.activeSelf);
    }

    // Toggle visibility of the Loading Screen
    private void ToggleLoadingScreen() {
        loadingScreen.SetActive(!loadingScreen.activeSelf);
    }

    // Toggle visibility of the DressUp Screen
    private void ToggleDressUpScreen() {
        dressUpScreen.SetActive(!dressUpScreen.activeSelf);
    }

    // Toggle visibility of the Rating Screen
    public void ToggleRatingScreen() {
        ratingScreen.SetActive(!ratingScreen.activeSelf);
    }

    // Toggle visibility of the Popup Menu
    public void TogglePopUpMenu() {
        popUpMenu.SetActive(!popUpMenu.activeSelf);
    }

    // Toggle Visibility of the store screen
    public void ToggleStoreScreen() {
        popUpMenu.SetActive(false);
        dressUpScreen.SetActive(!dressUpScreen.activeSelf);
        storeScreen.SetActive(!storeScreen.activeSelf);
    }

    public void PopUpOption() {
        popUpMenu.SetActive(!popUpMenu.activeSelf);
        dressUpScreen.SetActive(!dressUpScreen.activeSelf);
        optionScreen.SetActive(!optionScreen.activeSelf);
    }

    public void PopUpMain() {
        popUpMenu.SetActive(!popUpMenu.activeSelf);
        dressUpScreen.SetActive(!dressUpScreen.activeSelf);
        introScreen.SetActive(!introScreen.activeSelf);
    }

    private void SetUnifiedAnchor(GameObject screen) {
        screen.GetComponent<UIWidget>().leftAnchor.target = root.transform;
        screen.GetComponent<UIWidget>().rightAnchor.target = root.transform;
        screen.GetComponent<UIWidget>().topAnchor.target = root.transform;
        screen.GetComponent<UIWidget>().bottomAnchor.target = root.transform;
    }
}
