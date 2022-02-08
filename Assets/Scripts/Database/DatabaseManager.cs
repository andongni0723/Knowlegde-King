using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    public DatabaseReference databaseReference;

    public int AllXpNum;
    

    [Header("Data UI")]
    public Text coin_T;
    public Text diamond_T;
    public Text xp_T;
    public Slider xp_S;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void DisplayDataOnUI(int _coin, int _diamond,int _level, int _xp)
    {
        coin_T.text = _coin.ToString();
        diamond_T.text = _diamond.ToString();
        XpNumToUI(_level,_xp);
    } 

    public void XpNumToUI(int level,int _xp)
    {

    }

    public int XpUITONum(int _xp)
    {
        int result = _xp;

        return result;
    }
} 
