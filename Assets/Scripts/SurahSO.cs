using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Surah", menuName = "Surah")]
public class Surah : ScriptableObject
{

    [SerializeField] string Number;
    [SerializeField] string Name;
    [SerializeField] string Juz;
    [SerializeField] string Type;

}
