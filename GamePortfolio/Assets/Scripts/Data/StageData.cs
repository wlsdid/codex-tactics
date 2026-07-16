using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum StageModifierType
{
    None = 0, TutorialField, PackPressure, Stoneguard, StormSurge, VoidDrain, RadiantTrial
}

[System.Serializable]
public class StageData
{
    public string stageName = "Stage 1-1";
    public string encounterName = "Ruins Patrol";
    [TextArea(2, 4)] public string encounterDescription = "";
    // Each encounter owns three independent definitions. Runtime copies are made by BattleManager.
    public List<EnemyData> enemies = new List<EnemyData>();
    public StageModifierType stageModifier = StageModifierType.None;
    [TextArea(2, 4)] public string stageModifierDescription = "";

    public string BuildDisplayName() => $"{stageName}: {encounterName}";
    public string BuildObjectiveText() => $"Goal: Defeat all enemies in {encounterName}";

    private static EnemyPatternData Pattern(string verb, int normal, string strong, int strongDamage)
    {
        return new EnemyPatternData { normalAttackMessageVerb = verb, normalAttackDamage = normal, strongAttackName = strong, strongAttackDamage = strongDamage, strongAttackEveryTurns = 3 };
    }

    // Creates distinct data objects so no flanker's HP, break gauge, or status can alias the leader.
    private static List<EnemyData> CreateFormation(string name, int hp, ElementType weakness, EnemyPatternData leaderPattern, EnemyVisualVariant visual)
    {
        // This is intentionally explicit rather than list-position-derived: each live enemy owns its sprite identity.
        EnemyData leader = new EnemyData(name, hp, weakness, leaderPattern, visual, BattleVisualId.Goblin);
        EnemyData left = new EnemyData(name + " Left", Mathf.Max(1, hp * 2 / 3), weakness,
            Pattern("strikes", Mathf.Max(1, leaderPattern.normalAttackDamage - 3), leaderPattern.strongAttackName + " Feint", Mathf.Max(1, leaderPattern.strongAttackDamage - 7)), visual, BattleVisualId.Skeleton);
        EnemyData right = new EnemyData(name + " Right", Mathf.Max(1, hp * 3 / 4), weakness,
            Pattern("attacks", Mathf.Max(1, leaderPattern.normalAttackDamage - 1), leaderPattern.strongAttackName + " Rush", Mathf.Max(1, leaderPattern.strongAttackDamage - 4)), visual, BattleVisualId.Orc);
        return new List<EnemyData> { leader, left, right };
    }

    private static List<EnemyData> CreateRuinsPatrolFormation()
    {
        EnemyPatternData goblinPattern = Pattern("attacks", 15, "Heavy Slam", 30);
        return new List<EnemyData>
        {
            new EnemyData("Goblin", 80, ElementType.Fire, goblinPattern, EnemyVisualVariant.Goblin, BattleVisualId.Goblin),
            new EnemyData("Skeleton", 53, ElementType.Fire, Pattern("strikes", 12, "Heavy Slam Feint", 23), EnemyVisualVariant.Skeleton, BattleVisualId.Skeleton),
            new EnemyData("Orc Berserker", 60, ElementType.Fire, Pattern("attacks", 14, "Heavy Slam Rush", 26), EnemyVisualVariant.Orc, BattleVisualId.Orc)
        };
    }

    private static StageData Make(string stage, string encounter, string description, StageModifierType modifier, string modifierDescription, string enemy, int hp, ElementType weakness, string verb, int normal, string strong, int strongDamage, EnemyVisualVariant visual)
    {
        return new StageData { stageName = stage, encounterName = encounter, encounterDescription = description, stageModifier = modifier, stageModifierDescription = modifierDescription, enemies = CreateFormation(enemy, hp, weakness, Pattern(verb, normal, strong, strongDamage), visual) };
    }

