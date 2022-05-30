using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConversationData", menuName = "Data/ConversationData")]
public class ConversationData : ScriptableObject
{
    [System.Serializable]
    public class Line
    {
        public CharacterData character;
        public string text;
    }

    public Line[] lines;
}
