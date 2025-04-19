[System.Serializable]
public class PlayerSaveData
{
    public float Xpos;
    public float Ypos;

    public PlayerSaveData(PlayerController player)
    {
        Xpos = player.transform.position.x;
        Ypos = player.transform.position.y;
    }
}
