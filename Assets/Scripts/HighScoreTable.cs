using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class HighScoreTable : MonoBehaviour
{
    [SerializeField] private Transform entryContainer;
    [SerializeField] private Transform entryTemplate;
    private List<HighScoreEntry> HighScoreEntriesList;
    private List<Transform> highScoreTransformList;

    private void Awake()
    {
        entryTemplate.gameObject.SetActive(false);
        HighScoreEntriesList = new List<HighScoreEntry>()
        {
            new HighScoreEntry { timer = 23.23456f, name = "Dvir" },
            new HighScoreEntry { timer = 27.27566f, name = "Raz" },
            new HighScoreEntry { timer = 34.34f, name = "Or" },
            new HighScoreEntry { timer = 17.43728f, name = "Sonny" },
            new HighScoreEntry { timer = 30.23456f, name = "Geula" },
            new HighScoreEntry { timer = 31.23456f, name = "Amit" },
        };

        for (int i = 0; i < HighScoreEntriesList.Count; i++)
        {
            for (int j = 0; j < HighScoreEntriesList.Count; j++)
            {
                if (HighScoreEntriesList[j].timer >= HighScoreEntriesList[i].timer)
                {
                    HighScoreEntry tmp = HighScoreEntriesList[i];
                    HighScoreEntriesList[i] = HighScoreEntriesList[j];
                    HighScoreEntriesList[j] = tmp;
                }
            }
        }
        highScoreTransformList = new List<Transform>();
        foreach (HighScoreEntry highScoreEntry in HighScoreEntriesList)
        {
            CreateHighScoreEntryTransform(highScoreEntry, entryContainer, highScoreTransformList);
        }

    }

    private void CreateHighScoreEntryTransform(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 30f;
        int index = transformList.Count;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * index);
        entryTransform.gameObject.SetActive(true);

        int rank = index + 1;
        string rankString;
        rankString = rank + ".";
        entryTransform.Find("PosText").GetComponent<Text>().text = rankString;
        string name = highScoreEntry.name;
        entryTransform.Find("NameText").GetComponent<Text>().text = name;
        float timer = highScoreEntry.timer;
        TimeSpan timeSpan = TimeSpan.FromSeconds(timer);
        string formattedTime = string.Format("{0}:{1:00}", (int)timeSpan.TotalSeconds, timeSpan.TotalMilliseconds%1000);
        entryTransform.Find("TimeText").GetComponent<Text>().text = formattedTime;
        
        transformList.Add(entryTransform);
    }

    private class HighScoreEntry
    {
        public float timer;
        public string name;

    }
}
