﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CipherMachine;
using UnityEngine;
using Words;

public class cipherMachine : MonoBehaviour
{
    private static readonly CipherBase[][] _allCiphers = CMTools.NewArray(
        new CipherBase[] { new AffineCipher(invert: false), new AffineCipher(invert: true) },
        new CipherBase[] { new AMSCOTransposition(invert: false), new AMSCOTransposition(invert: true) },
        new CipherBase[] { new AtbashCipher() },
        new CipherBase[] { new AutokeyCipher(invert: false), new AutokeyCipher(invert: true) },
        new CipherBase[] { new BazeriesCipher(invert: false), new BazeriesCipher(invert: true) },
        new CipherBase[] { new BitSwitchCipher(invert: false), new BitSwitchCipher(invert: true) },
        new CipherBase[] { new BookCipher() },
        new CipherBase[] { new CaesarCipher(invert: false), new CaesarCipher(invert: true) },
        new CipherBase[] { new CaesarShuffleCipher(invert: false), new CaesarShuffleCipher(invert: true) },
        new CipherBase[] { new ChainBitRotationCipher(invert: false), new ChainBitRotationCipher(invert: true) },
        new CipherBase[] { new Chaocipher(invert: false), new Chaocipher(invert: true) },
        new CipherBase[] { new CollonCipher() },
        new CipherBase[] { new ColumnarTransposition(invert: false), new ColumnarTransposition(invert: true) },
        new CipherBase[] { new CondiCipher(invert: false), new CondiCipher(invert: true) },
        new CipherBase[] { new ConjugatedMatrixBifidCipher(invert: false), new ConjugatedMatrixBifidCipher(invert: true) },
        new CipherBase[] { new DigrafidCipher() },
        new CipherBase[] { new DualTriplexReflectorCipher(invert: false), new DualTriplexReflectorCipher(invert: true) },
        new CipherBase[] { new EnigmaCipher() },
        new CipherBase[] { new FoursquareCipher(invert: false), new FoursquareCipher(invert: true) },
        new CipherBase[] { new FractionatedMorseCipher() },
        new CipherBase[] { new GrandpreCipher() },
        new CipherBase[] { new GrilleTransposition(invert: false), new GrilleTransposition(invert: true) },
        new CipherBase[] { new GROMARKCipher(invert: false), new GROMARKCipher(invert: true) },
        new CipherBase[] { new HillCipher(invert: false), new HillCipher(invert: true) },
        new CipherBase[] { new HomophonicCipher() },
        new CipherBase[] { new LogicCipher() },
        new CipherBase[] { new M209Cipher() },
        new CipherBase[] { new MechanicalCipher(invert: false), new MechanicalCipher(invert: true) },
        new CipherBase[] { new MonoalphabeticCipher(invert: false), new MonoalphabeticCipher(invert: true) },
        new CipherBase[] { new MorbitCipher() },
        new CipherBase[] { new MyszkowskiTransposition(invert: false), new MyszkowskiTransposition(invert: true) },
        new CipherBase[] { new PlayfairCipher(invert: false), new PlayfairCipher(invert: true) },
        new CipherBase[] { new PortaCipher() },
        new CipherBase[] { new PortaxCipher() },
        new CipherBase[] { new PrissyCipher(invert: false), new PrissyCipher(invert: true) },
        new CipherBase[] { new QuagmireCipher(invert: false), new QuagmireCipher(invert: true) },
        new CipherBase[] { new RagbabyCipher(invert: false), new RagbabyCipher(invert: true) },
        new CipherBase[] { new RedefenceTransposition(invert: false), new RedefenceTransposition(invert: true) },
        new CipherBase[] { new RouteTransposition(invert: false), new RouteTransposition(invert: true) },
        new CipherBase[] { new RSACipher() },
        new CipherBase[] { new ScytaleTransposition(invert: false), new ScytaleTransposition(invert: true) },
        new CipherBase[] { new SeanCipher() },
        new CipherBase[] { new SemaphoreRotationCipher(invert: false), new SemaphoreRotationCipher(invert: true) },
        new CipherBase[] { new SolitaireCipher(invert: false), new SolitaireCipher(invert: true) },
        new CipherBase[] { new StripCipher(invert: false), new StripCipher(invert: true) },
        new CipherBase[] { new TransposedHalvedPolybiusCipher(invert: false), new TransposedHalvedPolybiusCipher(invert: true) },
        new CipherBase[] { new TridigitalCipher() },
        new CipherBase[] { new TrifidCipher(invert: false), new TrifidCipher(invert: true) },
        new CipherBase[] { new TrisquareCipher() },
        new CipherBase[] { new VICPhoneCipher() },
        new CipherBase[] { new VigenereCipher(invert: false), new VigenereCipher(invert: true) });

