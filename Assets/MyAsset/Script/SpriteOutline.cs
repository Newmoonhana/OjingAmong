using UnityEngine;


[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour
{
    public Color color = Color.white;

    [Range(0, 16)]
    public int outlineSize = 1;

    private SpriteRenderer spriteRenderer;

    //tmp
    MaterialPropertyBlock tmp_Mpb;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateOutline(true);
    }

    void OnDisable()
    {
        UpdateOutline(false);
    }

    void Update()
    {
        UpdateOutline(true);
    }

    void UpdateOutline(bool outline)
    {
        tmp_Mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(tmp_Mpb);
        tmp_Mpb.SetFloat("_Outline", outline ? 1f : 0);
        tmp_Mpb.SetColor("_OutlineColor", color);
        tmp_Mpb.SetFloat("_OutlineSize", outlineSize);
        spriteRenderer.SetPropertyBlock(tmp_Mpb);
    }
}