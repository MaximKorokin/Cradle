using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Persistence.DataTypes
{
    public struct SerializableVector2
    {
        public float X;
        public float Y;

        public SerializableVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator SerializableVector2(Vector2 vector)
        {
            return new SerializableVector2(vector.x, vector.y);
        }

        public static implicit operator Vector2(SerializableVector2 serializableVector)
        {
            return new Vector2(serializableVector.X, serializableVector.Y);
        }
    }
}
