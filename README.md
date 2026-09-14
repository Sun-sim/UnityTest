# MCPTest —— 射击类 Roguelike Demo

从零搭建的 3D 俯视角射击 Roguelike，全部用基础几何体（Cube/Sphere/Capsule/Plane）代替美术资源。
通过 Unity MCP 驱动编辑器完成场景与脚本搭建，作为可投递的作品集 Demo。

## 玩法
- WASD 移动，鼠标控制朝向，按住左键射击（子弹走对象池）。
- 敌人从场地边缘刷出、朝玩家移动，接触造成伤害。
- 每清空一波后暂停，随机抽 3 个强化，按 `1 / 2 / 3` 选择其一。
- 5 波全部清空即胜利；生命归零即失败。

## 工程结构
- `Assets/Scripts/PlayerController.cs` —— 相机相对移动 + 鼠标转向 + 边界限制 + 摄像机跟随
- `Assets/Scripts/PlayerShooting.cs` + `Bullet.cs` —— 射击与子弹（含简易对象池）
- `Assets/Scripts/Enemy.cs` + `EnemyHealth.cs` —— 敌人行为与血量（事件解耦）
- `Assets/Scripts/WaveSpawner.cs` —— 波次刷怪（边缘随机布点，事件统计存活）
- `Assets/Scripts/UpgradeManager.cs` —— Roguelike 三选一强化
- `Assets/Scripts/GameManager.cs` —— 计分 / 生命 / 波次 HUD 与胜负
- 预制体：`Assets/Prefabs/Bullet.prefab`、`Assets/Prefabs/Enemy.prefab`

## 运行
用 Unity 2022.3 LTS（中国版）打开 `MCPTest`，进入 `Assets/Scenes/Roguelike.unity`，点击 Play。

## 注意
- 国内 Unity 6 自 2025 年起不再提供，本工程基于 2022.3 LTS；后续可迁移到团结引擎。
- 美术为占位几何体，可后续替换为正式素材。
