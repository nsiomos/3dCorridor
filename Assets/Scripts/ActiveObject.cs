using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveObject : MonoBehaviour
{
    public enum Owner { None /*For in-game newly instantiated objects*/, Neutral, Player, Enemy}
    public enum CollisionType { Object, KineticWeapon, HeatWeapon}

    public const int ObjectMaxCollisionDamagePerTick = 100;

    public Owner owner = Owner.None;
    public CollisionType collisionType = CollisionType.Object;

    public int hp = 100;

    private bool IsCollision(ActiveObject otherActiveObject)
    {
        return (owner == Owner.Player && otherActiveObject.owner == Owner.Enemy)
            || ((owner == Owner.Player || owner == Owner.Enemy) && otherActiveObject.owner == Owner.Neutral);
    }

    private void KineticWeaponCollidesWithObject(ActiveObject otherObject)
    {
        int damageToOther = hp;
        int damageFromOther = otherObject.hp;
        otherObject.hp -= damageToOther;
        hp -= damageFromOther;
    }

    private void ObjectCollidesWithObject(ActiveObject otherObject)
    {
        int damageToOther = Mathf.Min(hp, ObjectMaxCollisionDamagePerTick);
        int damageFromOther = Mathf.Min(otherObject.hp, ObjectMaxCollisionDamagePerTick);
        otherObject.hp -= damageToOther;
        hp -= damageFromOther;
    }

    private void OnTriggerEnter(Collider other)
    {
        ActiveObject otherActiveObject = other.GetComponent<ActiveObject>();
        if (otherActiveObject == null || !IsCollision(otherActiveObject))
        {
            return;
        }

        switch (collisionType)
        {
            case CollisionType.KineticWeapon:
                if (otherActiveObject.collisionType == CollisionType.Object)
                {
                    KineticWeaponCollidesWithObject(otherActiveObject);
                }
                break;
            case CollisionType.Object:
                if (otherActiveObject.collisionType == CollisionType.Object)
                {
                    ObjectCollidesWithObject(otherActiveObject);
                }
                break;
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
