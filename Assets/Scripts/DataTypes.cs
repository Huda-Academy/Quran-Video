using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Surah
{
    private int number;
    private string name;
    private int juzz;
    private int origin;
    private int ayah;

    public int Number { get => number; set => number = value; }
    public string Name { get => name; set => name = value; }
    public int Juzz { get => juzz; set => juzz = value; }
    public int Origin { get => origin; set => origin = value; }
    public int Ayah { get => ayah; set => ayah = value; }

    public Surah(int number, string name, int juzz, int origin, int ayah)
    {
        this.number = number;
        this.name = name;
        this.juzz = juzz;
        this.origin = origin;
        this.ayah = ayah;
    }

}

public enum SurahOrigin
{
    Meccan = 1,
    Medinan = 2
}
