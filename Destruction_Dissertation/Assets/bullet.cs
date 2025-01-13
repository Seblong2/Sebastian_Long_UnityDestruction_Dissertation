using UnityEngine;

public class bullet : MonoBehaviour
{
    public float lifetime = 1;

    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    
}
