using UnityEngine;

public class cubeScript : MonoBehaviour , SelectionInterface{
    public GameObject Node;
    private MeshRenderer MeshR;
    private Vector2Int position;

    private void Start()
    {
        if (MeshR == null)
        {
            MeshR = GetComponent<MeshRenderer>();
            if (MeshR == null) Debug.Log("cube missing meshrenderer");
        }
    }
    public void selected(Color C)
    {
        //Debug.Log(C);
        if (MeshR != null) { 
        MeshR.material.EnableKeyword("_EMISSION");
            MeshR.material.SetColor("_EmissionColor", C);
        }
        else Debug.Log("no MeshRenderer");
    }

    public void deselected()
    {
        if (MeshR != null)
        {
            MeshR.material.DisableKeyword("_EMISSION");
        }
        else Debug.Log("no MeshRenderer");
    }
    
    public Vector2Int getPosition()
    {
        return position;
    }
    public void SetPosition(int x, int y) { SetPosition(new Vector2Int(x, y));}
    public void SetPosition(Vector2Int pos) { position = pos; }
}
