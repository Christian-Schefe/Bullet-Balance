using Tweenables;
using Tweenables.Core;
using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float animationSpeed;

    [SerializeField] private EntityHealthbar healthbar;
    [SerializeField] private DamageNumber damageNumberPrefab;

    private TweenRunner runner;
    private Sprite[] sprites;
    private int spriteIndex;

    private float timeSinceSpriteChange;

    public EntityHealthbar Healthbar => healthbar;

    public Color Color
    {
        get => sprite.color;
        set => sprite.color = value;
    }

    public Vector2 Pos
    {
        get => transform.position;
        set => transform.position = value;
    }

    private void Update()
    {
        if (sprites == null || sprites.Length == 0)
        {
            return;
        }
        timeSinceSpriteChange += Time.deltaTime;
        if (timeSinceSpriteChange >= animationSpeed)
        {
            timeSinceSpriteChange -= animationSpeed;
            spriteIndex = (spriteIndex + 1) % sprites.Length;
            sprite.sprite = sprites[spriteIndex];
        }
    }

    public void SetSpriteAnimation(Sprite[] sprites)
    {
        this.sprites = sprites;
        sprite.sprite = sprites[0];
        spriteIndex = 0;
    }

    public void AnimateTakeDamage(int damage)
    {
        var damageNumber = Instantiate(damageNumberPrefab, transform);
        damageNumber.DisplayDamage(transform, damage, false);
    }

    public void AnimateMove(Vector3 to, float delay, bool instant)
    {
        if (instant)
        {
            transform.position = to;
            return;
        }
        this.TweenPosition().To(to).Delay(delay).Duration(0.5f).Ease(Easing.CubicInOut).RunQueued(ref runner);
    }

    public void AnimateDestroy()
    {
        this.TweenScale().To(Vector3.zero).Duration(0.5f).Ease(Easing.CubicIn).OnComplete(() => Destroy(gameObject)).RunNew();
    }

    public void AnimateSpawn(float delay)
    {
        transform.localScale = Vector3.zero;
        this.TweenScale().From(Vector3.zero).To(Vector3.one).Delay(delay).Duration(0.5f).Ease(Easing.CubicOut).RunNew();
    }

    public void AnimateMeleeAttack(Vector3 to)
    {
        this.TweenPosition().To(to).Duration(0.3f).Ease(Easing.CubicInOut).PingPong(2).RunQueued(ref runner);
    }

    public void AnimateRangedAttack(Transform target, Vector3 to, System.Action onFinish)
    {
        this.TweenPosition(target).To(to).Duration(0.3f).Ease(Easing.CubicIn).OnFinally(onFinish).RunQueued(ref runner);
    }
}
