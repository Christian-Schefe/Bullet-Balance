using System.Collections;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class GoldSpawner : MonoBehaviour
{
    [SerializeField] private Arena arena;
    [SerializeField] private GameObject goldPrefab;

    [SerializeField] private float pickupDistance;
    [SerializeField] private float goldRadius;
    [SerializeField] private int maxGoldCount;
    [SerializeField] private AudioClip collectGoldSound;

    private Timer timer;

    private Dictionary<Vector2, GameObject> goldPositions;

    private void Awake()
    {
        goldPositions = new();
        timer = new RandomTimer(5, 10);
        timer.action = (_) => SpawnGold();
    }

    public void Tick(float time)
    {
        timer.Tick(time);

        var playerPos = arena.Player.Position;
        var goldToRemove = new List<Vector2>();
        foreach (var pos in goldPositions.Keys)
        {
            if (Vector2.Distance(playerPos, pos) < pickupDistance)
            {
                var gameObject = goldPositions[pos];
                this.TweenScale(goldPositions[pos].transform).To(Vector3.zero).Duration(0.3f).Ease(Easing.CubicIn).OnFinally(() => Destroy(gameObject)).RunNew();
                goldToRemove.Add(pos);
                arena.CollectGold(1);

                SfxSystem.PlaySfx(collectGoldSound);
            }
        }

        foreach (var pos in goldToRemove)
        {
            goldPositions.Remove(pos);
        }
    }

    private void SpawnGold()
    {
        var pos = arena.RandomPosition(goldRadius);

        var instance = Instantiate(goldPrefab, pos, Quaternion.identity);
        this.TweenScale(instance.transform).From(Vector3.zero).To(Vector3.one).Duration(0.3f).Ease(Easing.CubicOut).RunNew();
        goldPositions.Add(pos, instance);
    }
}
