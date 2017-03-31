﻿using UnityEngine;
using System.Collections;

// Next stage button is used by MoM to move between investigators and monsters
public class LogWindow
{
    public Dictionary<string, DialogBoxEditable> valueDBE;

    public bool developerToggle = false;

    // Construct and display
    public LogWindow()
    {
        // If a dialog window is open we force it closed (this shouldn't happen)
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("dialog"))
            Object.Destroy(go);
        Update();
    }

    public void Update(toggle = false)
    {
        developerToggle ^= toggle;
        Game game = Game.Get();
        game.logWindow = this;
        // white background because font rendering is broken
        string log = "";
        foreach (Quest.LogEntry e in game.quest.log)
        {
            log += e.GetEntry();
        }
        log.Trim('\n');
        if (Application.isEditor ^ developerToggle)
        {
            DialogBox db = new DialogBox(new Vector2(UIScaler.GetHCenter(-18f), 0.5f), new Vector2(20, 24.5f), log, Color.black, new Color(1, 1, 1, 0.9f));
        }
        else
        {
            DialogBox db = new DialogBox(new Vector2(UIScaler.GetHCenter(-14f), 0.5f), new Vector2(28, 24.5f), log, Color.black, new Color(1, 1, 1, 0.9f));
        }
        db.AddBorder();
        // This material works for the mask, but only renders in black
        db.textObj.GetComponent<UnityEngine.UI.Text>().material = (Material)Resources.Load("Fonts/FontMaterial");
        UnityEngine.UI.ScrollRect scrollRect = db.background.AddComponent<UnityEngine.UI.ScrollRect>();
        scrollRect.content = db.textObj.GetComponent<RectTransform>();
        scrollRect.horizontal = false;
        RectTransform textRect = db.textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(textRect.rect.width, db.textObj.GetComponent<UnityEngine.UI.Text>().preferredHeight);
        scrollRect.verticalNormalizedPosition = 0f;

        UnityEngine.UI.Mask mask = db.background.AddComponent<UnityEngine.UI.Mask>();

        new TextButton(new Vector2(UIScaler.GetHCenter(-3f), 25f), new Vector2(6, 2), "Close", delegate { Destroyer.Dialog(); });

        if (Application.isEditor ^ developerToggle)
        {
            DrawVarList();
        }
    }

    public void DrawVarList();
    {

        DialogBox db = new DialogBox(new Vector2(UIScaler.GetHCenter(2f), 0.5f), new Vector2(16, 24.5f), "");
        db.AddBorder();
        db.background.AddComponent<UnityEngine.UI.Mask>();
        UnityEngine.UI.ScrollRect scrollRect = db.background.AddComponent<UnityEngine.UI.ScrollRect>();

        GameObject scrollArea = new GameObject("scroll");
        RectTransform scrollInnerRect = scrollArea.AddComponent<RectTransform>();
        scrollArea.transform.parent = db.background.transform;
        scrollInnerRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 16f * UIScaler.GetPixelsPerUnit());
        scrollInnerRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 1);

        scrollRect.content = scrollInnerRect;
        scrollRect.horizontal = false;

        // List of vars
        float offset = 1;
        TextButton tb;
        valueDBE = new Dictionary<string, DialogBoxEditable>();
        foreach (KeyValuePair<string, float> kv in Game.Get().quest.vars.vars)
        {
            if (kv.Value != 0)
            {
                string key = q.Key;

                DialogBox db = new DialogBox(new Vector2(UIScaler.GetHCenter(2.5f), offset), new Vector2(12, 1.2f), key, Color.black, Color.white);
                db.textObj.GetComponent<UnityEngine.UI.Text>().material = (Material)Resources.Load("Fonts/FontMaterial");
                db.background.transform.parent = scrollArea.transform;
                db.AddBorder();

                DialogBoxEditable dbe = new DialogBoxEditable(new Vector2(UIScaler.GetHCenter(14.5f), offset), new Vector2(3, 1.2), kv.Value.ToString(), delegate { UpdateValue(key); }, Color.black, Color.white);
                dbe.textObj.GetComponent<UnityEngine.UI.Text>().material = (Material)Resources.Load("Fonts/FontMaterial");
                dbe.background.transform.parent = scrollArea.transform;
                dbe.AddBorder();
                valueDBE.Add(key, dbe);

                offset += 1.4;
            }
        }
        scrollInnerRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, (offset - 1) * UIScaler.GetPixelsPerUnit());
    }

    public void UpdateValue(string key)
    {
        float value;
        float.TryParse(valueDBE[key].uiInput.text, out value);
        Game.Get().quest.vars.SetVar(key, value);
        Destroyer.Dialog();
        Update();
    }
}
