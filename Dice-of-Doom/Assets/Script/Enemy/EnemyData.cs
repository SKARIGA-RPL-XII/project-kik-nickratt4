using System;

[Serializable]
public class EnemyListResponse
{
    public string status;
    public int player_level;
    public EnemyItem[] enemies;
}

[Serializable]
public class EnemyItem
{
    public int enemy_id;
    public string name;
    public int hp;
    public int damage;
    public int is_boss;
    public int level_game;
}
