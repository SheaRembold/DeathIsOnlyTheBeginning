using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour
{
    [SerializeField]
    GameObject uiRoot;
    [SerializeField]
    Image portrait;
    [SerializeField]
    Text text;

    public static ConversationManager Instance;

    public bool IsActive { get { return uiRoot.activeSelf; } }

    ConversationData conversation;
    System.Action onFinished;
    int line;

    void Awake()
    {
        Instance = this;
        uiRoot.SetActive(false);
    }

    public void StartConversation(ConversationData conversation, System.Action onFinished)
    {
        this.conversation = conversation;
        this.onFinished = onFinished;
        line = 0;
        uiRoot.SetActive(true);
        UpdateLine();
    }

    void UpdateLine()
    {
        portrait.sprite = conversation.lines[line].character.Icon;
        text.text = conversation.lines[line].text;
    }

    public void NextLine()
    {
        line++;
        if (line < conversation.lines.Length)
        {
            UpdateLine();
        }
        else
        {
            uiRoot.SetActive(false);
            if (onFinished != null)
                onFinished();
        }
    }
}
