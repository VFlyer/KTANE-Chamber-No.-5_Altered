using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class ChamberNoFive : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;
    private KMAudio.KMAudioRef SoundIThink;
    public KMSelectable[] LetterButtons;
    public KMSelectable StartButton;
    public TextMesh[] LetterOptions;
    public TextMesh Words;
    public TextMesh Timer;
    public Material[] Static;
    public GameObject Background;
    public GameObject Screen;
    public GameObject WholeAssTV;
    public Material[] StaticButBad;
    public KMSelectable[] LiterallyAllWrong;
    public KMSelectable TheTrumpet;
    public GameObject ButtonToStart;
    public Material End;
    public GameObject[] Instruments;
    public TextMesh[] InstrumentTexts;

    int[] Buttons = { 0, 1, 2, 3 };
    int[] TPNumbers = { 1, 2, 3, 4, 5, 6, 7 };

    float Hue = 0.1f;
    float Saturation = 0f;
    float TimerNumber = 30f;
    float Value = 1f;

    string[] Clues = { "LIFE", "SIDE", "NEED", "SEE", "SUN", "LONG", "AM" };
    string[] NamesOfGorls = { "MONICA", "ERICA", "RITA", "TINA", "SANDRA", "MARY", "JESSICA" };
    string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    bool[] Used = new bool[7];

    bool[] YouCantHide = new bool[4];
    bool Active;
    bool StageTwo;
    bool Sound = true;
    bool CanPress = true;
    bool AutosolveWait;

    Vector3[] BadTrumpets = new Vector3[7];
    Vector3[] Rotationsoftrumpet = new Vector3[7];

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    private IDictionary<string, object> tpAPI;
    private bool _twitchMode;

#pragma warning disable 414
    bool TwitchPlaysActive;
