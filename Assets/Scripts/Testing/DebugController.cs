using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;

public class DebugController : MonoBehaviour
{
    [SerializeField] private GameObject networkDebugUI;

    [Command("network-debug")]
    private void ToggleNetworkDebug(bool enable)
    {
        networkDebugUI.SetActive(enable);
    }
}