    public static StageData CreateStage1Normal() => new StageData { stageName = "Stage 1-1", encounterName = "Ruins Patrol", encounterDescription = "A mixed patrol guards the moonlit ruins.", stageModifier = StageModifierType.TutorialField, stageModifierDescription = "A safe training ground. No special hazards.", enemies = CreateRuinsPatrolFormation() };
    public static StageData CreateStage1Boss() => Make("Stage 1-2", "Ruins Warden", "The Ruins Warden blocks the inner gate.", StageModifierType.TutorialField, "A safe training ground. No special hazards.", "Ruins Warden", 140, ElementType.Fire, "commands the patrol", 18, "Warden Slam", 36, EnemyVisualVariant.Skeleton);
    public static StageData CreateStage2Normal() => Make("Stage 2-1", "Wolf Pack", "A wolf pack prowls the clearing.", StageModifierType.PackPressure, "Enemy strong attacks come more frequently!", "Wolf Scout", 100, ElementType.Nature, "lunges", 18, "Pack Howl", 35, EnemyVisualVariant.Orc);
    public static StageData CreateStage2Boss() => Make("Stage 2-2", "Alpha Wolf", "The Alpha Wolf leads the charge.", StageModifierType.PackPressure, "Enemy strong attacks come more frequently!", "Alpha Wolf", 180, ElementType.Nature, "leads the pack", 22, "Alpha Strike", 42, EnemyVisualVariant.DarkKnight);
    public static StageData CreateStage3Normal() => Make("Stage 3-1", "Golem Sentries", "Stone sentries block the path.", StageModifierType.Stoneguard, "Enemy starts with reinforced break defense.", "Golem Sentry", 120, ElementType.Earth, "pounds", 20, "Bedrock Slam", 38, EnemyVisualVariant.Golem);
    public static StageData CreateStage3Boss() => Make("Stage 3-2", "Ancient Golem", "The Ancient Golem awakens.", StageModifierType.Stoneguard, "Enemy starts with reinforced break defense.", "Ancient Golem", 220, ElementType.Earth, "crumbles earth", 25, "Cataclysm", 48, EnemyVisualVariant.Golem);
    public static StageData CreateStage4Normal() => Make("Stage 4-1", "Storm Hawks", "Storm Hawks circle overhead.", StageModifierType.StormSurge, "Every 3 turns, residual lightning strikes.", "Storm Hawk", 140, ElementType.Lightning, "swoops", 22, "Thunder Dive", 40, EnemyVisualVariant.Orc);
    public static StageData CreateStage4Boss() => Make("Stage 4-2", "Thunder Phoenix", "The Thunder Phoenix rises.", StageModifierType.StormSurge, "Every 3 turns, residual lightning strikes.", "Thunder Phoenix", 250, ElementType.Lightning, "calls lightning", 28, "Skyfall", 55, EnemyVisualVariant.DarkKnight);
    public static StageData CreateStage5Normal() => Make("Stage 5-1", "Shadow Wraiths", "Wraiths drift through the darkness.", StageModifierType.VoidDrain, "Shadow energy drains AP over time.", "Shadow Wraith", 160, ElementType.Dark, "lashes out with shadow", 25, "Void Grasp", 45, EnemyVisualVariant.Lich);
    public static StageData CreateStage5Boss() => Make("Stage 5-2", "Shadow Lord", "The Shadow Lord descends from the void.", StageModifierType.VoidDrain, "Shadow energy drains AP over time.", "Shadow Lord", 280, ElementType.Dark, "commands shadow tendrils", 30, "Oblivion Strike", 55, EnemyVisualVariant.Lich);
    public static StageData CreateStage6Normal() => Make("Stage 6-1", "Light Wardens", "Radiant wardens stand guard.", StageModifierType.RadiantTrial, "The ultimate trial. Enemies are relentless.", "Light Warden", 180, ElementType.Light, "strikes with holy light", 28, "Radiance Blast", 50, EnemyVisualVariant.Golem);
    public static StageData CreateStage6Boss() => Make("Stage 6-2", "Holy Sentinel", "The Holy Sentinel descends.", StageModifierType.RadiantTrial, "The ultimate trial. Enemies are relentless.", "Holy Sentinel", 320, ElementType.Light, "commands divine judgment", 32, "Heavenly Wrath", 60, EnemyVisualVariant.DarkKnight);

    public static string GetModifierDisplayName(StageModifierType type) => type switch { StageModifierType.TutorialField => "Tutorial Field", StageModifierType.PackPressure => "Pack Pressure", StageModifierType.Stoneguard => "Stoneguard", StageModifierType.StormSurge => "Storm Surge", StageModifierType.VoidDrain => "Void Drain", StageModifierType.RadiantTrial => "Radiant Trial", _ => "None" };
    public string BuildModifierSummaryText() => string.IsNullOrWhiteSpace(stageModifierDescription) ? $"Modifier: {GetModifierDisplayName(stageModifier)}" : $"Modifier: {GetModifierDisplayName(stageModifier)}\nEffect: {stageModifierDescription}";
    public static List<StageData> GetEncountersForStage(int stageIndex) => stageIndex switch { 0 => new List<StageData> { CreateStage1Normal(), CreateStage1Boss() }, 1 => new List<StageData> { CreateStage2Normal(), CreateStage2Boss() }, 2 => new List<StageData> { CreateStage3Normal(), CreateStage3Boss() }, 3 => new List<StageData> { CreateStage4Normal(), CreateStage4Boss() }, 4 => new List<StageData> { CreateStage5Normal(), CreateStage5Boss() }, 5 => new List<StageData> { CreateStage6Normal(), CreateStage6Boss() }, _ => new List<StageData> { CreateStage1Normal(), CreateStage1Boss() } };
}
