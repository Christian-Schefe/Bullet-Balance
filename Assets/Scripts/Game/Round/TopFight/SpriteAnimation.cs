using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float animationSpeed;
    [SerializeField] private List<Sprite> sprites;

    private int spriteIndex;
    private float timeSinceSpriteChange;

    private void Update()
    {
        if (sprites == null || sprites.Count == 0)
        {
            return;
        }
        timeSinceSpriteChange += Time.deltaTime;
        if (timeSinceSpriteChange >= animationSpeed)
        {
            timeSinceSpriteChange -= animationSpeed;
            spriteIndex = (spriteIndex + 1) % sprites.Count;
            sprite.sprite = sprites[spriteIndex];
        }
    }
}
