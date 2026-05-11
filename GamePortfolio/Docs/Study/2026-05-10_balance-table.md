# Study Note - Balance Table

## Concept

Balancing means choosing numbers that create the intended player experience. In this prototype, values are small and readable so the battle loop is easy to explain.

## Applied examples

- `Hero max HP = 100` makes damage easy to understand.
- `Normal Attack = 15` creates steady pressure without ending the battle too fast.
- `Heavy Slam = 30 every 3rd enemy turn` creates a predictable danger spike.
- `Guard = 50% reduction` gives the player a clear defensive answer.
- `Fire Skill = 30 + weakness bonus 10` rewards using the enemy weakness.

## Portfolio lesson

A balance table is useful because it records intent. If a reviewer asks why an enemy has 80 HP or why Guard reduces damage by 50%, the project has a written answer instead of only code values.

## Future lesson

When the battle grows, balance values should move toward data assets or tables so designers can tune them without editing core battle code.
