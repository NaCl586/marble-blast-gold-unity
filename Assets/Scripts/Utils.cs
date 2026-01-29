using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Utils
{
    public static string FormatTime(float time)
    {
        TimeSpan ts = TimeSpan.FromMilliseconds(time);
        if (time == -1)
            return "99:59.999";
        else
            return string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
    }

    // Optional aliases for common names
    private static readonly Dictionary<string, KeyCode> Aliases = new Dictionary<string, KeyCode>()
    {
        { "Ctrl", KeyCode.LeftControl },
        { "Control", KeyCode.LeftControl },
        { "Left Control", KeyCode.LeftControl },
        { "Right Control", KeyCode.RightControl },

        { "Shift", KeyCode.LeftShift },
        { "Left Shift", KeyCode.LeftShift },
        { "Right Shift", KeyCode.RightShift },

        { "Alt", KeyCode.LeftAlt },
        { "Left Alt", KeyCode.LeftAlt },
        { "Right Alt", KeyCode.RightAlt },

        { "Escape", KeyCode.Escape },

        { "Return", KeyCode.Return },

        { "Delete", KeyCode.Delete },

        { "Backspace", KeyCode.Backspace },
        { "Space", KeyCode.Space },

        { "Page Up", KeyCode.PageUp },
        { "Page Down", KeyCode.PageDown },

        // Mouse buttons
        { "Mouse0", KeyCode.Mouse0 },
        { "Mouse1", KeyCode.Mouse1 },
        { "Mouse2", KeyCode.Mouse2 },
        { "Mouse3", KeyCode.Mouse3 },
        { "Mouse4", KeyCode.Mouse4 },
        { "Mouse5", KeyCode.Mouse5 },
        { "Mouse6", KeyCode.Mouse6 },

        { "Left Mouse Button", KeyCode.Mouse0 },
        { "Right Mouse Button", KeyCode.Mouse1 },
        { "Middle Mouse Button", KeyCode.Mouse2 },

        { "LMB", KeyCode.Mouse0 },
        { "RMB", KeyCode.Mouse1 },
        { "MMB", KeyCode.Mouse2 },

        { "Left", KeyCode.LeftArrow },
        { "Right", KeyCode.RightArrow },
        { "Up", KeyCode.UpArrow },
        { "Down", KeyCode.DownArrow }
    };

    public static bool TryParseKeyCode(string input, out KeyCode keyCode)
    {
        keyCode = KeyCode.None;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        // Normalize
        string normalized = input
            .Trim();

        // Alias check
        if (Aliases.TryGetValue(normalized, out keyCode))
            return true;

        // Direct enum parsing
        foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
        {
            if (kc.ToString().ToUpperInvariant() == normalized)
            {
                keyCode = kc;
                return true;
            }
        }

        return false;
    }

    public static KeyCode ParseKeyCode(string input)
    {
        if (TryParseKeyCode(input, out var key))
            return key;

        Debug.LogWarning($"Unknown KeyCode: \"{input}\"");
        return KeyCode.None;
    }

    private static readonly Dictionary<KeyCode, string> DisplayNames =
        new Dictionary<KeyCode, string>()
    {
        { KeyCode.LeftControl,  "Left Ctrl" },
        { KeyCode.RightControl, "Right Ctrl" },

        { KeyCode.LeftShift,  "Left Shift" },
        { KeyCode.RightShift, "Right Shift" },

        { KeyCode.LeftAlt,  "Left Alt" },
        { KeyCode.RightAlt, "Right Alt" },

        { KeyCode.Return, "Enter" },
        { KeyCode.Escape, "Esc" },
        { KeyCode.Backspace, "Backspace" },
        { KeyCode.Delete, "Delete" },
        { KeyCode.Space, "Space" },

        { KeyCode.PageUp, "Page Up" },
        { KeyCode.PageDown, "Page Down" },

        // Mouse buttons
        { KeyCode.Mouse0, "Left Mouse Button" },
        { KeyCode.Mouse1, "Right Mouse Button" },
        { KeyCode.Mouse2, "Middle Mouse Button" },
        { KeyCode.Mouse3, "Mouse 3" },
        { KeyCode.Mouse4, "Mouse 4" },
        { KeyCode.Mouse5, "Mouse 5" },
        { KeyCode.Mouse6, "Mouse 6" },

        // Arrow keys
        { KeyCode.LeftArrow,  "Left" },
        { KeyCode.RightArrow, "Right" },
        { KeyCode.UpArrow,    "Up" },
        { KeyCode.DownArrow,  "Down" },
    };

    public static string KeyCodeToString(KeyCode keyCode)
    {
        if (keyCode == KeyCode.None)
            return string.Empty;

        if (DisplayNames.TryGetValue(keyCode, out var display))
            return display;

        // Default enum name fallback
        string name = keyCode.ToString();

        // Alpha keys: Alpha0 → 0
        if (name.StartsWith("Alpha"))
            return name.Substring(5);

        // Keypad keys: Keypad1 → Keypad 1
        if (name.StartsWith("Keypad"))
            return "Keypad " + name.Substring(6);

        // Split camel case: LeftBracket → Left Bracket
        return SplitCamelCase(name);
    }

    private static string SplitCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(input[0]);

        for (int i = 1; i < input.Length; i++)
        {
            char c = input[i];
            if (char.IsUpper(c))
                sb.Append(' ');
            sb.Append(c);
        }

        return sb.ToString();
    }

    private static readonly Dictionary<string, System.Func<ControlBinding, KeyCode>> BindLookup =
        new Dictionary<string, System.Func<ControlBinding, KeyCode>>()
    {
        { "moveleft",   cb => cb.moveLeft },
        { "moveright",  cb => cb.moveRight },
        { "moveforward",cb => cb.moveForward },
        { "movebackward",cb => cb.moveBackward },

        { "jump",       cb => cb.jump },
        { "mousefire",  cb => cb.usePowerup },

        { "panup",      cb => cb.rotateCameraUp },
        { "pandown",    cb => cb.rotateCameraDown },
        { "turnleft",   cb => cb.rotateCameraLeft },
        { "turnright",  cb => cb.rotateCameraRight },

        { "freelook",   cb => cb.freelookKey }
    };

    private static readonly Regex BindRegex =
        new Regex(@"<func:bind\s+([a-zA-Z0-9_]+)>", RegexOptions.IgnoreCase);

    public static string Resolve(string input)
    {
        if (ControlBinding.instance == null)
            return input;

        return BindRegex.Replace(input, match =>
        {
            string bindName = match.Groups[1].Value.ToLowerInvariant();

            if (!BindLookup.TryGetValue(bindName, out var getter))
                return match.Value; // leave unchanged if unknown

            KeyCode key = getter(ControlBinding.instance);
            return KeyCodeToString(key);
        });
    }

    private const float Aspect16by9 = 16f / 9f;
    private const float AspectTolerance = 0.01f;

    public static List<Resolution> Get16by9Resolutions(bool clampToMonitor = true)
    {
        Resolution max = Screen.currentResolution;

        return Screen.resolutions
            .Where(r =>
            {
                float aspect = (float)r.width / r.height;
                if (Mathf.Abs(aspect - Aspect16by9) > AspectTolerance)
                    return false;

                if (clampToMonitor &&
                    (r.width > max.width || r.height > max.height))
                    return false;

                return true;
            })
            // Remove refresh-rate duplicates
            .GroupBy(r => (r.width, r.height))
            .Select(g => g.First())
            // Sort ascending
            .OrderBy(r => r.width)
            .ThenBy(r => r.height)
            .ToList();
    }
}
