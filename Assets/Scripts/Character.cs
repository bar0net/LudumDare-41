using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [Header("Atributes")]
    public int health = 2;
    public int armour = 2;
    public float damageDelay = 0.5f;

    protected int currArmour = 1;
    protected float dmgTimer = 0.0f;

    SpriteRenderer[] _sr;

    protected virtual void Start()
    {
        _sr = GetComponentsInChildren<SpriteRenderer>();
        currArmour = armour;
    }

    private void Update()
    {
        if (dmgTimer > 0 && health > 0)
        {
            dmgTimer -= Time.deltaTime;

            // Change the alpha of the character to reflect that it is invulnerable
            // because it is recovering from a recent hit
            if (dmgTimer <= 0) PaintCharacter(1);
            else PaintCharacter(0.6f);
        }
    }


    public virtual void Hit(int damage, bool healthOnly = false, float x = 0, float y = 0)
    {
        // Exit if the Hit can only damage health points and
        // the character has armour
        if (healthOnly && currArmour > 0) return;

        // Exit if the character is recovering from a previous hit
        if (dmgTimer > 0) return;

        // Start the recovery timer
        dmgTimer = damageDelay;

        // If the armour can fully absorb the impact, damage the armour and exit
        if (currArmour >= damage)
        {
            currArmour -= damage;
            damage = 0;
            return;
        }

        // Armour absorbs part of the damage
        damage -= currArmour;
        currArmour = 0;

        // Health absorbs the rest of the damage
        health -= damage;
        if (health <= 0) Die();
    }
    
    void PaintCharacter(float alpha)
    {
        for (int i = 0; i < _sr.Length; i++)
        {
            Color c = _sr[i].color;
            _sr[i].color = new Color(c.r,c.g,c.b,alpha);
        }
    }

    protected virtual void Die() { }
}
