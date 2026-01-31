namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public interface ISaveSerializer
    {
        T Deserialize<T>(string json);
        string Serialize<T>(T data);
    }
}