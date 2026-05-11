# Study Note - 2026-05-11 UI Guide Text for Onboarding

## Concept

A small static UI label can act as onboarding. It does not need to change battle logic; it simply tells the player what actions are meaningful before they start clicking.

## Applied in this project

The generated battle scene now creates `Battle Guide Text` under the title:

```text
Battle Guide: Attack to deal damage | Fire Skill applies Burn | Guard before Heavy Slam | Watch Enemy Intent | Retry after result
```

This is generated from `Assets/Editor/BattleSceneAutoBuilder.cs`, so the scene can be rebuilt consistently from the Unity menu instead of editing the scene by hand.

## Validation idea

Because this guide is part of the generated scene, the validator checks both:

- the `Battle Guide Text` object exists
- the text includes key tokens such as `Attack`, `Fire Skill`, `Burn`, `Guard`, `Enemy Intent`, and `Retry`

## Lesson learned

Portfolio UX is not only about adding mechanics. Sometimes a small label that explains the intended test route makes existing mechanics much easier for a reviewer to notice.