#pragma warning restore 414

    void Awake()
    {
        GetComponent<KMBombModule>().OnActivate += Activate;

        moduleId = moduleIdCounter++;

        foreach (KMSelectable Option in LetterButtons)
        {
            Option.OnInteract += delegate () { OptionPress(Option); return false; };
        }
        StartButton.OnInteract += delegate () { StartButtonPress(); return false; };
        foreach (KMSelectable Drum in LiterallyAllWrong)
        {
            Drum.OnInteract += delegate () { DrumPress(Drum); return false; };
        }
        TheTrumpet.OnInteract += delegate () { TheTrumpetPress(); return false; };
    }

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            LetterOptions[i].text = "";
        }
        Words.text = "Start";
    }

    void Activate()
    {
        if (TwitchPlaysActive)
        {
            //Sound = false;
            _twitchMode = true;
            GameObject tpAPIGameObject = GameObject.Find("TwitchPlays_Info");
            if (tpAPIGameObject != null)
                tpAPI = tpAPIGameObject.GetComponent<IDictionary<string, object>>();
            else
                _twitchMode = false;
            TPNumbers = TPNumbers.Shuffle();
            TimerNumber = 40f;
            Timer.text = "0:40.00";
            for (int i = 0; i < 7; i++)
            {
                InstrumentTexts[i].text = TPNumbers[i].ToString();
            }
        }
        else
        {
            for (int i = 0; i < 7; i++)
            {
                InstrumentTexts[i].text = "";
            }
        }
    }

    void DrumPress(KMSelectable Drum)
    {
        if (!StageTwo)
        {
            return;
        }
        for (int i = 0; i < LiterallyAllWrong.Length; i++)
        {
            if (Drum == LiterallyAllWrong[i])
            {
                GetComponent<KMBombModule>().HandleStrike();
            }
        }
    }

    void TheTrumpetPress()
    {
        if (!StageTwo)
        {
            return;
        }
        GetComponent<KMBombModule>().HandlePass();
        for (int i = 0; i < Instruments.Length; i++)
        {
            Instruments[i].gameObject.SetActive(false);
        }
        Debug.LogFormat("[Chamber No. 5 #{0}] The Trumpet!", moduleId);
        Background.GetComponent<MeshRenderer>().material = End;
        Background.GetComponent<Renderer>().material.color = Color.white;
    }

    void StartButtonPress()
    {
        if (Active)
        {
            return;
        }
        if (Sound)
        {
            SoundIThink = Audio.PlaySoundAtTransformWithRef("ALittleBit", transform);
        }
        StartButton.gameObject.SetActive(false);
        Active = true;
        WordChoice();
    }

    void OptionPress(KMSelectable Option)
    {
        if (!Active || !CanPress)
        {
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            if (Option == LetterButtons[i])
            {
                if (i == Buttons[0])
                {
                    for (int q = 0; q < 4; q++)
                    {
                        LetterOptions[q].text = "";
                    }
                    Words.text = "";
                    StartCoroutine(CorrectLetter());
                }
                else
                {
                    GetComponent<KMBombModule>().HandleStrike();
                    for (int q = 0; q < 4; q++)
                    {
                        LetterOptions[q].text = "";
                    }
                    Words.text = "";
                    StartCoroutine(WrongLetter());
                }
            }
        }
    }

    IEnumerator CorrectLetter()
    {
        CanPress = false;
        for (int p = 0; p < Static.Length; p++)
        {
            Screen.GetComponent<MeshRenderer>().material = Static[p];
            yield return new WaitForSeconds(.1f);
        }
        WordChoice();
        CanPress = true;
    }

    IEnumerator WrongLetter()
    {
        CanPress = false;
        for (int p = 0; p < Static.Length; p++)
        {
            Screen.GetComponent<MeshRenderer>().material = StaticButBad[p];
            yield return new WaitForSeconds(.1f);
        }
        for (int i = 0; i < 7; i++)
        {
            Used[i] = false;
        }
        WordChoice();
        CanPress = true;
    }

    IEnumerator Animation()
    {
        for (int i = 0; i < 100; i++)
        {
            WholeAssTV.transform.Translate(0.0f, -0.0005f, 0.0f);
            yield return new WaitForSeconds(.01f);
        }
        StageTwo = true;
        WholeAssTV.gameObject.SetActive(false);
        for (int i = 0; i < 7; i++)
        {
            BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
            float Whatever = 0f;
            while (Whatever == 0f)
            {
                Whatever = UnityEngine.Random.Range(-1f, 1f);
            }
            Rotationsoftrumpet[i] = new Vector3(0f, Whatever, 0f);
        }
    }

    IEnumerator Blacken()
    {
        if (SoundIThink != null)
        {
            SoundIThink.StopSound();
            SoundIThink = null;
        }
        float fadeOutTime = 0.5f;
        Renderer backRend = Background.GetComponent<Renderer>();
        Material originalMat = new Material(backRend.material);
        Material newMat = new Material(Static[10]);
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            backRend.material.Lerp(originalMat, newMat, t / fadeOutTime);
            yield return null;
        }
        yield return new WaitForSeconds(2.5f);
        AutosolveWait = false;
    }

    void WordChoice()
    {
        Buttons.Shuffle();
        int Dumbass = 0;
        Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (Used[0] && Used[1] && Used[2] && Used[3] && Used[4] && Used[5] && Used[6])
        {
            Active = false;
            AutosolveWait = true;
            StartCoroutine(Animation());
            StartCoroutine(Blacken());
            return;
        }
        Dumbass = UnityEngine.Random.Range(0, 7);
        while (Used[Dumbass])
        {
            Dumbass = UnityEngine.Random.Range(0, 7);
        }
        Words.text = Clues[Dumbass];
        Debug.LogFormat("[Chamber No. 5 #{0}] The chosen word was {1}, select any letter from {2}.", moduleId, Clues[Dumbass], NamesOfGorls[Dumbass]);
        LetterOptions[Buttons[0]].text = NamesOfGorls[Dumbass][UnityEngine.Random.Range(0, NamesOfGorls[Dumbass].Length)].ToString();
        for (int i = 0; i < NamesOfGorls[Dumbass].Length; i++)
        {
            Alphabet = Alphabet.Replace(NamesOfGorls[Dumbass][i].ToString(), "");
        }
        for (int i = 1; i < 4; i++)
        {
            LetterOptions[Buttons[i]].text = Alphabet[UnityEngine.Random.Range(0, Alphabet.Length)].ToString();
            Alphabet = Alphabet.Replace(LetterOptions[Buttons[i]].text.ToString(), "");
        }
        Used[Dumbass] = true;
        if (_twitchMode)
            tpAPI["ircConnectionSendMessage"] = "Module " + GetModuleCode() + " (Chamber No. 5) has screen " + Words.text.ToUpperInvariant() + " with letters " + LetterOptions[0].text + " " + LetterOptions[1].text + " " + LetterOptions[2].text + " " + LetterOptions[3].text + ".";
    }

    void Update()
    {
        if (Active)
        {
            TimerNumber -= Time.deltaTime;
            string DisplayTimerNumber = string.Format("{0:00.00}", Math.Round(TimerNumber, 2));
            Timer.text = "0:" + DisplayTimerNumber;
            if (TimerNumber <= 30f - 21.89f && !YouCantHide[0] && Sound)
            {
                SoundIThink.StopSound();
                SoundIThink = null;
                SoundIThink = Audio.PlaySoundAtTransformWithRef("youcanthide1.2x", transform);
                YouCantHide[0] = true;
            }
            if (TimerNumber <= 30f - 21.89f - 2.28f && !YouCantHide[1] && Sound)
            {
                SoundIThink.StopSound();
                SoundIThink = null;
                SoundIThink = Audio.PlaySoundAtTransformWithRef("youcanthide1.3x", transform);
                YouCantHide[1] = true;
            }
            if (TimerNumber <= 30f - 21.89f - 2.28f - 2.088f && !YouCantHide[2] && Sound)
            {
                SoundIThink.StopSound();
                SoundIThink = null;
                SoundIThink = Audio.PlaySoundAtTransformWithRef("youcanthide1.4x", transform);
                YouCantHide[2] = true;
            }
            if (TimerNumber <= 30f - 21.89f - 2.28f - 2.088f - 1.968f && !YouCantHide[3] && Sound)
            {
                SoundIThink.StopSound();
                SoundIThink = null;
                SoundIThink = Audio.PlaySoundAtTransformWithRef("youcanthide1.4x", transform);
                YouCantHide[3] = true;
            }
            if (TimerNumber < .01f)
            {
                StopAllCoroutines();
                while (SoundIThink != null)
                {
                    SoundIThink.StopSound();
                    SoundIThink = null;
                }
                Active = false;
                StartButton.gameObject.SetActive(true);
                for (int i = 0; i < 4; i++)
                {
                    LetterOptions[i].text = "";
                }
                Screen.GetComponent<MeshRenderer>().material = Static[10];
                Words.text = "Start";
                for (int i = 0; i < 7; i++)
                {
                    Used[i] = false;
                }
                TimerNumber = 30f;
                if (TwitchPlaysActive)
                {
                    TimerNumber = 40f;
                }
                for (int i = 0; i < YouCantHide.Length; i++)
                {
                    YouCantHide[i] = false;
                }
                if (!CanPress)
                    CanPress = true;
            }
        }
        else
        {
            while (SoundIThink != null)
            {
                SoundIThink.StopSound();
                SoundIThink = null;
            }
        }
        if (StageTwo)
        {
            for (int i = 0; i < 7; i++)
            {
                if (i != 6)
                {
                    LiterallyAllWrong[i].gameObject.transform.localEulerAngles += Rotationsoftrumpet[i] * Time.deltaTime * 100f;
                    LiterallyAllWrong[i].gameObject.transform.localPosition += BadTrumpets[i] * 0.2f * Time.deltaTime;
                    if (LiterallyAllWrong[i].gameObject.transform.localPosition.x < -0.07f)
                    {
                        BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(0.1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
                    }
                    if (LiterallyAllWrong[i].gameObject.transform.localPosition.z < -0.07f)
                    {
                        BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(0.1f, 1f));
                    }
                    if (LiterallyAllWrong[i].gameObject.transform.localPosition.x > 0.07f)
                    {
                        BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, -.1f), 0f, UnityEngine.Random.Range(-1f, 1f));
                    }
                    if (LiterallyAllWrong[i].gameObject.transform.localPosition.z > 0.07f)
                    {
                        BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, -.1f));
                    }
                }
                else
                {
                    TheTrumpet.gameObject.transform.localEulerAngles += Rotationsoftrumpet[i] * Time.deltaTime * 100f;
                    TheTrumpet.gameObject.transform.localPosition += BadTrumpets[i] * 0.2f * Time.deltaTime;
                    if (TheTrumpet.gameObject.transform.localPosition.x < -0.07f)
                    {
                        BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(0.1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
                    }
                    if (TheTrumpet.gameObject.transform.localPosition.z < -0.07f)
                    {
                        BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(0.1f, 1f));
                    }
                    if (TheTrumpet.gameObject.transform.localPosition.x > 0.07f)
                    {
                        BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, -.1f), 0f, UnityEngine.Random.Range(-1f, 1f));
                    }
                    if (TheTrumpet.gameObject.transform.localPosition.z > 0.07f)
                    {
                        BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, -.1f));
                    }
                }
            }
        }
    }

    private string GetModuleCode()
    {
        Transform closest = null;
        float closestDistance = float.MaxValue;
        foreach (Transform children in transform.parent)
        {
            var distance = (transform.position - children.position).magnitude;
            if (children.gameObject.name == "TwitchModule(Clone)" && (closest == null || distance < closestDistance))
            {
                closest = children;
                closestDistance = distance;
            }
        }
        return closest != null ? closest.Find("MultiDeckerUI").Find("IDText").GetComponent<UnityEngine.UI.Text>().text : null;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} start to start. Use !{0} X to press that letter. Use !{0} # to press the instrument with that number. On TP the timer is extended to 40 seconds.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        Command = Command.Trim().ToUpper();
        yield return null;
        if (Command == "START" & !StageTwo)
        {
            StartButton.OnInteract();
        }
        else if (!StageTwo)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Command == LetterOptions[i].text)
                {
                    LetterButtons[i].OnInteract();
                    yield break;
                }
            }
            yield return "sendtochaterror I don't understand!";
        }
        else
        {
            if (Command.Length != 1)
            {
                yield return "sendtochaterror I don't understand!";
                yield break;
            }
            else if (!"1234567".Contains(Command))
            {
                yield return "sendtochaterror I don't understand!";
                yield break;
            }
            if (Array.IndexOf(TPNumbers, int.Parse(Command)) == 0)
            {
                TheTrumpet.OnInteract();
            }
            else
            {
                LiterallyAllWrong[UnityEngine.Random.Range(0, 6)].OnInteract();
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (!StageTwo && !Active)
        {
            StartButton.OnInteract();
            yield return new WaitForSeconds(.1f);
        }
        while (!StageTwo && !AutosolveWait)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i == Buttons[0])
                {
                    LetterButtons[i].OnInteract();
                }
            }
            yield return null;
        }
        while (AutosolveWait)
        {
            yield return true;
        }
        TheTrumpet.OnInteract();
    }
}
