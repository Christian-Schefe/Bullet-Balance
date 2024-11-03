using System.Collections;
using System.Collections.Generic;
using Tweenables;
using Tweenables.Core;
using UnityEngine;

public class Player : MonoBehaviour, ITickable
{
    [SerializeField] private float speed;
    [SerializeField] private float radius;
    [SerializeField] private Arena arena;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color idleColor, hitColor;

    private Vector2 velocity;
    private Vector2 acceleration;

    public Vector2 Position => transform.position;
    public Collider2D Collider => collider2d;

    private TweenRunner tweenRunner;

    private void Start()
    {
        velocity = Vector2.zero;
        acceleration = Vector2.zero;
        sprite.color = idleColor;
    }

    public void Tick(float time)
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        var direction = new Vector2(horizontal, vertical);
        if (direction.magnitude > 1)
        {
            direction.Normalize();
        }

        velocity = Vector2.SmoothDamp(velocity, direction, ref acceleration, 0.1f);

        var pos = (Vector2)transform.position + speed * Time.deltaTime * velocity;

        transform.position = arena.Constrain(pos, radius);
    }

    public void OnHit()
    {
        this.TweenAny<Color>().Use(c => sprite.color = c).From(hitColor).To(idleColor).Ease(Easing.Linear).Duration(0.1f).RunImmediate(ref tweenRunner);
    }
}
