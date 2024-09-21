using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using Unity.Netcode;

public class PauseMenuUIController : NetworkBehaviour
{
    private VisualElement root;
    private Dictionary<string, Button> buttons = new Dictionary<string, Button>();

    [SerializeField] private GameObject mainMenuPrefab;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        buttons["Resume"] = root.Q<Button>("Resume-Button");
        buttons["Settings"] = root.Q<Button>("Settings-Button");
        buttons["Quit-To-Menu-Button"] = root.Q<Button>("Quit-To-Menu-Button");
        buttons["Quit-To-Desktop-Button"] = root.Q<Button>("Quit-To-Desktop-Button");

        buttons["Resume"].RegisterCallback<ClickEvent>(evt => Resume());
        buttons["Settings"].RegisterCallback<ClickEvent>(evt => ShowSettings());
        buttons["Quit-To-Menu-Button"].RegisterCallback<ClickEvent>(evt => QuitToMenu());
        buttons["Quit-To-Desktop-Button"].RegisterCallback<ClickEvent>(evt => QuitToDesktop());
    }

    private void Resume()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovementNetwork>().TogglePause();

        Destroy(this.gameObject);
    }

    private void ShowSettings()
    {
        // Show settings menu
    }

    private void QuitToMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsClient)
            {
                Debug.Log("Stopping client...");
                NetworkManager.Singleton.Shutdown();
            }
            else if (NetworkManager.Singleton.IsServer)
            {
                Debug.Log("Stopping server...");
                NetworkManager.Singleton.Shutdown();
            }
        }
        else
        {
            Debug.LogWarning("NetworkManager is null. Cannot quit to menu.");
        }

        // Load the main menu
        Instantiate(mainMenuPrefab);
        Destroy(this.gameObject);
    }

    private void QuitToDesktop()
    {
        QuitToMenu(); // Call to handle network shutdown

        // Exit the application
        #if UNITY_EDITOR
        Debug.Log("Exiting play mode in the editor...");
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Debug.Log("Quitting application...");
        Application.Quit();
        #endif
    }

}
