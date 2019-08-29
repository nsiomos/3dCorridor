using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : ActiveObject
{
    private Vector3 startPosition;

    public float range;
    public float speed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        startPosition = transform.position;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, startPosition) > range)
        {
            Destroy(gameObject);
        }
    }
}
