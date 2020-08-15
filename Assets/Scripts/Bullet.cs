using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private enum materialsType {BOUNCE, STATIC} 
    public PhysicsMaterial2D bounceMaterial;
    public PhysicsMaterial2D staticMaterial;

    // Start is called before the first frame update
    void Start()
    {
        changeMaterial(materialsType.BOUNCE);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void changeMaterial(materialsType type) 
    {
        switch (type) {
            case materialsType.BOUNCE :
                gameObject.GetComponent<CircleCollider2D>().sharedMaterial = bounceMaterial;
                break;
            default :
                gameObject.GetComponent<CircleCollider2D>().sharedMaterial = staticMaterial;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        changeMaterial(materialsType.STATIC);
        Debug.Log(trigger.tag);
        if (trigger.tag == "Bumper") {
            trigger.GetComponents<Bumper>()[0].bump(gameObject);
        }
    }
}
