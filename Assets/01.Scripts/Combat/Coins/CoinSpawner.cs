using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private ReSpawnCoin _coinPrefab;
    [SerializeField] private DecalCircle _decalCircle;

    [Header("Setting values")]
    [SerializeField] private int _maxCoins = 30; //�ִ� 30���� ���� ����
    [SerializeField] private int _coinValue = 10; //���δ� 10
    [SerializeField] private LayerMask _layerMask; //���� �����ϴ� ������ ��ֹ��� �ִ��� �˻�
    [SerializeField] private float _spawnTerm = 30f;
    //[SerializeField] private float _spawnRadius = 8f;
    [SerializeField] private List<SpawnPoint> spawnPointList;

    private bool _isSpawning = false;
    private float _spawnTime = 0;
    private int _spawnCountTime = 5; //5�� ī��Ʈ�ٿ� �ϰ� ����

    private float _coinRadius;

    private Stack<ReSpawnCoin> _coinPool = new Stack<ReSpawnCoin>(); //���� Ǯ
    private List<ReSpawnCoin> _activeCoinList = new List<ReSpawnCoin>(); //Ȱ��ȭ�� ����

    //�̳༮�� ������ �����ϴ� �ڵ��.
    private ReSpawnCoin SpawnCoin()
    {
        if (IsServer == false) return null;

        ReSpawnCoin coin = Instantiate(_coinPrefab, Vector3.zero, Quaternion.identity);
        coin.SetValue(_coinValue);
        coin.GetComponent<NetworkObject>().Spawn(); //��Ʈ��ũ�� ���ؼ� �̳༮�� �� �����Ѵ�.

        coin.OnCollected += HandleCoinCollected;

        return coin;
    }


    //�̰͵� ������ �Ҳ���.
    private void HandleCoinCollected(ReSpawnCoin coin)
    {
        if (IsServer == false) return;

        _activeCoinList.Remove(coin);
        coin.SetVisible(false);
        _coinPool.Push(coin);
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer == false)
        {
            return;
        }

        //�̰ɷ� ���� ũ�⸦ ���.
        _coinRadius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

        for(int i = 0; i < _maxCoins; ++i)
        {
            ReSpawnCoin coin = SpawnCoin();
            coin.SetVisible(false); //ó�� ������ �ֵ��� ���ش�.
            _coinPool.Push(coin);
        }
    }

    public override void OnNetworkDespawn()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (IsServer == false) return;

        //���߿� ���⿡ ������ ���۵Ǿ��� ���� ������ �����ǰ� �����ؾ� ��.
        if (GameManager.Instance.GameStarted == false) return;


        if(_isSpawning == false && _activeCoinList.Count == 0)
        {
            _spawnTime += Time.deltaTime;
            if(_spawnTime >= _spawnTerm)
            {
                _spawnTime = 0;
                StartCoroutine(SpawnCoroutine());
            }
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        _isSpawning = true;
        int pointIndex = Random.Range(0, spawnPointList.Count);
        SpawnPoint point = spawnPointList[pointIndex];
        int maxCoinCnt = Mathf.Min(_maxCoins, point.SpawnPoints.Count);
        int coinCount = Random.Range(maxCoinCnt / 2, maxCoinCnt + 1);

        for(int i = _spawnCountTime; i > 0; --i)
        {
            CountDownClientRpc(i, pointIndex, coinCount);
            yield return new WaitForSeconds(1f);
        }

        //�̺κ��� ���߿� ������ �Ҳ���.
        float coinDelay = 2f;
        List<Vector3> points = point.SpawnPoints;

        for (int i = 0; i < coinCount; ++i)
        {
            int end = points.Count - i - 1;
            int idx = Random.Range(0, end + 1);
            Vector3 pos = points[idx];

            (points[idx], points[end]) = (points[end], points[idx]);

            var coin = _coinPool.Pop();
            coin.transform.position = pos;
            coin.ResetCoin();
            _activeCoinList.Add(coin);

            yield return new WaitForSeconds(coinDelay);
        }

        _isSpawning = false;
        DecalCircleCloseClientRpc();
    }

    [ClientRpc]
    private void CountDownClientRpc(int sec, int pointIndex, int coinCount)
    {
        SpawnPoint point = spawnPointList[pointIndex];
        if(_decalCircle.showDecal == false)
        {
            _decalCircle.OpenCircle(point.Position, point.Radius);
        }

        if(sec <= 1)
        {
            _decalCircle.StopBlinkIcon();
        }
        MessageSystem.Instance.ShowText($"{point.pointName} : After {sec},  {coinCount} Coin will be generate", 0.8f);
    }

    [ClientRpc]
    private void DecalCircleCloseClientRpc()
    {
        _decalCircle.CloseCircle();
    }
}
