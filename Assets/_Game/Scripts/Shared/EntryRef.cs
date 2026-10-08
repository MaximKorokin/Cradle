using System;

namespace Assets._Game.Scripts.Shared
{
    /// <summary>
    /// Immutable snapshot of a repository entry reference.
    /// Id == null: not bound. Id != null and Exists: alive. Id != null and !Exists: removed (invalid).
    /// </summary>
    public readonly struct EntryRef : IEquatable<EntryRef>
    {
        public string Id { get; }
        public bool Exists { get; }

        public EntryRef(string id, bool exists)
        {
            Id = id;
            Exists = exists;
        }

        public bool IsBound => Id != null;

        public bool Equals(EntryRef other) => Id == other.Id && Exists == other.Exists;
        public override bool Equals(object obj) => obj is EntryRef other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Id, Exists);

        public static bool operator ==(EntryRef left, EntryRef right) => left.Equals(right);
        public static bool operator !=(EntryRef left, EntryRef right) => !left.Equals(right);
    }
}
