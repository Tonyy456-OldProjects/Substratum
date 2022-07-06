using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    [SerializeField] Sprite leftSprite;
    [SerializeField] Sprite rightSprite;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float threshold = 0.01f;
        if(body.velocity.x > threshold)
        {
            sprite.sprite = rightSprite;
        } else if (body.velocity.x < -threshold)
        {
            sprite.sprite = leftSprite;
        }
    }
}
