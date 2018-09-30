public class Global : MonoSingleton<Global>
{
    public PlayerController PlayerController;

    void Awake()
    {
        PlayerController = gameObject.GetComponent<PlayerController>();
    }
}
