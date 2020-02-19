using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OutfitZone : MonoBehaviour
{
    public int outfit_override;

    public void Start()
    {
        if (gameObject.layer != 20)
            gameObject.layer = 20;
    }
}
