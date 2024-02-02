using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        // Load JSON data from resources
        TextAsset surahJson = Resources.Load<TextAsset>("Data/surahs");
        TextAsset qariJson = Resources.Load<TextAsset>("Data/qaris");

        Surah[] surahs = JsonHelper.FromJson<Surah>(surahJson.text);
        Qari[] qaris = JsonHelper.FromJson<Qari>(qariJson.text);

        foreach (Surah surah in surahs)
        {
            Debug.Log(surah.number);
            Debug.Log(surah.name);
            Debug.Log(surah.juzz);
            Debug.Log(surah.origin);
            Debug.Log(surah.ayah);
        }

        foreach (Qari qari in qaris)
        {
            Debug.Log(qari.id);
            Debug.Log(qari.name);
        }
    }
}
