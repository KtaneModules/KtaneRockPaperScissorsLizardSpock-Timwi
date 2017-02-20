using System;
using System.Collections.Generic;
using System.Linq;
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

    public Transform Rock;
    public Transform Paper;
    public Transform Scissors;
    public Transform Lizard;
    public Transform Spock;

    private Transform[] _all;
    private Transform _decoy;
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
        Module.OnActivate += ActivateModule;
        _all = new[] { Rock, Paper, Scissors, Lizard, Spock };
        for (int i = 0; i < 5; i++)
        {
            var j = i;
            var obj = _all[i];
            var selectable = obj.GetComponent<KMSelectable>();
            selectable.OnInteract += delegate { selectable.AddInteractionPunch(); HandlePress(j, obj); return false; };
        }

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
                    _all[i].LocalTranslate(new Vector3((float) (.055 * Math.Sin(Math.PI / 5 * 2 * i)), 0, (float) (-.055 * Math.Cos(Math.PI / 5 * 2 * i))));
                _decoy = null;
                children = new int?[] { 1, 0, 4, 2, 2, 3 };
                break;

            case 1: // in two rows
                for (int i = 0; i < 5; i++)
                    _all[i].LocalTranslate(new Vector3((i < 2 ? i : i - 3) * .055f, 0, (i < 2 ? -.035f : .035f)));
                _decoy = _all[3];
                children = new int?[] { 1, 0, null, 4, 3, 2 };
                break;

            case 2: // in two columns
                for (int i = 0; i < 5; i++)
                    _all[i].LocalTranslate(new Vector3((i < 2 ? -.035f : .035f), 0, (i < 2 ? i : i - 3) * .055f));
                _decoy = _all[3];
                children = new int?[] { 2, null, null, 3, 0, null, 4, 1, null };
                break;

            case 3: // X, bottom
                for (int i = 0; i < 5; i++)
                    _all[i].LocalTranslate(new Vector3(.055f * (2 * i % 3 - 1), 0, .035f * (2 * i / 3 - 1) + .015f));
                _decoy = _all[2];
                children = new int?[] { 1, 2, 0, 4, 2, 3 };
                break;

            case 4: // X, left
                for (int i = 0; i < 5; i++)
                    _all[i].LocalTranslate(new Vector3(.035f * (2 * i % 3 - 1) + .02f, 0, .055f * (2 * i / 3 - 1)));
                _decoy = _all[2];
                children = new int?[] { 1, 0, null, 2, 2, null, 4, 3, null };
                break;

            case 5: // quarter circle with a bottom-left pivot
                for (int i = 0; i < 4; i++)
                    _all[i].LocalTranslate(new Vector3((float) (-.07 * Math.Sin(Math.PI * (i * .25 - .125)) + .02), 0, (float) (-.07 * Math.Cos(Math.PI * (i * .25 - .125)) + .02)));
                _all[4].LocalTranslate(new Vector3(.02f, 0, .02f));
                _decoy = _all[4];
                children = new int?[] { 0, 1, null, 4, 4, 2, 4, 4, 3 };
                break;

            case 6: // quarter circle with a top-right pivot
                for (int i = 0; i < 4; i++)
                    _all[i].LocalTranslate(new Vector3((float) (.07 * Math.Sin(Math.PI * (i * .25 - .125)) - .02), 0, (float) (.07 * Math.Cos(Math.PI * (i * .25 - .125)) - .02)));
                _all[4].LocalTranslate(new Vector3(-.02f, 0, -.02f));
                _decoy = _all[4];
                children = new int?[] { 3, 4, 4, 2, 4, 4, null, 1, 0 };
                break;

            case 7: // +
                _all[0].LocalTranslate(new Vector3(.055f, 0, 0));
                _all[1].LocalTranslate(new Vector3(0, 0, .055f));
                _all[2].LocalTranslate(new Vector3(-.055f, 0, 0));
                _all[3].LocalTranslate(new Vector3(0, 0, -.055f));
                _decoy = _all[4];
                children = new int?[] { null, 3, null, 0, 4, 2, null, 1, null };
                break;

            case 8: // Z
                _all[0].LocalTranslate(new Vector3(.055f, 0, -.055f));
                _all[1].LocalTranslate(new Vector3(0, 0, .055f));
                _all[2].LocalTranslate(new Vector3(-.055f, 0, .055f));
                _all[3].LocalTranslate(new Vector3(0, 0, -.055f));
                _decoy = _all[4];
                children = new int?[] { 0, 3, null, null, 4, null, null, 1, 2 };
                break;

            case 9: // S
                _all[0].LocalTranslate(new Vector3(.055f, 0, -.0275f));
                _all[1].LocalTranslate(new Vector3(0, 0, .055f));
                _all[2].LocalTranslate(new Vector3(-.055f, 0, .0275f));
                _all[3].LocalTranslate(new Vector3(0, 0, -.055f));
                _decoy = _all[4];
                children = new int?[] { 0, 3, null, 0, 4, 2, null, 1, 2 };
                break;
        }

        MainSelectable.Children = children.Select(nint => nint == null ? null : _all[nint.Value].GetComponent<KMSelectable>()).ToArray();
        MainSelectable.UpdateChildren();
    }

    /// <summary>
    ///     Generates a random string of the specified length, taking characters from the specified arsenal of characters.</summary>
    /// <param name="length">
    ///     Length of the string to generate.</param>
    /// <param name="takeCharactersFrom">
    ///     Arsenal to take random characters from. (Default is upper- and lower-case letters and digits.)</param>
    /// <param name="rnd">
    ///     If not <c>null</c>, uses the specified random number generator.</param>
    static string GenerateString(int length, string takeCharactersFrom)
    {
        return new string(Enumerable.Range(0, length).Select(i => takeCharactersFrom[Rnd.Range(0, takeCharactersFrom.Length)]).ToArray());
    }

    void ActivateModule()
    {
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var digits = "0123456789";
        var serial = Bomb.GetSerialNumber() ?? new string("??DLXD".Select(ch => (ch == 'L' ? letters : ch == 'D' ? digits : ch == '?' ? letters + digits : ch.ToString()).Apply(chs => chs[Rnd.Range(0, chs.Length)])).ToArray());

        var decoy = Array.IndexOf(new[] { Rock, Paper, Scissors, Lizard, Spock }, _decoy);

        var scores = newArray(
            // Row 1: serial number letter
            serial.Contains('X') || serial.Contains('Y') ? null : newArray(
                /* Rock */      serial.Count(c => c == 'R' || c == 'O'),
                /* Paper */     serial.Count(c => c == 'P' || c == 'A'),
                /* Scissors */  serial.Count(c => c == 'S' || c == 'I'),
                /* Lizard */    serial.Count(c => c == 'L' || c == 'Z'),
                /* Spock */     serial.Count(c => c == 'C' || c == 'K')
            ),

            // Row 2: port
            Bomb.GetPortCount(KMBombInfoExtensions.KnownPortType.PS2) > 0 ? null : newArray(
                /* Rock */      Bomb.GetPortCount(KMBombInfoExtensions.KnownPortType.RJ45),
                /* Paper */     Bomb.GetPortCount(KMBombInfoExtensions.KnownPortType.Parallel),
                /* Scissors */  Bomb.GetPortCount(KMBombInfoExtensions.KnownPortType.Serial),
                /* Lizard */    Bomb.GetPortCount(KMBombInfoExtensions.KnownPortType.DVI),
                /* Spock */     Bomb.GetPortCount(KMBombInfoExtensions.KnownPortType.StereoRCA)
            ),

            // Row 3: lit indicator
            Bomb.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.TRN) ? null : newArray(
                /* Rock */      Bomb.GetOnIndicators().Count(i => i == "FRK" || i == "FRQ"),
                /* Paper */     Bomb.GetOnIndicators().Count(i => i == "BOB" || i == "IND"),
                /* Scissors */  Bomb.GetOnIndicators().Count(i => i == "CAR" || i == "SIG"),
                /* Lizard */    Bomb.GetOnIndicators().Count(i => i == "CLR" || i == "NSA"),
                /* Spock */     Bomb.GetOnIndicators().Count(i => i == "SND" || i == "MSA")
            ),

            // Row 4: unlit indicator
            Bomb.IsIndicatorOff(KMBombInfoExtensions.KnownIndicatorLabel.TRN) ? null : newArray(
                /* Rock */      Bomb.GetOffIndicators().Count(i => i == "FRK" || i == "FRQ"),
                /* Paper */     Bomb.GetOffIndicators().Count(i => i == "BOB" || i == "IND"),
                /* Scissors */  Bomb.GetOffIndicators().Count(i => i == "CAR" || i == "SIG"),
                /* Lizard */    Bomb.GetOffIndicators().Count(i => i == "CLR" || i == "NSA"),
                /* Spock */     Bomb.GetOffIndicators().Count(i => i == "SND" || i == "MSA")
            ),

            // Row 5: serial number digits
            newArray(
                /* Rock */      serial.Count(c => c == '0' || c == '5'),
                /* Paper */     serial.Count(c => c == '3' || c == '6'),
                /* Scissors */  serial.Count(c => c == '1' || c == '9'),
                /* Lizard */    serial.Count(c => c == '2' || c == '8'),
                /* Spock */     serial.Count(c => c == '4' || c == '7')
            )
        );

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

    private void HandlePress(int index, Transform obj)
    {
        if (_mustPress == null || !_pressed.Add(index))
            return;

        obj.localPosition = new Vector3(obj.localPosition.x, 0.014f, obj.localPosition.z);

        if (!_mustPress.Contains(index))
        {
            obj.GetComponent<MeshRenderer>().material = MatWrong;
            Debug.LogFormat("[Rock-Paper-Scissors-Lizard-Spock #{1}] {0} is wrong.", _names[index], _moduleId);
            Module.HandleStrike();
        }
        else
        {
            obj.GetComponent<MeshRenderer>().material = MatCorrect;
            var solved = false;
            if (!_mustPress.Except(_pressed).Any())
            {
                solved = true;
                Module.HandlePass();
            }
            Debug.LogFormat("[Rock-Paper-Scissors-Lizard-Spock #{1}] {0} is correct.{2}", _names[index], _moduleId, solved ? " Module solved." : "");
        }
    }
}
