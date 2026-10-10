namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public sealed class QuestObjectiveCodec : DataCodecBase<QuestObjectiveProgressData>
    {
        public override string Type => "QuestObjective";
    }

    public readonly struct QuestObjectiveProgressData
    {
        public readonly int CurrentAmount;
        public readonly object Payload;

        public QuestObjectiveProgressData(int currentAmount, object payload)
        {
            CurrentAmount = currentAmount;
            Payload = payload;
        }
    }
}
