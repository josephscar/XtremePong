using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHand : MonoBehaviour
{
    public Side owner = Side.Left;    // set Left on player paddle, Right on AI paddle
    public int maxCards = 6;

    private readonly List<CardData> hand = new();
    public IReadOnlyList<CardData> Cards => hand;

    public bool TryAdd(CardData card)
    {
        if (hand.Count >= maxCards) return false;
        hand.Add(card);
        // TODO: fire UI update event here
        return true;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null || hand.Count == 0) return;

        if (kb.digit1Key.wasPressedThisFrame) PlayIndex(0);
        else if (kb.digit2Key.wasPressedThisFrame) PlayIndex(1);
        else if (kb.digit3Key.wasPressedThisFrame) PlayIndex(2);
        else if (kb.digit4Key.wasPressedThisFrame) PlayIndex(3);
        else if (kb.digit5Key.wasPressedThisFrame) PlayIndex(4);
        else if (kb.digit6Key.wasPressedThisFrame) PlayIndex(5);
    }

    void PlayIndex(int idx)
    {
        Debug.Log($"Attempted PlayIndex = {idx}");
        if (idx < 0 || idx >= hand.Count) return;
        var card = hand[idx];
        hand.RemoveAt(idx);
        CardEffectSystem.Instance.Apply(owner, card);
        // TODO: fire UI update event here
    }
}
