using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    public DatabaseReference databaseReference;
    public bool isLoadOK = false;

    public int AllXpNum;
    public int xp;
    public int coin;
    public int diamond;
    public int level;
    

    [Header("Data UI")]
    public Text coin_T;
    public Text diamond_T;
    public Text level_T;
    public Slider xp_S;

    const int BASIC_LEVEL_UP_XP = 10;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Update()
    {
        if (isLoadOK)
        {
            ShowOnUI();
        }    
    }

    public void DisplayDataOnUI(int _coin, int _diamond, int _xp)
    {
        coin = _coin;
        diamond = _diamond;
        xp = _xp;

        isLoadOK = true;
    }

    public void ShowOnUI()
    {
        coin_T.text = coin.ToString();
        diamond_T.text = diamond.ToString();
        StartCoroutine(XpNumToUI(xp));
    }

    public void XpNumToUI2(int _xp) // Load
    {
        // xp => level and bar
        int calXp = 0;
        int levelMaxXp = 10;
        int nowLv = 1;
        bool isOn = true;

        while (isOn)
        {
            if(_xp-calXp < levelMaxXp)
            {
                isOn = false;
                break;
            }

            calXp += levelMaxXp;
            nowLv++;
            levelMaxXp = nowLv * BASIC_LEVEL_UP_XP;
        }

        // Display on UI
        float xpPercentage = (_xp - calXp) / levelMaxXp * 100;

        //xp = _xp - calXp;
        level_T.text = $"Lv.{nowLv}";
        xp_S.value = xpPercentage;
        print(xp_S.value);
    }
    public IEnumerator XpNumToUI(float _xp) // Load
    {
        // xp => level and bar
        float calXp = 0;
        float levelMaxXp = 10;
        int nowLv = 1;
        bool isOn = true;

        while (isOn)
        {
            if (_xp - calXp < levelMaxXp)
            {
                isOn = false;
                break;
            }

            calXp += levelMaxXp;
            nowLv++;
            levelMaxXp = nowLv * BASIC_LEVEL_UP_XP;
        }

        // Display on UI
        float xpPercentage = (_xp - calXp) / levelMaxXp;

        //xp = _xp - calXp;
        level_T.text = $"Lv.{nowLv}";
        xp_S.value = xpPercentage;
        yield return null;
    }

    public int XpUITONum(int _level, int _xp) // Save
    {
        // level and bar => xp
        int nowXp = 0;

        for (int i = 1; i <= _level; i++)
        {
            nowXp += i * 10;
        }

        nowXp += _xp;

        return nowXp;
    }

    public void addCoin()
    {
        coin++;
    }
    public void addDiamond()
    {
        diamond++;
    }

    public void addXp()
    {
        xp++;
    }
} 
