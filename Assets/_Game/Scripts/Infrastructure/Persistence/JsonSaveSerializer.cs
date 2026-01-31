namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public class JsonSaveSerializer : ISaveSerializer
    {
        public T Deserialize<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize<T>(T data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }
    }
}
