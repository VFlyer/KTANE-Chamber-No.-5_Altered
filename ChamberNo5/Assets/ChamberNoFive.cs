using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class ChamberNoFive : MonoBehaviour {

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

    string[] NamesOfGorls = {"MONICA", "ERICA", "RITA", "TINA", "SANDRA", "MARY", "JESSICA"};
    string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string[] Clues = {"LIFE", "SIDE", "NEED", "SEE", "SUN", "LONG", "AM"};
    bool[] Used = {false, false, false, false, false, false, false};
    int[] Buttons = {0, 1, 2, 3};
    float TimerNumber = 30f;
    float Hue = 0.1f;
    float Saturation = 0f;
    float Value = 1f;

    bool Active = false;
    bool StageTwo = false;
    bool[] YouCantHide = {false, false, false, false};

    Vector3[] BadTrumpets = new Vector3[7];
    Vector3[] Rotationsoftrumpet = new Vector3[7];

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake() {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable Option in LetterButtons) {
            Option.OnInteract += delegate () { OptionPress(Option); return false; };
        }
        StartButton.OnInteract += delegate () { StartButtonPress(); return false; };
        foreach (KMSelectable Drum in LiterallyAllWrong) {
            Drum.OnInteract += delegate () { DrumPress(Drum); return false; };
        }
        TheTrumpet.OnInteract += delegate () { TheTrumpetPress(); return false; };
    }

    void Start() {
      for (int i = 0; i < 4; i++) {
        LetterOptions[i].text = "";
      }
      Words.text = "Start";
    }

    void DrumPress (KMSelectable Drum) {
      for (int i = 0; i < LiterallyAllWrong.Length; i++) {
        if (Drum == LiterallyAllWrong[i]) {
          GetComponent<KMBombModule>().HandleStrike();
        }
      }
    }

    void TheTrumpetPress () {
      GetComponent<KMBombModule>().HandlePass();
      for (int i = 0; i < Instruments.Length; i++) {
        Instruments[i].gameObject.SetActive(false);
      }
      Debug.LogFormat("[Chamber No. 5 #{0}] The Trumpet!", moduleId);
      Background.GetComponent<MeshRenderer>().material = End;
      Background.GetComponent<Renderer>().material.color = Color.white;
    }

    void StartButtonPress () {
      if (Active) {
        return;
      }
      SoundIThink = Audio.PlaySoundAtTransformWithRef("ALittleBit", transform);
      StartButton.gameObject.SetActive(false);
      Active = true;
      WordChoice();
    }

    void OptionPress(KMSelectable Option) {
      if (!Active) {
        return;
      }
      for (int i = 0; i < 4; i++) {
        if (Option == LetterButtons[i]) {
          if (i == Buttons[0]) {
            for (int q = 0; q < 4; q++) {
              LetterOptions[q].text = "";
            }
            Words.text = "";
            StartCoroutine(FuckThisNeededToBeACoroutine());
          }
          else {
            GetComponent<KMBombModule>().HandleStrike();
            for (int q = 0; q < 4; q++) {
              LetterOptions[q].text = "";
            }
            Words.text = "";
            StartCoroutine(FuckThisNeededToBeACoroutineButBad());

          }
        }
      }
    }

    IEnumerator FuckThisNeededToBeACoroutine () {
      for (int p = 0; p < Static.Length; p++) {
        Screen.GetComponent<MeshRenderer>().material = Static[p];
        yield return new WaitForSeconds(.1f);
      }
      WordChoice();
    }

    IEnumerator FuckThisNeededToBeACoroutineButBad () {
      for (int p = 0; p < Static.Length; p++) {
        Screen.GetComponent<MeshRenderer>().material = StaticButBad[p];
        yield return new WaitForSeconds(.1f);
      }
      for (int i = 0; i < 7; i++) {
        Used[i] = false;
      }
      WordChoice();
    }

    IEnumerator Animation () {
      for (int i = 0; i < 100; i++) {
        WholeAssTV.transform.Translate(0.0f, -0.0005f, 0.0f);
        yield return new WaitForSeconds(.01f);
      }
      StageTwo = true;
      WholeAssTV.gameObject.SetActive(false);
      for (int i = 0; i < 7; i++) {
        BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
        float Whatever = 0f;
        while (Whatever == 0f) {
          Whatever = UnityEngine.Random.Range(-1f, 1f);
        }
        Rotationsoftrumpet[i] = new Vector3(0f, Whatever, 0f);
      }
    }

    IEnumerator Blacken()
    {
      if (SoundIThink != null) {
        SoundIThink.StopSound();
        SoundIThink = null;
      }
        float fadeOutTime = 2.0f;
        Material originalMat = Background.GetComponent<Renderer>().material;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            Background.GetComponent<Renderer>().material.Lerp(originalMat, Static[10], Mathf.Min(1, t / fadeOutTime));
            yield return null;
        }
    }

    void WordChoice() {
      Buttons.Shuffle();
      int Dumbass = 0;
      Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      if (Used[0] && Used[1] && Used[2] && Used[3] && Used[4] && Used[5] && Used[6]) {
        Active = false;
        StartCoroutine(Animation());
        StartCoroutine(Blacken());
        return;
      }
      Dumbass = UnityEngine.Random.Range(0, 7);
      while (Used[Dumbass]) {
        Dumbass = UnityEngine.Random.Range(0, 7);
      }
      Words.text = Clues[Dumbass];
      Debug.LogFormat("[Chamber No. 5 #{0}] The chosen word was {1}, select any letter from {2}.", moduleId, Clues[Dumbass], NamesOfGorls[Dumbass]);
      LetterOptions[Buttons[0]].text = NamesOfGorls[Dumbass][UnityEngine.Random.Range(0, NamesOfGorls[Dumbass].Length)].ToString();
      for (int i = 0; i < NamesOfGorls[Dumbass].Length; i++) {
        Alphabet = Alphabet.Replace(NamesOfGorls[Dumbass][i].ToString(), "");
      }
      for (int i = 1; i < 4; i++) {
        LetterOptions[Buttons[i]].text = Alphabet[UnityEngine.Random.Range(0, Alphabet.Length)].ToString();
        Alphabet = Alphabet.Replace(LetterOptions[Buttons[i]].text.ToString(), "");
      }
      Used[Dumbass] = true;
    }

    void Update () {
      if (Active) {
        TimerNumber -= Time.deltaTime;
        string DisplayTimerNumber = string.Format("{0:00.00}", Math.Round(TimerNumber, 2));
        Timer.text = "0:" + DisplayTimerNumber;
        if (TimerNumber <= 30f - 21.89f && !YouCantHide[0]) {
          SoundIThink.StopSound();
          SoundIThink = null;
          SoundIThink = Audio.PlaySoundAtTransformWithRef("youcanthide1.2x", transform);
          YouCantHide[0] = true;
        }
        if (TimerNumber <= 30f - 21.89f - 2.28f && !YouCantHide[1]) {
          SoundIThink.StopSound();
          SoundIThink = null;
          SoundIThink = Audio.PlaySoundAtTransformWithRef("youcanthide1.3x", transform);
          YouCantHide[1] = true;
        }
        if (TimerNumber <= 30f - 21.89f - 2.28f - 2.088f && !YouCantHide[2]) {
          SoundIThink.StopSound();
          SoundIThink = null;
          SoundIThink = Audio.PlaySoundAtTransformWithRef("youcanthide1.4x", transform);
          YouCantHide[2] = true;
        }
        if (TimerNumber <= 30f - 21.89f - 2.28f - 2.088f - 1.968f  && !YouCantHide[3]) {
          SoundIThink.StopSound();
          SoundIThink = null;
          SoundIThink = Audio.PlaySoundAtTransformWithRef("youcanthide1.4x", transform);
          YouCantHide[3] = true;
        }
        if (TimerNumber < .01f) {
          while (SoundIThink != null) {
            SoundIThink.StopSound();
            SoundIThink = null;
          }
          Active = false;
          StartButton.gameObject.SetActive(true);
          for (int i = 0; i < 4; i++) {
            LetterOptions[i].text = "";
          }
          Words.text = "Start";
          for (int i = 0; i < 7; i++) {
            Used[i] = false;
          }
          TimerNumber = 30f;
          for (int i = 0; i < YouCantHide.Length; i++) {
            YouCantHide[i] = false;
          }
        }
      }
      else {
        while (SoundIThink != null) {
          SoundIThink.StopSound();
          SoundIThink = null;
        }
      }
      if (StageTwo) {
        for (int i = 0; i < 7; i++) {
          if (i != 6) {
            LiterallyAllWrong[i].gameObject.transform.localEulerAngles += Rotationsoftrumpet[i] * Time.deltaTime * 100f;
            LiterallyAllWrong[i].gameObject.transform.localPosition += BadTrumpets[i] * 0.2f * Time.deltaTime;
            if (LiterallyAllWrong[i].gameObject.transform.localPosition.x < -0.07f) {
              BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(0.1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
            }
            if (LiterallyAllWrong[i].gameObject.transform.localPosition.z < -0.07f) {
              BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(0.1f, 1f));
            }
            if (LiterallyAllWrong[i].gameObject.transform.localPosition.x > 0.07f) {
              BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, -.1f), 0f, UnityEngine.Random.Range(-1f, 1f));
            }
            if (LiterallyAllWrong[i].gameObject.transform.localPosition.z > 0.07f) {
              BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, -.1f));
            }
          }
          else {
            TheTrumpet.gameObject.transform.localEulerAngles += Rotationsoftrumpet[i] * Time.deltaTime * 100f;
            TheTrumpet.gameObject.transform.localPosition += BadTrumpets[i] * 0.2f * Time.deltaTime;
            if (TheTrumpet.gameObject.transform.localPosition.x < -0.07f) {
              BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(0.1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
            }
            if (TheTrumpet.gameObject.transform.localPosition.z < -0.07f) {
              BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(0.1f, 1f));
            }
            if (TheTrumpet.gameObject.transform.localPosition.x > 0.07f) {
              BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, -.1f), 0f, UnityEngine.Random.Range(-1f, 1f));
            }
            if (TheTrumpet.gameObject.transform.localPosition.z > 0.07f) {
              BadTrumpets[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, -.1f));
            }
          }
        }
      }
    }
    /*
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} VOL up/down to interact with the volume buttons, CH up/down to change the channel, and Submit to submit the current frame.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command){
      yield return null;
    }*/
}
