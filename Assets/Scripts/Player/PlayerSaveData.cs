[System.Serializable]
public class PlayerSaveData
{
    public float Xpos;
    public float Ypos;
    public bool dashEnabled;
    public bool doubleJumpEnabled;

    public PlayerSaveData(PlayerController player)
    {
        Xpos = player.transform.position.x;
        Ypos = player.transform.position.y;
        dashEnabled = player.dashEnabled;
        doubleJumpEnabled = player.doubleJumpEnabled;
    }
}