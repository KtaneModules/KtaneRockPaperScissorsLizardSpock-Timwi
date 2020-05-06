using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KModkit;
using RockPaperScissorsLizardSpock;
using UnityEngine;
using Rnd = UnityEngine.Random;

/// <summary>
/// On the Subject of Rock-Paper-Scissors-Lizard-Spock
/// Created by Timwi
/// </summary>
public class RockPaperScissorsLizardSpockModule : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;
    public KMRuleSeedable RuleSeedable;

    public KMSelectable Rock;
    public KMSelectable Paper;
    public KMSelectable Scissors;
    public KMSelectable Lizard;
    public KMSelectable Spock;

    private KMSelectable[] _all;
    private KMSelectable _decoy;
    private int[] _mustPress;
    private HashSet<int> _pressed = new HashSet<int>();

    private static string[] _names = new[] { "Rock", "Paper", "Scissors", "Lizard", "Spock" };

    public Material MatCorrect;
    public Material MatWrong;
    public KMSelectable MainSelectable;

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    void Start()
    {
        _moduleId = _moduleIdCounter++;
        _all = new[] { Rock, Paper, Scissors, Lizard, Spock };
        var names = "Rock,Paper,Scissors,Lizard,Spock".Split(',');
        for (int i = 0; i < 5; i++)
            _all[i].OnInteract += GetButtonPressHandler(i, _all[i], names[i]);

        // Now shuffle them randomly
        for (int i = 4; i > 0; i--)
        {
            var ix = Rnd.Range(0, i);
            var t = _all[ix];
            _all[ix] = _all[i];
            _all[i] = t;
        }

        // Choose one of the possible arrangements
        int?[] children = null;
        switch (Rnd.Range(0, 10))
        {
            case 0: // pentagon
                for (int i = 0; i < 5; i++)
                    _all[i].transform.LocalTranslate(new Vector3((float) (.055 * Math.Sin(Math.PI / 5 * 2 * i)), 0, (float) (-.055 * Math.Cos(Math.PI / 5 * 2 * i))));
                _decoy = null;
                children = new int?[] { 1, 0, 4, 2, 2, 3 };
                break;

            case 1: // in two rows
                for (int i = 0; i < 5; i++)
                    _all[i].transform.LocalTranslate(new Vector3((i < 2 ? i : i - 3) * .055f, 0, (i < 2 ? -.035f : .035f)));
                _decoy = _all[3];
                children = new int?[] { 1, 0, null, 4, 3, 2 };
                break;

            case 2: // in two columns
                for (int i = 0; i < 5; i++)
                    _all[i].transform.LocalTranslate(new Vector3((i < 2 ? -.035f : .035f), 0, (i < 2 ? i : i - 3) * .055f));
                _decoy = _all[3];
                children = new int?[] { 2, null, null, 3, 0, null, 4, 1, null };
                break;

            case 3: // X, bottom
                for (int i = 0; i < 5; i++)
                    _all[i].transform.LocalTranslate(new Vector3(.055f * (2 * i % 3 - 1), 0, .035f * (2 * i / 3 - 1) + .015f));
                _decoy = _all[2];
                children = new int?[] { 1, 2, 0, 4, 2, 3 };
                break;

            case 4: // X, left
                for (int i = 0; i < 5; i++)
                    _all[i].transform.LocalTranslate(new Vector3(.035f * (2 * i % 3 - 1) + .02f, 0, .055f * (2 * i / 3 - 1)));
                _decoy = _all[2];
                children = new int?[] { 1, 0, null, 2, 2, null, 4, 3, null };
                break;

            case 5: // quarter circle with a bottom-left pivot
                for (int i = 0; i < 4; i++)
                    _all[i].transform.LocalTranslate(new Vector3((float) (-.07 * Math.Sin(Math.PI * (i * .25 - .125)) + .02), 0, (float) (-.07 * Math.Cos(Math.PI * (i * .25 - .125)) + .02)));
                _all[4].transform.LocalTranslate(new Vector3(.02f, 0, .02f));
                _decoy = _all[4];
                children = new int?[] { 0, 1, null, 4, 4, 2, 4, 4, 3 };
                break;

            case 6: // quarter circle with a top-right pivot
                for (int i = 0; i < 4; i++)
                    _all[i].transform.LocalTranslate(new Vector3((float) (.07 * Math.Sin(Math.PI * (i * .25 - .125)) - .02), 0, (float) (.07 * Math.Cos(Math.PI * (i * .25 - .125)) - .02)));
                _all[4].transform.LocalTranslate(new Vector3(-.02f, 0, -.02f));
                _decoy = _all[4];
                children = new int?[] { 3, 4, 4, 2, 4, 4, null, 1, 0 };
                break;

            case 7: // +
                _all[0].transform.LocalTranslate(new Vector3(.055f, 0, 0));
                _all[1].transform.LocalTranslate(new Vector3(0, 0, .055f));
                _all[2].transform.LocalTranslate(new Vector3(-.055f, 0, 0));
                _all[3].transform.LocalTranslate(new Vector3(0, 0, -.055f));
                _decoy = _all[4];
                children = new int?[] { null, 3, null, 0, 4, 2, null, 1, null };
                break;

            case 8: // Z
                _all[0].transform.LocalTranslate(new Vector3(.055f, 0, -.055f));
                _all[1].transform.LocalTranslate(new Vector3(0, 0, .055f));
                _all[2].transform.LocalTranslate(new Vector3(-.055f, 0, .055f));
                _all[3].transform.LocalTranslate(new Vector3(0, 0, -.055f));
                _decoy = _all[4];
                children = new int?[] { 0, 3, null, null, 4, null, null, 1, 2 };
                break;

            case 9: // S
                _all[0].transform.LocalTranslate(new Vector3(.055f, 0, -.0275f));
                _all[1].transform.LocalTranslate(new Vector3(0, 0, .055f));
                _all[2].transform.LocalTranslate(new Vector3(-.055f, 0, .0275f));
                _all[3].transform.LocalTranslate(new Vector3(0, 0, -.055f));
                _decoy = _all[4];
                children = new int?[] { 0, 3, null, 0, 4, 2, null, 1, 2 };
                break;
        }

        if (_decoy != null)
            Debug.LogFormat("[Rock-Paper-Scissors-Lizard-Spock #{0}] Decoy is {1}.", _moduleId, _names[Array.IndexOf(new[] { Rock, Paper, Scissors, Lizard, Spock }, _decoy)]);
        else
            Debug.LogFormat("[Rock-Paper-Scissors-Lizard-Spock #{0}] There is no decoy.", _moduleId);

        MainSelectable.Children = children.Select(nint => nint == null ? null : _all[nint.Value].GetComponent<KMSelectable>()).ToArray();
        MainSelectable.UpdateChildren();

        // So much for arranging the signs on the module. Now for generating the rules!
        var rnd = RuleSeedable.GetRNG();
        Debug.LogFormat("[Rock-Paper-Scissors-Lizard-Spock #{0}] Using rule seed: {1}", _moduleId, rnd.Seed);

        var letters = "SYHREAGCUPMZKNBJQDFLXVWIOT".OrderBy(ch => rnd.NextDouble()).ToArray();
        var ports = new[] { Port.RJ45, Port.StereoRCA, Port.DVI, Port.Parallel, Port.Serial, Port.PS2 }.OrderBy(port => rnd.NextDouble()).ToArray();
        var litIndicators = new[] { "IND", "TRN", "SIG", "CAR", "NSA", "FRK", "CLR", "MSA", "SND", "FRQ", "BOB" }.OrderBy(ind => rnd.NextDouble()).ToArray();
        var unlitIndicators = new[] { "CLR", "CAR", "FRQ", "IND", "MSA", "NSA", "TRN", "SIG", "SND", "BOB", "FRK" }.OrderBy(ind => rnd.NextDouble()).ToArray();
        var digits = "9574831602".OrderBy(ch => rnd.NextDouble()).ToArray();

        var serial = Bomb.GetSerialNumber();
        var decoy = Array.IndexOf(new[] { Rock, Paper, Scissors, Lizard, Spock }, _decoy);

        var scores = newArray(
            // ports
            Bomb.GetPortCount(ports[0]) > 0 ? null : newArray(
                /* Rock */      Bomb.GetPortCount(ports[1]),
                /* Paper */     Bomb.GetPortCount(ports[2]),
                /* Scissors */  Bomb.GetPortCount(ports[3]),
                /* Lizard */    Bomb.GetPortCount(ports[4]),
                /* Spock */     Bomb.GetPortCount(ports[5])
            ),

            // serial number digits
            newArray(
                /* Rock */      serial.Count(c => c == digits[0] || c == digits[1]),
                /* Paper */     serial.Count(c => c == digits[2] || c == digits[3]),
                /* Scissors */  serial.Count(c => c == digits[4] || c == digits[5]),
                /* Lizard */    serial.Count(c => c == digits[6] || c == digits[7]),
                /* Spock */     serial.Count(c => c == digits[8] || c == digits[9])
            ),

            // unlit indicators
            Bomb.IsIndicatorOff(unlitIndicators[0]) ? null : newArray(
                /* Rock */      Bomb.GetOffIndicators().Count(i => i == unlitIndicators[1] || i == unlitIndicators[2]),
                /* Paper */     Bomb.GetOffIndicators().Count(i => i == unlitIndicators[3] || i == unlitIndicators[4]),
                /* Scissors */  Bomb.GetOffIndicators().Count(i => i == unlitIndicators[5] || i == unlitIndicators[6]),
                /* Lizard */    Bomb.GetOffIndicators().Count(i => i == unlitIndicators[7] || i == unlitIndicators[8]),
                /* Spock */     Bomb.GetOffIndicators().Count(i => i == unlitIndicators[9] || i == unlitIndicators[10])
            ),

            // serial number letters
            serial.Contains(letters[0]) || serial.Contains(letters[1]) ? null : newArray(
                /* Rock */      serial.Count(c => c == letters[2] || c == letters[3]),
                /* Paper */     serial.Count(c => c == letters[4] || c == letters[5]),
                /* Scissors */  serial.Count(c => c == letters[6] || c == letters[7]),
                /* Lizard */    serial.Count(c => c == letters[8] || c == letters[9]),
                /* Spock */     serial.Count(c => c == letters[10] || c == letters[11])
            ),

            // lit indicators
            Bomb.IsIndicatorOn(litIndicators[0]) ? null : newArray(
                /* Rock */      Bomb.GetOnIndicators().Count(i => i == litIndicators[1] || i == litIndicators[2]),
                /* Paper */     Bomb.GetOnIndicators().Count(i => i == litIndicators[3] || i == litIndicators[4]),
                /* Scissors */  Bomb.GetOnIndicators().Count(i => i == litIndicators[5] || i == litIndicators[6]),
                /* Lizard */    Bomb.GetOnIndicators().Count(i => i == litIndicators[7] || i == litIndicators[8]),
                /* Spock */     Bomb.GetOnIndicators().Count(i => i == litIndicators[9] || i == litIndicators[10])
            )
        )
            .OrderBy(row => rnd.NextDouble()).ToArray();

        var result = scores
            .Select((row, ix) => row == null ? null : row.Max().Apply(maxScore => new { Row = ix, Winners = row.SelectIndexWhere(sc => sc == maxScore).ToArray() }))
            .Where(inf => inf != null && inf.Winners.Length == 1 && inf.Winners[0] != decoy)
            .FirstOrDefault();

        if (result != null)
        {
            Debug.LogFormat("[Rock-Paper-Scissors-Lizard-Spock #{2}] {0} wins on account of Row #{1}.", _names[result.Winners[0]], result.Row + 1, _moduleId);

            switch (result.Winners[0])
            {
                case 0: _mustPress = new[] { 1, 4 }; break;
                case 1: _mustPress = new[] { 2, 3 }; break;
                case 2: _mustPress = new[] { 0, 4 }; break;
                case 3: _mustPress = new[] { 0, 2 }; break;
                default: _mustPress = new[] { 1, 3 }; break;
            }
        }
        else
        {
            Debug.LogFormat("[Rock-Paper-Scissors-Lizard-Spock #{1}] No winner{0}.", decoy == -1 ? " and no decoy" : null, _moduleId);
            _mustPress = Enumerable.Range(0, 5).Where(i => i != decoy).ToArray();
        }

        Debug.LogFormat("[Rock-Paper-Scissors-Lizard-Spock #{1}] Must press: {0}", string.Join(", ", _mustPress.Select(m => _names[m]).ToArray()), _moduleId);
    }

    private static T[] newArray<T>(params T[] array) { return array; }

    private KMSelectable.OnInteractHandler GetButtonPressHandler(int index, KMSelectable obj, string sound)
    {
        return delegate
        {
            if (_mustPress == null || !_pressed.Add(index))
                return false;

            obj.AddInteractionPunch();
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 0.014f, obj.transform.localPosition.z);

            if (!_mustPress.Contains(index))
            {
                obj.GetComponent<MeshRenderer>().material = MatWrong;
                Debug.LogFormat("[Rock-Paper-Scissors-Lizard-Spock #{1}] {0} is wrong.", _names[index], _moduleId);
                Module.HandleStrike();
            }
            else
            {
                Audio.PlaySoundAtTransform(sound, obj.transform);
                obj.GetComponent<MeshRenderer>().material = MatCorrect;
                var solved = false;
                if (!_mustPress.Except(_pressed).Any())
                {
                    solved = true;
                    _mustPress = null;
                    Module.HandlePass();
                }
                Debug.LogFormat("[Rock-Paper-Scissors-Lizard-Spock #{1}] {0} is correct.{2}", _names[index], _moduleId, solved ? " Module solved." : "");
            }
            return false;
        };
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Submit your answer with “!{0} Scissors Lizard”.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        var pieces = command.Trim().ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var skip = 0;
        if (pieces.Length > 0 && pieces[0] == "press")
            skip = 1;

        var list = new List<KMSelectable>();
        foreach (var piece in pieces.Skip(skip))
        {
            switch (piece)
            {
                case "rock": list.Add(Rock); break;
                case "paper": list.Add(Paper); break;
                case "scissors": list.Add(Scissors); break;
                case "lizard": list.Add(Lizard); break;
                case "spock": list.Add(Spock); break;
                default: yield break;
            }
        }
        if (list.Count == 0)
            yield break;
        yield return null;
        foreach (var item in list)
        {
            item.OnInteract();
            yield return new WaitForSeconds(.8f);
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        var selectables = new[] { Rock, Paper, Scissors, Lizard, Spock };
        foreach (var i in _mustPress)
            if (!_pressed.Contains(i))
            {
                selectables[i].OnInteract();
                yield return new WaitForSeconds(.8f);
            }
    }
}