    public TextMesh[] screenTexts;
    public MeshRenderer[] screenTextMeshes;
    public MeshRenderer submitMesh;
    public Material[] materials;
    public KMBombInfo Bomb;
    public KMBombModule module;
    public AudioClip[] sounds;
    public KMAudio Audio;
    public TextMesh submitText;
    public KMSelectable leftArrow;
    public KMSelectable rightArrow;
    public KMSelectable submit;
    public KMSelectable[] keyboard;
    public Font DEFAULT_FONT;
    public Material DEFAULT_FONT_MAT;

    private PageInfo[] pages;
    private string answer;
    private int page = 0;
    private bool submitScreen;
    private static int moduleIdCounter = 1;
    private int moduleId;
    private bool moduleSolved;
    private bool moduleSelected;
    private readonly Color[] textColors = { Color.white, Color.black };

    void Awake()
    {
        if (Application.isEditor)
            for (var i = 0; i < _allCiphers.Length; i++)
            {
                for (var j = i + 1; j < _allCiphers.Length; j++)
                    if (_allCiphers[i][0].Code == _allCiphers[j][0].Code)
                        Debug.LogErrorFormat(@"{0} and {1} use the same code ({2}).", _allCiphers[i][0].Name, _allCiphers[j][0].Name, _allCiphers[i][0].Code);
                for (var j = 0; j < _allCiphers[i].Length; j++)
                    for (var k = j + 1; k < _allCiphers[i].Length; k++)
                        if (_allCiphers[i][j].Name == _allCiphers[i][k].Name)
                            Debug.LogErrorFormat(@"{0} and {1} use the same name.", _allCiphers[i][j].Name, _allCiphers[i][k].Name);
            }

        moduleId = moduleIdCounter++;
        leftArrow.OnInteract += delegate () { left(leftArrow); return false; };
        rightArrow.OnInteract += delegate () { right(rightArrow); return false; };
        submit.OnInteract += delegate () { submitWord(submit); return false; };
        module.GetComponent<KMSelectable>().OnFocus += delegate { moduleSelected = true; };
        module.GetComponent<KMSelectable>().OnDefocus += delegate { moduleSelected = false; };
        foreach (KMSelectable keybutton in keyboard)
        {
            KMSelectable pressedButton = keybutton;
            pressedButton.OnInteract += delegate () { letterPress(pressedButton); return false; };
        }
    }

    void Start()
    {
        // Generate random word
        var word = answer = new Data().PickWord(4, 8);
        Debug.LogFormat("[Cipher Machine #{0}] Solution: {1}", moduleId, answer);
        var pagesList = new List<PageInfo>();
        var cipherIxs = Enumerable.Range(0, _allCiphers.Length).ToArray().Shuffle();
        for (var i = 0; i < 3; i++)
        {
            var cipher = _allCiphers[cipherIxs[i]].PickRandom();
            Debug.LogFormat("[Cipher Machine #{0}] Encrypting {1} with {2} ({3})", moduleId, word, cipher.Name, cipher.Code);
            var result = cipher.Encrypt(answer, Bomb);
            foreach (var msg in result.LogMessages)
                Debug.LogFormat("[Cipher Machine #{0}] [{1}] {2}", moduleId, cipher.Name, msg);
            Debug.LogFormat("[Cipher Machine #{0}] Result: {1}", moduleId, result.Encrypted);
            word = result.Encrypted;
            foreach (var p in result.Pages)
                p.Code = cipher.Code;
            pagesList.InsertRange(0, result.Pages);
        }
        pagesList.Insert(0, new PageInfo(new ScreenInfo[] { new ScreenInfo(word, new int[] { 35, 35, 35, 32, 28 }[answer.Length - 4]) }));
        pages = pagesList.ToArray();
        getScreens();
    }

