namespace RainbowUtlitiesTests
{
    public readonly struct Update
    {
        public Update(int id, int ownerId)
        {
            Id = id;
            OwnerId = ownerId;
        }

        public int Id { get; }

        public int OwnerId { get; }

        public override string ToString()
        {
            return $"Update id {Id}, from {OwnerId}";
        }
    }
}
