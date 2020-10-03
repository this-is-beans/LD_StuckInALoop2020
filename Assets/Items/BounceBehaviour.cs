using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBehaviour : MonoBehaviour
{
    public Transform mainTransform;
    public Transform spriteTransform;

    public float currentBounceTime;
    public float currentYVelocity;
    public Vector2 direction;
    public bool dead;

    public float initialHeight;
    public float initialYVelocity;
    public float speed;
    public float gravity;
    public float bounciness;

    void Start()
    {
        enabled = false;
    }

    public void Throw(Vector2 dir)
    {
        enabled = true;
        dead = false;

        spriteTransform.localPosition = new Vector2(0, initialHeight);
        direction = dir;
        currentYVelocity = initialYVelocity;
    }

    public void Update()
    {
        if (dead)
        {
            return;
        }

        currentBounceTime += Time.deltaTime;

        float xShift = direction.x * Time.deltaTime * speed;
        float yShift = direction.y * Time.deltaTime * speed;
        transform.position = new Vector2(transform.position.x + xShift, transform.position.y + yShift);

        currentYVelocity += gravity * Time.deltaTime;

        spriteTransform.localPosition = new Vector2(spriteTransform.localPosition.x, spriteTransform.localPosition.y + (currentYVelocity * Time.deltaTime));
        //float currentY = transform.localPosition.y;

        if (spriteTransform.localPosition.y <= 0)
        {
            spriteTransform.localPosition = new Vector2(spriteTransform.localPosition.x, 0);
            currentYVelocity = -(currentYVelocity * bounciness);

            if (currentBounceTime > 0.05f)
            {
                currentBounceTime = 0;
                return;
            }
            Dead();
        }
    }

    public void Dead()
    {
        dead = true;
        //speed = 0;
        //GetComponent<BoxCollider2D>().enabled = false;
        enabled = false;
        //gameObject.layer = 8;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log ("Collided with " + collision.gameObject.name);
        direction = Vector2.zero;
    }
}
