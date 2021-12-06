using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaAmmo : MonoBehaviour
{
    public AudioClip collectedAmmo;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeAmmo(6);
            Destroy(gameObject);

            controller.PlaySound(collectedAmmo);
        }
    }

}