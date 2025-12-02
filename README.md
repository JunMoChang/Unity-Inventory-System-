# Unity RPG 背包系统

> 基于 Unity UGUI 实现的功能完整的 RPG 游戏背包系统，支持物品管理、自动分类、多维度排序

![Unity](https://img.shields.io/badge/Unity-2022.3+-blue)
![License](https://img.shields.io/badge/License-MIT-green)
![C#](https://img.shields.io/badge/C%23-9.0-purple)

## ✨ 功能特性

- ✅ **物品管理**
  - 拾取、堆叠（最大 99）、使用、丢弃
  - 鼠标左键查看详情，右键丢弃物品
  
- ✅ **自动分类**
  - 物品按类型自动存入对应容器
  - 支持 5 大分类：武器、消耗品、防具、材料、其他

- ✅ **智能筛选**
  - 点击分类标签切换显示
  - All 标签容器同步所有分类数据

- ✅ **多维度排序**
  - 支持按名称、类型、数量、稀有度排序
  - 升序/降序一键切换

- ✅ **流畅交互**
  - 鼠标滚轮滚动查看更多物品
  - 格子选中高亮显示

## 🎬 演示
![功能演示](docs/demo.gif)

*拾取物品 → 自动分类 → 切换筛选 → 排序整理*

## 🛠️ 技术栈

- **Unity** 2022.3+
- **C# 9.0**
- **UGUI** (ScrollRect, Dropdown, Grid Layout Group)
- **ScriptableObject** (物品数据配置)

## 🎯 技术亮点

### 1️⃣ 引用机制实现数据同步

```
源格子（Weapon 容器） ← 存储真实数据
    ↓↑ 双向引用
引用格子（All 容器）   ← 只负责显示
```

**优势**：避免数据冗余，操作同步更新，内存占用低

**核心代码**：

```csharp
public void SetAsReferenceSlot(ItemSlot targetSlot)
{
    isReferenceSlot = true;
    sourceSlot = targetSlot;

    if (!targetSlot.referenceSlots.Contains(this))
    {
        targetSlot.referenceSlots.Add(this);
    }

    SyncFromSourceSlot(targetSlot);
}
```

### 2️⃣ 集合遍历安全处理

解决"遍历时修改集合"导致的运行时异常

```csharp
// 使用副本遍历，避免遍历时修改
var copy = new List<ItemSlot>(referenceSlots);
foreach (var slot in copy)
{
    slot.SyncFromSourceSlot(this);
}
```

### 3️⃣ 自动分类存储

拾取时根据 `ItemType` 自动路由到对应容器

```csharp
InventorySlotsContainer itemSlotContainer = SelectedSlotContainer(itemSo.itemType);
itemSlotContainer.AddItem(itemName, itemSo, quantity, sprite, description);
//建立关系，同步 All 容器显示
CreateReferenceInAllContainer(this);
SyncToReferenceSlots();
```

### 4️⃣ 动态 UI 生成

使用 `Dropdown` + `Vertical Layout Group` 动态生成排序选项

## 🚀 快速开始

### 环境要求

- Unity 2022.3 或更高版本
- TextMeshPro 包

### 安装步骤

1. **克隆项目**
```bash
git clone https://github.com/YourUsername/Unity-Inventory-System.git
cd Unity-Inventory-System
```

2. **用 Unity 打开项目**
   - 打开 Unity Hub
   - 点击 "Add" 选择项目文件夹
   - 选择 Unity 2022.3+ 版本打开

3. **运行场景**
   - 打开 `Assets/Scenes/InventoryScene`
   - 点击 Play 按钮

### 操作说明

| 操作 | 按键/鼠标 |
|------|----------|
| 打开/关闭背包 | `E` |
| 查看物品详情 | 鼠标左键点击 |
| 使用物品 | 选中后再次左键点击 |
| 丢弃物品 | 鼠标右键点击 |
| 滚动查看 | 鼠标滚轮 |
| 切换分类 | 点击顶部标签 |
| 排序 | 选择排序方式下拉菜单 |

## 📁 项目结构

```
Assets/
├── Scripts/
│   ├── InventoryManager.cs           # 背包管理器（核心）
│   ├── InventorySlotsContainer.cs    # 容器管理（分类）
│   ├── ItemSlot.cs                    # 格子逻辑（交互）
│   ├── InventorySort.cs               # 排序功能
│   ├── InventoryCategory.cs           # 分类切换
│   └── Item.cs                        # 物品逻辑
├── ScriptableObjects/
│   ├── ItemScriptableObject.cs        # 物品数据定义
│   └── Items/                         # 物品配置文件
│       ├── Weapons/
│       ├── Consumables/
│       └── Materials/
├── Prefabs/
│   └── UI/
│       ├── InventoryCanvas.prefab     # 背包 UI
│       └── ItemSlot.prefab            # 格子预制体
└── Scenes/
    └── InventoryScene.unity           # 演示场景
```

## 📚 核心类说明

### InventoryManager
背包系统的核心管理类

**主要职责**：
- 管理所有容器（All、Weapon、Consumable 等）
- 处理物品添加、使用、丢弃
- 维护引用关系

**关键方法**：
```csharp
public int AddItem(string itemName, ItemScriptableObject itemSo, ...);
public void CreateReferenceInAllContainer(ItemSlot sourceSlot);
```

### ItemSlot

单个格子的逻辑

**主要职责**：

- 显示物品信息（图标、数量、描述）
- 处理点击交互（选中、使用、丢弃）
- 同步数据（源格子 ⇄ 引用格子）

**关键方法**：

```csharp
public int AddItem(...);
public void SetAsReferenceSlot(ItemSlot sourceSlot);
private void SyncFromSourceSlot(ItemSlot targetSlot);
```

### ItemScriptableObject
物品数据配置

**字段**：
```csharp
public ItemType itemType;     // 物品类型
public ItemRarity rarity;     // 稀有度
public BaseState baseState;   // 基础状态（如回血）
public int amountRecover;     // 恢复量
```

## 🔧 自定义配置

### 添加新物品

1. 右键 `Assets/ScriptableObjects/Items/` → Create → Inventory → Item
2. 配置物品属性
3. 放入场景测试拾取

### 调整背包容量

```csharp
// 在 Inspector 中调整 ItemSlots 数组大小
[SerializeField] private ItemSlot[] itemSlots; // Size: 20
```

### 修改排序规则

```csharp
// InventorySort.cs
switch (currentSortType)
{
    case ItemSortType.Name:
        resultSortedList = resultSortedList.OrderBy(t => t.itemName).ToList();
        break;
    // 添加自定义排序规则
}
```

## 📝 已知问题

- ⚠️ All 容器在初次打开时需要手动刷新引用（已修复）
- ⚠️ 排序后需要重新建立引用关系

## 🔜 后续计划

- [ ] 物品拖拽交换位置（IBeginDragHandler）
- [ ] 物品搜索功能（InputField 实时过滤）
- [ ] 背包容量扩展系统
- [ ] 数据持久化（JsonUtility 保存）
- [ ] 装备系统集成

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📄 开源协议

本项目采用 [MIT License](LICENSE)

## 👤 作者

**你的名字**

- GitHub: [@JunMoChang](https://github.com/JunMoChang)
- Email: 3484773855@qq.com
- 博客: https://blog.csdn.net/Mo_Chang?spm=1001.2014.3001.5343

---

⭐ 如果这个项目对你有帮助，欢迎 Star！

🐛 发现 Bug？[提交 Issue](https://github.com/YourUsername/Unity-Inventory-System/issues)

💬 有问题？[开启讨论](https://github.com/YourUsername/Unity-Inventory-System/discussions)

