using UnityEngine;

public enum CardKind { SpeedBoost, ShieldWall, TimeSlow }

[CreateAssetMenu(fileName = "Card_", menuName = "XtremePong/Card")]
public class CardData : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public CardKind kind;
    [Min(0)] public float value = 1f;      // e.g., speed multiplier or slow scale
    [Min(0)] public float duration = 1.5f; // seconds for effects that persist
}
