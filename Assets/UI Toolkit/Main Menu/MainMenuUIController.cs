using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class MainMenuUIController : MonoBehaviour
{
    private VisualElement root;

    private Dictionary<string, VisualElement> menus = new Dictionary<string, VisualElement>();
    private Button playButton;
    private Button settingsButton;
    private Button quitButton;

    private Dictionary<string, Button> playButtons = new Dictionary<string, Button>();

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // Get buttons from the UI
        playButton = root.Q<Button>("Play");
        settingsButton = root.Q<Button>("Settings");
        quitButton = root.Q<Button>("Quit");

        // Register button click events
        playButton.RegisterCallback<ClickEvent>(evt => ShowMenu("Play-Menu"));
        settingsButton.RegisterCallback<ClickEvent>(evt => ShowMenu("Settings-Menu"));
        quitButton.RegisterCallback<ClickEvent>(evt => Quit());

        // Get visual elements for different menus and store them in the dictionary
        menus["Main-Menu"] = root.Q<VisualElement>("Main-Menu");
        menus["Play-Menu"] = root.Q<VisualElement>("Play-Menu");
        menus["Settings-Menu"] = root.Q<VisualElement>("Settings-Menu");

        // Initialize the menus
        foreach (var menu in menus.Values)
        {
            menu.style.display = DisplayStyle.None; // Hide all menus initially
        }
        menus["Main-Menu"].style.display = DisplayStyle.Flex; // Show main menu

        // Register hover events for each play option
        RegisterHoverEvents("Split-Screen");
        RegisterHoverEvents("LAN");
        RegisterHoverEvents("Online");

        playButtons["Split-Screen-Start"] = root.Q<Button>("Split-Screen-Start");
        playButtons["LAN-Host"] = root.Q<Button>("LAN-Host");
        playButtons["LAN-Join"] = root.Q<Button>("LAN-Join");
        playButtons["Online-Host"] = root.Q<Button>("Online-Host");
        playButtons["Online-Join"] = root.Q<Button>("Online-Join");

        foreach (var button in playButtons.Values)
        {
            button.RegisterCallback<ClickEvent>(evt => ButtonClicked(button.name));
        }

        // Back buttons
        root.Q<Button>("Play-Back-Button").RegisterCallback<ClickEvent>(evt => ShowMenu("Main-Menu"));
        root.Q<Button>("Settings-Back-Button").RegisterCallback<ClickEvent>(evt => ShowMenu("Main-Menu"));
    }

    private void ShowMenu(string menuName)
    {
        foreach (var menu in menus.Values)
        {
            menu.style.display = DisplayStyle.None; // Hide all menus
        }
        if (menus.TryGetValue(menuName, out var menuToShow))
        {
            menuToShow.style.display = DisplayStyle.Flex; // Show the selected menu
        }
    }

    private void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void RegisterHoverEvents(string optionName)
    {
        var optionElement = root.Q<VisualElement>(optionName + "-Panel");
        if (optionElement != null)
        {
            optionElement.RegisterCallback<PointerEnterEvent>(evt => ToggleButtons(optionName, true));
            optionElement.RegisterCallback<PointerLeaveEvent>(evt => ToggleButtons(optionName, false));
        }
    }

    private void ToggleButtons(string buttonName, bool show)
    {
        var targetElement = root.Q<VisualElement>(buttonName + "-Buttons");
        if (targetElement != null)
        {
            targetElement.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void ButtonClicked(string buttonName)
    {
        Debug.Log("Button clicked: " + buttonName);

        switch (buttonName)
        {
            case "Split-Screen-Start":
                // Start split-screen game
                break;
            case "LAN-Host":
                NetworkManager.Singleton.StartHost();
                root.style.display = DisplayStyle.None;
                break;
            case "LAN-Join":
                ChangeTargetIpAddress("127.0.0.1"); //not true lan, needs updating
                NetworkManager.Singleton.StartClient();
                root.style.display = DisplayStyle.None;
                break;
            case "Online-Host":
                NetworkManager.Singleton.StartHost();
                break;
            case "Online-Join":
                //display join game menu
                //enter ip address
                break;
            default:
                break;
        }
    }

    private void ChangeTargetIpAddress(string newIPAddress)
    {
        var networkManager = NetworkManager.Singleton;
        var transport = networkManager.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = newIPAddress;
            Debug.Log($"Transport IP address changed to: {newIPAddress}");
        }
        else
        {
            Debug.LogError("UnityTransport component not found on NetworkManager.");
        }
    }
}
