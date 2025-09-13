using Cinemachine;
using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    public static event Action<PlayerController> OnPlayerSpawn;
    public static event Action<PlayerController> OnPlayerDespawn;


    [Header("Reference")]
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private SpriteRenderer _minimapIcon;

    [Header("Setting Values")]
    [SerializeField] private int _ownerCamPriority = 15;
    [SerializeField] private Color _ownerColor;

    //��ũ �ε���
    public NetworkVariable<Color> tankColor;

    public PlayerVisual VisualCompo { get; private set; }
    public PlayerMovement MovementCompo { get; private set; }
    public Health HealthCompo { get; private set; }
    public CoinCollector CoinCompo { get; private set; }
    public ProjectileLauncher LauncherCompo { get; private set; }

    public NetworkVariable<FixedString32Bytes> playerName;

    public ShopNPC shopNpc;
    private PlayerInput _playerInput;

    private void Awake()
    {
        tankColor = new NetworkVariable<Color>();
        playerName = new NetworkVariable<FixedString32Bytes>();

        VisualCompo = GetComponent<PlayerVisual>();
        MovementCompo = GetComponent<PlayerMovement>();
        HealthCompo = GetComponent<Health>();

        CoinCompo = GetComponent<CoinCollector>();
        _playerInput = GetComponent<PlayerInput>();

        LauncherCompo = GetComponent<ProjectileLauncher>();
    }

    public override void OnNetworkSpawn()
    {
        tankColor.OnValueChanged += HandleColorChanged;
        playerName.OnValueChanged += HandleNameChanged;
        if (IsOwner)
        {
            _followCam.Priority = _ownerCamPriority;
            _minimapIcon.color = _ownerColor;
            _playerInput.OnShopKeyEvent += HandleShopKeyEvent;
        }

        if (IsServer)
        {
            UserData data = HostSingleton.Instance.GameManager
                                .NetServer.GetUserDataByClientID(OwnerClientId);
            playerName.Value = data.username;
        }
        HandleNameChanged(string.Empty, playerName.Value); //���� �ε��ÿ��� ����ȭ�ϰ�
        OnPlayerSpawn?.Invoke(this); //������ �̺�Ʈ�� �����Ѵ�.
    }

    public override void OnNetworkDespawn()
    {
        tankColor.OnValueChanged -= HandleColorChanged;
        playerName.OnValueChanged -= HandleNameChanged;

        if(IsOwner)
        {
            _playerInput.OnShopKeyEvent -= HandleShopKeyEvent;
        }

        OnPlayerDespawn?.Invoke(this);
    }

    private void HandleShopKeyEvent()
    {
        if (shopNpc == null) return; //���Կ� ���� ���� �����Ѵ�.

        shopNpc.OpenShop(this);
    }

    private void HandleNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        _nameText.text = newValue.ToString();
    }

    private void HandleColorChanged(Color previousValue, Color newValue)
    {
        VisualCompo.SetTintColor(newValue);
    }

    #region Only Server execution area
    //���߿� SetTankData�� �����Ҳ���.
    public void SetTankData(Color color, int coin)
    {
        tankColor.Value = color;
        CoinCompo.totalCoin.Value = coin;
    }

    #endregion

    [ClientRpc]
    public void AddDamageToLauncherClientRpc(int upgradeValue)
    {
        LauncherCompo.damage += upgradeValue;
    }

    [ClientRpc]
    public void AddHPClientRpc(int increaseValue)
    {
        HealthCompo.maxHealth += increaseValue;
    }
}
