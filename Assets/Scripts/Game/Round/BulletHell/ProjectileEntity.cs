using System.Collections;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class ProjectileEntity : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D hitbox;

    private Vector2 dir;
    private float radius;

    public Sprite Sprite
    {
        get => sprite.sprite;
        set => sprite.sprite = value;
    }

    public Color Color
    {
        get => sprite.color;
        set => sprite.color = value;
    }

    public Vector2 Dir
    {
        get => dir;
        set => SetDir(value);
    }

    public float Radius
    {
        get => radius;
        set => SetRadius(value);
    }

    public Vector2 Pos
    {
        get => transform.position;
        set => transform.position = value;
    }

    public void SetDir(Vector2 dir)
    {
        this.dir = (dir == Vector2.zero) ? Vector2.up : dir;
        var angle = -Vector2.SignedAngle(dir, Vector2.up);
        transform.rotation = Quaternion.Euler(new(0, 0, angle));
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
        transform.localScale = 2 * radius * Vector3.one;
    }

    public void AnimateDestroy(float duration, bool instant)
    {
        if (instant)
        {
            Destroy(gameObject);
            return;
        }
        this.TweenScale().To(Vector3.zero).Duration(duration).Ease(Easing.CubicIn).OnComplete(() => Destroy(gameObject)).RunNew();
    }

    public bool IsCollidingWith(Collider2D other)
    {
        return hitbox.IsTouching(other);
    }

    public bool IsOutside(Arena arena)
    {
        return arena.IsFullyOutside(Pos, radius);
    }
}
