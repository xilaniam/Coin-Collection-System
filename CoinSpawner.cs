using DG.Tweening;
using DiamondCasino;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance { get; private set; }
    [SerializeField] GameObject coinPrefab; // Assign in Inspector
    [SerializeField] WinNumber winNumber;
    [SerializeField] Transform balancePos;  // Assign in Inspector
    [SerializeField] float scaleDuration = 0.5f;
    [SerializeField] int delayBeforeMove = 2; // Delay before the coin moves
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float offset;
    [SerializeField] TMP_Text balanceAmountText;
    float amountBal;
   // float winAMount;
    public float BALANCE { get => amountBal; }
    private void Awake()
    {
        CheckFirstGame();
        // Singleton implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure there's only one instance
            return;
        }
        Instance = this;
    }

    void CheckFirstGame()
    {
        if (!PlayerPrefs.HasKey("isFirstGame"))
        {
            PlayerPrefs.SetInt("isFirstGame", 1);
            amountBal = 50000;
            PlayerPrefs.SetFloat("balance", amountBal);
            PlayerPrefs.Save();
        }
        else
        {
            amountBal = PlayerPrefs.GetFloat("balance");
        }
    }

    private void Start()
    {
        moveSpeed *= 10;
        balanceAmountText.text = amountBal.ToString();
        if (coinPrefab == null || balancePos == null)
        {
            Debug.LogError("CoinPrefab or BalancePos is not assigned in the Inspector.");
        }
    }

/*    private void Update()
    {
        WebSocketBalance.Instance?.SendBalance(amount);
    }*/

    public void SpawnCoin(Vector3 pos , float amount)
    {
        if (coinPrefab == null || balancePos == null)
        {
            Debug.LogError("CoinPrefab or BalancePos is not assigned. Cannot spawn coin.");
            return;
        }

        // Instantiate the coin and set its initial scale
        var coin = Instantiate(coinPrefab, pos, Quaternion.identity);
        SpawnWinAmount(pos , amount);
        coin.transform.localScale = Vector3.zero;

        // Create a DOTween Sequence for animation
        Sequence coinSequence = DOTween.Sequence();
        Vector3 coinPos = balancePos.position - new Vector3(offset, 0, 0);


        float distance = Vector3.Distance(coin.transform.position, coinPos);

        float moveDuration = distance/moveSpeed;
        // Scale up the coin
        coinSequence.Append(coin.transform.DOScale(0.4f, scaleDuration).SetEase(Ease.OutSine));

        coinSequence.Append(coin.transform.DOMoveY(coin.transform.position.y+.08f, .5f).SetEase(Ease.InOutSine).SetLoops(delayBeforeMove,LoopType.Yoyo));

        // Add a delay before moving
        //coinSequence.AppendInterval(delayBeforeMove);

        // Move the coin to the balance position
        coinSequence.Append(coin.transform.DOMove(coinPos, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            amountBal += (amount*50);
            SaveBalancePrefs(amountBal);
            UpdateBalanceUI();
        }));

        coinSequence.Append(coin.transform.DOScale(0, scaleDuration).SetEase(Ease.OutSine));

        // Destroy the coin after it reaches the target
        coinSequence.OnComplete(() =>
        {
            Destroy(coin);
            
        });
    }

    public void SpawnCoin(Vector3 pos , Transform targetPos , float amount)
    {
        if (coinPrefab == null || balancePos == null)
        {
            Debug.LogError("CoinPrefab or BalancePos is not assigned. Cannot spawn coin.");
            return;
        }

        // Instantiate the coin and set its initial scale
        var coin = Instantiate(coinPrefab, pos, Quaternion.identity);
        SpawnWinAmount(pos ,amount);
        coin.transform.localScale = Vector3.zero;

        // Create a DOTween Sequence for animation
        Sequence coinSequence = DOTween.Sequence();
        Vector3 coinPos = targetPos.position - new Vector3(offset, 0, 0);


        float distance = Vector3.Distance(coin.transform.position, coinPos);

        float moveDuration = distance / moveSpeed;
        // Scale up the coin
        coinSequence.Append(coin.transform.DOScale(0.4f, scaleDuration).SetEase(Ease.OutSine));

        coinSequence.Append(coin.transform.DOMoveY(coin.transform.position.y + .08f, .5f).SetEase(Ease.InOutSine).SetLoops(delayBeforeMove, LoopType.Yoyo));

        // Add a delay before moving
        //coinSequence.AppendInterval(delayBeforeMove);

        // Move the coin to the balance position
        coinSequence.Append(coin.transform.DOMove(coinPos, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            UpdateTotalWin(amount, targetPos);
        }));

        coinSequence.Append(coin.transform.DOScale(0, scaleDuration).SetEase(Ease.OutSine));

        // Destroy the coin after it reaches the target
        coinSequence.OnComplete(() =>
        {
            Destroy(coin);

        });
    }

    public void SpawnWinAmount(Vector3 pos , float amount)
    {
        WinNumber number = Instantiate(winNumber, pos, Quaternion.identity , transform);
        number.SetWinAmount(amount * 50);
        //winAMount = number.GetWinAmount();
        number.transform.localScale = Vector3.zero;
        number.transform.DOScale(1, 0.5f);
        number.transform.DOShakePosition(2, 0.3f, 3);
        //number.DOLocalMoveY(number.position.y + 1, 0.5f).OnComplete(() => { number.DOShakePosition(2, 0.3f , 3); });
        
        this.Wait(2, () =>
        {
        number.transform.DOScale(0, 0.5f).OnComplete(() =>{ Destroy(number.gameObject); });
            
        });
    }

    public void SetBalance(float amount)
    {
        amountBal = amount;
        SaveBalancePrefs(amountBal);
    }

    public void UpdateBalanceUI()
    {
        balanceAmountText.text = amountBal.ToString();
        balancePos.DOScale(1.2f, 0.1f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            balancePos.DOScale(1, 0.15f).SetEase(Ease.OutSine);
        });
    }

    public void UpdateTotalWin(float amount , Transform pos)
    {
        pos.GetComponent<EntryManager>().IncreaseAmountForBot(amount);
        pos.DOScale(1.2f, 0.1f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            pos.DOScale(1, 0.15f).SetEase(Ease.OutSine);
        });
    }

    void SaveBalancePrefs(float amount)
    {
        PlayerPrefs.SetFloat("balance",amount);
        PlayerPrefs.Save();
    }
}
