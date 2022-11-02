using UnityEngine;

public class TextOutline : MonoBehaviour
{
    public float pixelSize = 1;
    public Color outlineColor = Color.black;
    public bool resolutionDependant = false;
    public int doubleResolution = 1024;

    private TextMesh textMesh;
    private MeshRenderer meshRenderer;

    //tmp
    GameObject tmp_Obj;
    MeshRenderer tmp_Mr;
    Vector3 tmp_V, tmp_V2, tmp_V3;
    TextMesh tmp_Tm;
    bool tmp_bool;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        meshRenderer = GetComponent<MeshRenderer>();

        for (int i = 0; i < 8; i++)
        {
            tmp_Obj = new GameObject("outline", typeof(TextMesh));
            tmp_Obj.transform.parent = transform;
            tmp_Obj.transform.localScale = new Vector3(1, 1, 1);

            tmp_Mr = tmp_Obj.GetComponent<MeshRenderer>();
            tmp_Mr.material = new Material(meshRenderer.material);
            tmp_Mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tmp_Mr.receiveShadows = false;
            tmp_Mr.sortingLayerID = meshRenderer.sortingLayerID;
            tmp_Mr.sortingLayerName = meshRenderer.sortingLayerName;
        }
    }

    void LateUpdate()
    {
        tmp_V = Camera.main.WorldToScreenPoint(transform.position);

        outlineColor.a = textMesh.color.a * textMesh.color.a;

        // copy attributes
        for (int i = 0; i < transform.childCount; i++)
        {
            tmp_Tm = transform.GetChild(i).GetComponent<TextMesh>();
            tmp_Tm.color = outlineColor;
            tmp_Tm.text = textMesh.text;
            tmp_Tm.alignment = textMesh.alignment;
            tmp_Tm.anchor = textMesh.anchor;
            tmp_Tm.characterSize = textMesh.characterSize;
            tmp_Tm.font = textMesh.font;
            tmp_Tm.fontSize = textMesh.fontSize;
            tmp_Tm.fontStyle = textMesh.fontStyle;
            tmp_Tm.richText = textMesh.richText;
            tmp_Tm.tabSize = textMesh.tabSize;
            tmp_Tm.lineSpacing = textMesh.lineSpacing;
            tmp_Tm.offsetZ = textMesh.offsetZ + 1;

            tmp_bool = resolutionDependant && (Screen.width > doubleResolution || Screen.height > doubleResolution);
            tmp_V2 = GetOffset(i) * (tmp_bool ? 2.0f * pixelSize : pixelSize);
            tmp_V3 = Camera.main.ScreenToWorldPoint(tmp_V + tmp_V2);
            tmp_Tm.transform.position = tmp_V3;

            tmp_Mr = transform.GetChild(i).GetComponent<MeshRenderer>();
            tmp_Mr.sortingLayerID = meshRenderer.sortingLayerID;
            tmp_Mr.sortingLayerName = meshRenderer.sortingLayerName;
        }
    }

    Vector3 GetOffset(int i)
    {
        switch (i % 8)
        {
            case 0: return new Vector3(0, 1, 0);
            case 1: return new Vector3(1, 1, 0);
            case 2: return new Vector3(1, 0, 0);
            case 3: return new Vector3(1, -1, 0);
            case 4: return new Vector3(0, -1, 0);
            case 5: return new Vector3(-1, -1, 0);
            case 6: return new Vector3(-1, 0, 0);
            case 7: return new Vector3(-1, 1, 0);
            default: return Vector3.zero;
        }
    }
}


//출처: https://bbangdeveloper.tistory.com/entry/TextMesh로-outline-Text-표시 [빵개발연구소]
