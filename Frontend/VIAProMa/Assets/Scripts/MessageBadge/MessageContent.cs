using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New message", menuName = "MessageBadge Content", order = 51)]
public class MessageContent : ScriptableObject
{
    [SerializeField] private long code;
    [SerializeField] private Sprite icon;
    [SerializeField] private string text;

    public long Code { get => code; set => code = value; }
    public Sprite Icon { get => icon; }
    public string Text { get => text; }
}