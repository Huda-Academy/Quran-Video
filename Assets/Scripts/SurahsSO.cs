using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Surahs", menuName = "Surahs List")]
public class SurahsSO : ScriptableObject
{
    [SerializeField] List<Surah> Surahs = new List<Surah>();
}
