using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public UnitScript unit_scp;
    

    private void Awake()
    {
        unit_scp = GetComponent<UnitScript>();
        unit_scp.SetIsPlayer(true);
    }

    public void Move(float x, float y)
    {
        unit_scp.Move(x, y);
    }
}
