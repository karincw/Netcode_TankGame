using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardBehaviour : NetworkBehaviour
{
    [SerializeField] private RankData _rankDataPrefab;
    [SerializeField] private RectTransform _contentRect; //�������� �߰��� ��Ʈ

    private int _displayCount = 5; //4���� �����ְ� �ڱ��ڽ��� �����ִ�

    private NetworkList<LeaderBoardEntityState> _leaderBoards;
    private List<RankData> _rankDataUIList;

    private void Awake()
    {
        _leaderBoards = new NetworkList<LeaderBoardEntityState>();
        _rankDataUIList = new List<RankData>();
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            foreach(PlayerController p in players)
            {
                HandlePlayerSpawn(p);
            }

            PlayerController.OnPlayerSpawn += HandlePlayerSpawn;
            PlayerController.OnPlayerDespawn += HandlePlayerDeSpawn;
        }

        if(IsClient)
        {
            _leaderBoards.OnListChanged += HandleLeaderboardChanged;

            foreach(var entity in _leaderBoards)
            {
                HandleLeaderboardChanged(new NetworkListEvent<LeaderBoardEntityState>
                {
                    Type = NetworkListEvent<LeaderBoardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }
    }


    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            PlayerController.OnPlayerSpawn -= HandlePlayerSpawn;
            PlayerController.OnPlayerDespawn -= HandlePlayerDeSpawn;
        }
        if (IsClient)
        {
            _leaderBoards.OnListChanged -= HandleLeaderboardChanged;
        }
    }


    #region Only client execution area
    private void HandleLeaderboardChanged(NetworkListEvent<LeaderBoardEntityState> evt)
    {
        switch (evt.Type)
        {
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Add:
            
                if(CheckExistByClientID(evt.Value.clientID) == false)
                {
                    AddItem(evt.Value);
                }
                break;

            case NetworkListEvent<LeaderBoardEntityState>.EventType.Remove:
                RemoveItemByClientID(evt.Value.clientID);
                break;
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Value:
                UpdateItemByClientID(evt.Value.clientID, evt.Value.coins);
                break;
        }
        ReOrderRankDataUI();
    }

    public bool CheckExistByClientID(ulong clientID)
    {
        //���� �ְ� �ϳ��� ������ true�ΰ� any
        return _rankDataUIList.Any(x => x.ClientID == clientID); 
    }

    public void AddItem(LeaderBoardEntityState state)
    {
        RankData newItem = Instantiate(_rankDataPrefab, _contentRect);

        _rankDataUIList.Add(newItem);
        newItem.Initialize(state); //UI�ʱ�ȭ ���ְ�
        ReOrderRankDataUI(); //�����ϴ°�
    }

    private void ReOrderRankDataUI()
    {
        _rankDataUIList.Sort((a, b) => b.Coins.CompareTo(a.Coins));

        float spacing = 5f;
        float offset = spacing;

        int displayedCount = 0;

        for(int i = 0; i < _rankDataUIList.Count; i++)
        {
            var item = _rankDataUIList[i];
            item.rank = i + 1;
            item.UpdateText();

            if(displayedCount < _displayCount || 
                item.ClientID == NetworkManager.Singleton.LocalClientId)
            {
                item.Show(true);
                displayedCount++;
            }
            else
            {
                item.Show(false);
            }

            if (item.gameObject.activeSelf)
            {
                item.Rect.anchoredPosition = new Vector2(20f, -offset);
                offset += item.Rect.sizeDelta.y + spacing;
            }

            Vector2 contentSize = _contentRect.sizeDelta;
            contentSize.y = offset;
            _contentRect.sizeDelta = contentSize;
        }
    }

    public void RemoveItemByClientID(ulong clientID)
    {
        RankData item = _rankDataUIList.FirstOrDefault(x => x.ClientID == clientID);
        if(item != null)
        {
            _rankDataUIList.Remove(item);
            Destroy(item.gameObject);
            ReOrderRankDataUI(); //������
        }
    }

    public void UpdateItemByClientID(ulong clientID, int coins)
    {
        RankData item = _rankDataUIList.FirstOrDefault(x => x.ClientID == clientID);
        if(item != null)
        {
            item.UpdateCoin(coins);
        }
    }


    #endregion


    #region only server execution area!

    private void HandlePlayerSpawn(PlayerController player)
    {
        _leaderBoards.Add(new LeaderBoardEntityState
        {
            clientID = player.OwnerClientId,
            playerName = player.playerName.Value,
            coins = 0,
        });

        player.CoinCompo.totalCoin.OnValueChanged += (oldCoin, newCoin) =>
        {
            HandleCoinsChanged(player.OwnerClientId, newCoin);
        };
    }

    private void HandleCoinsChanged(ulong clientID, int newCoin)
    {
        for(int i = 0; i < _leaderBoards.Count; i++)
        {
            if (_leaderBoards[i].clientID != clientID) continue;

            var oldItem = _leaderBoards[i];
            
            _leaderBoards[i] = new LeaderBoardEntityState
            {
                clientID = oldItem.clientID,
                playerName = oldItem.playerName,
                coins = newCoin
            };
            break;
        }
    }

    private void HandlePlayerDeSpawn(PlayerController player)
    {
        if (_leaderBoards == null) return;
        foreach(var entity in _leaderBoards)
        {
            //���� ���� �Ǵ� �÷��̾ ã�ƾ� �Ѵ�
            if (entity.clientID != player.OwnerClientId) continue;

            try
            {
                _leaderBoards.Remove(entity);
            }
            catch(Exception e)
            {
                Debug.LogWarning(
                    $"{entity.playerName} - {entity.clientID} : delete exception {e.Message}");
            }
            break;
        }
    }

    #endregion
}
