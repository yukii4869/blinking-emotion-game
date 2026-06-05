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
    Interact,        // startet Interaktion (GambleGuest)
    WaitingForCard,  // Spieler soll Karte wählen
    WaitingForEyes,  // Spieler soll Augen schließen
    Resolve,          // 50/50 Outcome
    Wander,
    Attack,
    Stunned,
    GoingHome,
    Approach
}
