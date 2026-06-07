/*public enum EnemyState
{
    Wander,
    Alert,
    Chase,
    Attack,
    Stunned
}*/
public enum EnemyState
{
    Idle,            // Standardzustand
    Alert,           // z.B. für NoiseGuest
    Chase,           // verfolgt den Spieler
    Search,          // sucht nach Spieler / Geräusch
    Attack,
    GoingHome,
    Approach,
    Special
}
