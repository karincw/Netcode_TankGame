using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Button Host;
    [SerializeField] private Button CLient;

    private void Awake()
    {
        Host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        CLient.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