    string getKey(string k, string alpha, bool start)
    {
        for (int aa = 0; aa < k.Length; aa++)
        {
            for (int bb = aa + 1; bb < k.Length; bb++)
            {
                if (k[aa] == k[bb])
                {
                    k = k.Substring(0, bb) + "" + k.Substring(bb + 1);
                    bb--;
                }
            }
            alpha = alpha.Replace(k[aa].ToString(), "");
        }
        if (start)
            return (k + "" + alpha);
        else
            return (alpha + "" + k);
    }
    int correction(int p, int max)
    {
        while (p < 0)
            p += max;
        while (p >= max)
            p -= max;
        return p;
    }
    void left(KMSelectable arrow)
    {
        if (!moduleSolved)
        {
            Audio.PlaySoundAtTransform(sounds[0].name, transform);
            submitScreen = false;
            arrow.AddInteractionPunch();
            page--;
            page = correction(page, pages.Length);
            getScreens();
        }
    }
    void right(KMSelectable arrow)
    {
        if (!moduleSolved)
        {
            Audio.PlaySoundAtTransform(sounds[0].name, transform);
            submitScreen = false;
            arrow.AddInteractionPunch();
            page++;
            page = correction(page, pages.Length);
            getScreens();
        }
    }
    private void getScreens()
    {
        for (int aa = 0; aa < 8; aa++)
        {
            if (aa >= pages[page].Screens.Length)
            {
                screenTexts[aa].text = "";
                screenTexts[aa].font = DEFAULT_FONT;
                screenTextMeshes[aa].material = DEFAULT_FONT_MAT;
            }
            else
            {
                screenTexts[aa].text = pages[page].Screens[aa].Text;
                screenTexts[aa].fontSize = pages[page].Screens[aa].FontSize;
                if (pages[page].Screens[aa].TextFont == null)
                {
                    screenTexts[aa].font = DEFAULT_FONT;
                    screenTextMeshes[aa].material = DEFAULT_FONT_MAT;
                }
                else
                {
                    screenTexts[aa].font = pages[page].Screens[aa].TextFont;
                    screenTextMeshes[aa].material = pages[page].Screens[aa].FontMaterial;
                }
            }
        }
        submitMesh.material = materials[pages[page].Invert ? 1 : 0];
        submitText.color = textColors[pages[page].Invert ? 1 : 0];
        submitText.text = (page + 1) + pages[page].Code;
    }
    void submitWord(KMSelectable submitButton)
    {
        if (!moduleSolved)
        {
            submitButton.AddInteractionPunch();
            if (submitScreen)
            {
                if (screenTexts[6].text.Equals(answer))
                {
                    Audio.PlaySoundAtTransform(sounds[2].name, transform);
                    module.HandlePass();
                    moduleSolved = true;
                    screenTexts[6].text = "";
                }
                else
                {
                    Audio.PlaySoundAtTransform(sounds[3].name, transform);
                    module.HandleStrike();
                    page = 0;
                    getScreens();
                    submitScreen = false;
                }
            }
        }
    }
    void letterPress(KMSelectable pressed)
    {
        if (!moduleSolved)
        {
            pressed.AddInteractionPunch();
            Audio.PlaySoundAtTransform(sounds[1].name, transform);
            if (submitScreen)
            {
                if (screenTexts[6].text.Length < answer.Length)
                    screenTexts[6].text = screenTexts[6].text + "" + pressed.GetComponentInChildren<TextMesh>().text;
            }
            else
            {
                submitText.text = "SUB";
                screenTexts[0].text = "";
                screenTexts[1].text = "";
                for (int aa = 0; aa < 8; aa++)
                    screenTexts[aa].text = "";
                screenTexts[6].text = pressed.GetComponentInChildren<TextMesh>().text;
                screenTexts[6].fontSize = new int[] { 35, 35, 35, 32, 28 }[answer.Length - 4];
                submitScreen = true;
                submitMesh.material = materials[0];
                submitText.color = textColors[0];
            }
        }
    }
#pragma warning disable 414
    private string TwitchHelpMessage = "Move to other screens using !{0} right|left|r|l|. Submit the decrypted word with !{0} submit qwertyuiopasdfghjklzxcvbnm";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {

        if (command.EqualsIgnoreCase("right") || command.EqualsIgnoreCase("r"))
        {
            yield return null;
            rightArrow.OnInteract();
            yield return new WaitForSeconds(0.1f);

        }
        if (command.EqualsIgnoreCase("left") || command.EqualsIgnoreCase("l"))
        {
            yield return null;
            leftArrow.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 2 || !split[0].Equals("SUBMIT") || split[1].Length != 6) yield break;
        int[] buttons = split[1].Select(getPositionFromChar).ToArray();
        if (buttons.Any(x => x < 0)) yield break;

        yield return null;

        yield return new WaitForSeconds(0.1f);
        foreach (char let in split[1])
        {
            keyboard[getPositionFromChar(let)].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
        submit.OnInteract();
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        if (submitScreen && !answer.StartsWith(screenTexts[2].text))
        {
            KMSelectable[] arrows = new KMSelectable[] { leftArrow, rightArrow };
            arrows[UnityEngine.Random.Range(0, 2)].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        int start = submitScreen ? screenTexts[2].text.Length : 0;
        for (int i = start; i < 6; i++)
        {
            keyboard[getPositionFromChar(answer[i])].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        submit.OnInteract();
        yield return new WaitForSeconds(0.1f);
    }
    private int getPositionFromChar(char c)
    {
        return "QWERTYUIOPASDFGHJKLZXCVBNM".IndexOf(c);
    }
    void Update()
    {
        if (moduleSelected)
        {
            for (var ltr = 0; ltr < 26; ltr++)
                if (Input.GetKeyDown(((char) ('a' + ltr)).ToString()))
                    keyboard[getPositionFromChar((char) ('A' + ltr))].OnInteract();
            if (Input.GetKeyDown(KeyCode.Return))
                submit.OnInteract();
        }
    }
}
