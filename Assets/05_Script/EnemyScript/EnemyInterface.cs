public interface IEnemyInterface
{
    public int GroupNumber { get; set; }
    public LR LR { get; set; }
    public bool Leader { get; set; }
}
public enum EnemyType
{
    attacker,
    defender,
    supporter,
}