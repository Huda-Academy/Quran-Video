using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Surah
{
    public int number;
    public string name;
    public int juzz;
    public int origin;
    public int ayah;
}

[Serializable]
public class Qari
{
    public int id;
    public string name;
}
