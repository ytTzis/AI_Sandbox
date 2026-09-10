# Task3 Map Layout

## Goal

Build a compact survival-competition sandbox inspired by common party-game obstacle courses. The scene demonstrates NavMesh navigation, an AI state machine, and physical mechanisms that can hit, knock back, and stun the AI.

## Scene Flow

AI Spawn -> Patrol Arena -> Rotating Bar -> Narrow Bridge -> Moving Wall -> Knockback Pad -> Player Target

## Objects

- Ground_NavMesh: 40 x 40 walkable arena with a NavMeshSurface.
- Boundary walls: keep the AI inside the test area.
- Static obstacles: force the NavMeshAgent to route around blocks instead of walking in a straight line.
- RotatingBar_Hit_Stun: party-game style spinning arm that triggers Hit and Stunned states.
- MovingWall_Horizontal: dynamic wall that tests collision and knockback logic.
- KnockbackPad_Red: red trigger zone that knocks the AI away.
- PatrolPoint_A/B/C: points used by the Patrol state.
- Player_Target_Goal: target used by the Chase state.

## State Showcase

- Patrol: AI loops through patrol points.
- Chase: AI moves toward Player_Target_Goal when close enough.
- Hit: AI is pushed by a mechanism.
- Stunned: AI pauses briefly after being hit, then returns to Patrol or Chase.

## Prefab Workflow

1. Open Unity and wait for script compilation.
2. Choose `Tools -> Task3 -> Create Mechanism Prefabs` once.
3. Drag prefabs from `Assets/Prefabs/Mechanisms` into the scene and arrange the map freely.
4. Rebuild the NavMesh after changing static walls or platforms.

The prefab generator creates only missing assets. It does not overwrite prefabs that you have already adjusted.

Use `Tools -> Task3 -> Build NavMesh AI Sandbox` only when you want to rebuild the complete example layout. The example layout now uses prefab instances for all mechanisms.
