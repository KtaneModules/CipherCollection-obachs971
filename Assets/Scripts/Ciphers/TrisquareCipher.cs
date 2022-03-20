﻿using CipherMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Words;

public class TrisquareCipher
{
    public ResultInfo encrypt(string word, string id, string log, KMBombInfo Bomb)
    {
        Debug.LogFormat("{0} Being Trisquare Cipher", log);
        Data data = new Data();
        string encrypt = "";
        string replaceJ = "";
        string alpha = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
        Debug.LogFormat("{0} [Trisquare Cipher] Before Replacing Js: {1}", log, word);
        for (int i = 0; i < word.Length; i++)
        {
            if (word[i] == 'J')
            {
                word = word.Substring(0, i) + "" + alpha[Random.Range(0, alpha.Length)] + "" + word.Substring(i + 1);
                replaceJ = replaceJ + "" + word[i];
            }
            else
                replaceJ = replaceJ + "" + alpha.Replace(word[i].ToString(), "")[Random.Range(0, 24)];
        }
        Debug.LogFormat("{0} [Trisquare Cipher] After Replacing Js: {1}", log, word);
        Debug.LogFormat("{0} [Trisquare Cipher] Screen 4: {1}", log, replaceJ);
        string[] kws = new string[3];
        string[] keys = new string[3];
        string[][] kwFronts = new string[3][];
        for (int i = 0; i < 3; i++)
        {
            kws[i] = data.PickWord(4, 8);
            kwFronts[i] = CMTools.generateBoolExp(Bomb);
            keys[i] = CMTools.getKey(kws[i].Replace("J", "I"), "ABCDEFGHIKLMNOPQRSTUVWXYZ", kwFronts[i][1][0] == 'T');
            Debug.LogFormat("{0} [Trisquare Cipher] Keyword #{1}: {2}", log, (i + 1), kws[i]);
            Debug.LogFormat("{0} [Trisquare Cipher] Key #{1}: {2} -> {3} -> {4}", log, (i + 1), kwFronts[i][0], kwFronts[i][1], keys[i]);
        }
        string intersection = "";
        for (int i = 0; i < (word.Length / 2); i++)
        {
            int r1 = keys[0].IndexOf(word[i * 2]) / 5;
            int c1 = keys[0].IndexOf(word[i * 2]) % 5;
            int r2 = keys[1].IndexOf(word[(i * 2) + 1]) / 5;
            int c2 = keys[1].IndexOf(word[(i * 2) + 1]) % 5;
            intersection = intersection + "" + keys[2][(r1 * 5) + c2];
            r1 = (r1 + Random.Range(0, 4) + 1) % 5;
            c2 = (c2 + Random.Range(0, 4) + 1) % 5;
            encrypt = encrypt + "" + keys[0][(r1 * 5) + c1] + "" + keys[1][(r2 * 5) + c2];
            Debug.LogFormat("{0} [Trisquare Cipher] {1}{2} -> {3}{4}{5}", log, word[i * 2], word[(i * 2) + 1], encrypt[i * 2], encrypt[(i * 2) + 1], intersection[i]);
        }
        if (word.Length % 2 == 1)
            encrypt = encrypt + "" + word[word.Length - 1];
        Debug.LogFormat("{0} [Trisquare Cipher] {1} -> {2}", log, word, encrypt);
        Debug.LogFormat("{0} [Trisquare Cipher] Screen D: {1}", log, intersection);
        ScreenInfo[] screens = new ScreenInfo[9];
        screens[0] = new ScreenInfo(kws[0], new int[] { 35, 35, 35, 32, 28 }[kws[0].Length - 4]);
        screens[1] = new ScreenInfo(kwFronts[0][0], 25);
        screens[2] = new ScreenInfo(kws[1], new int[] { 35, 35, 35, 32, 28 }[kws[1].Length - 4]);
        screens[3] = new ScreenInfo(kwFronts[1][0], 25);
        screens[4] = new ScreenInfo(kws[2], new int[] { 35, 35, 35, 32, 28 }[kws[2].Length - 4]);
        screens[5] = new ScreenInfo(kwFronts[2][0], 25);
        screens[6] = new ScreenInfo(replaceJ, new int[] { 35, 35, 35, 32, 28 }[replaceJ.Length - 4]);
        screens[7] = new ScreenInfo(intersection, new int[] { 25, 25, 20 }[intersection.Length - 2]);
        screens[8] = new ScreenInfo(id, 35);
        return new ResultInfo
        {
            Encrypted = encrypt,
            Score = 5,
            Pages = new PageInfo[] { new PageInfo(screens) }
        };
    }
}
