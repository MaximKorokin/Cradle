namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public interface ISaveStorage
    {
        string LoadText(string key);
        void SaveText(string key, string value);
    }
}