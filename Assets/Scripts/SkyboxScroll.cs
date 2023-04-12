using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxScroll : MonoBehaviour
{
    public float velocity;
    private MeshRenderer renderer;
    private float offset;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    void FixedUpdate()
    {
        offset += velocity * Time.fixedDeltaTime;
        renderer.material.SetTextureOffset ("_MainTex", new Vector2(offset,0));
    }
}
