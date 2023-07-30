using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class RandomTextGenerator : MonoBehaviour
{
    public TextMesh textMesh;
    public int length = 10;
    private static System.Random random = new System.Random();

    private void FixedUpdate()
    {
        textMesh.text = RandomString(length);
    }
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
