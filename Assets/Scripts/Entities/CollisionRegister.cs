using UnityEngine;

//This is a generic class to detect if the player collided
//Useful to detect if the player was hit by weapons or targets
public class CollisionRegister : MonoBehaviour
{
    public bool playerCollided;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") playerCollided = true;
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player") playerCollided = false;
    }
}
