using UnityEngine;

public class SkyboxScroll : MonoBehaviour
{
    public float velocity;
    private MeshRenderer meshRenderer;
    private float offset;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void FixedUpdate()
    {
        offset += velocity * Time.fixedDeltaTime;
        meshRenderer.material.SetTextureOffset ("_MainTex", new Vector2(offset,0));
    }
}
