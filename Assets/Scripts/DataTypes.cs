using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Surah
{
    public int number;
    public string name;
    public int juzz;
    public int revelation;
    public int ayat;
}

[Serializable]
public class Qari
{
    public int id;
    public string fullName;
    public string name;
}
