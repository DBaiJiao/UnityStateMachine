# Unity 泛型仓库系统

一个强大且灵活的Unity泛型仓库系统，专为主菜单的物品存取和角色存取而设计。

## 功能特性

### 🎯 核心功能
- **泛型设计**: 支持任何实现 `IRepositoryItem` 接口的类型
- **自动序列化**: 支持JSON格式的数据持久化
- **事件系统**: 完整的事件回调机制
- **线程安全**: 支持多线程环境下的数据操作
- **内存管理**: 智能的内存管理和数据清理

### 📦 内置支持
- **物品系统**: 完整的游戏物品管理，支持堆叠、消耗、装备等
- **角色系统**: 全面的角色数据管理，包含属性、技能、装备等
- **搜索功能**: 强大的搜索和过滤功能
- **统计信息**: 详细的数据统计和分析

### 🔧 管理功能
- **统一管理**: RepositoryManager 提供统一的仓库管理接口
- **自动保存**: 可配置的自动保存机制
- **备份恢复**: 数据备份和恢复功能
- **错误处理**: 完善的错误处理和日志记录

## 快速开始

### 1. 基础设置

```csharp
// 获取仓库管理器实例
var repositoryManager = RepositoryManager.Instance;

// 访问内置仓库
var itemRepository = repositoryManager.Items;
var characterRepository = repositoryManager.Characters;
```

### 2. 创建和管理物品

```csharp
// 创建一个武器
var sword = new GameItem("钢铁长剑")
    .SetItemType(ItemType.Weapon)
    .SetRarity(ItemRarity.Common)
    .SetDescription("一把锋利的钢铁长剑")
    .SetValueAndWeight(150, 3.0f)
    .SetCombatStats(attack: 20);

// 添加到仓库
itemRepository.Add(sword);

// 创建可堆叠的消耗品
var potion = new GameItem("生命药水")
    .SetItemType(ItemType.Consumable)
    .SetStackable(true, 20)
    .SetConsumable(true)
    .SetValueAndWeight(50, 0.2f);

// 智能添加（会自动处理堆叠）
itemRepository.SmartAdd(potion);

// 搜索和查找
var weapons = itemRepository.GetByType(ItemType.Weapon);
var rareItems = itemRepository.GetByRarity(ItemRarity.Rare);
var searchResults = itemRepository.SearchItems("剑");
```

### 3. 创建和管理角色

```csharp
// 创建一个战士角色
var warrior = new GameCharacter("亚瑟王")
    .SetCharacterClass(CharacterClass.Warrior)
    .SetGender(CharacterGender.Male)
    .SetDescription("传说中的不列颠王")
    .SetBackstory("拔出石中剑的王者...");

// 设置基础属性
warrior.Stats.SetBaseAttributes(
    strength: 16,     // 力量
    dexterity: 12,    // 敏捷  
    intelligence: 10, // 智力
    constitution: 15, // 体质
    wisdom: 11,       // 智慧
    charisma: 14      // 魅力
);

// 添加到仓库
characterRepository.Add(warrior);

// 角色升级
warrior.Stats.AddExperience(1000);

// 装备物品
warrior.EquipItem(sword.Id);

// 学习技能
warrior.LearnSkill("剑术精通");
warrior.AddActiveAbility("重击");
```

### 4. 数据持久化

```csharp
// 保存所有数据
repositoryManager.SaveAllRepositories();

// 加载所有数据
repositoryManager.LoadAllRepositories();

// 设置自动保存（每5分钟保存一次）
repositoryManager.SetAutoSave(true, 300f);

// 备份数据
repositoryManager.BackupAllRepositories();
```

## 架构设计

### 核心接口

```csharp
// 仓库项目基础接口
public interface IRepositoryItem
{
    string Id { get; }
    string DisplayName { get; }
    DateTime CreatedAt { get; }
    DateTime LastModified { get; set; }
    IRepositoryItem Clone();
    bool IsValid();
}

// 泛型仓库接口
public interface IRepository<T> where T : class, IRepositoryItem
{
    int Count { get; }
    IEnumerable<T> GetAll();
    T GetById(string id);
    bool Add(T item);
    bool Update(T item);
    bool Remove(string id);
    // ... 更多方法
}
```

### 类层次结构

```
IRepositoryItem
├── BaseRepositoryItem (抽象基类)
    ├── GameItem (游戏物品)
    └── GameCharacter (游戏角色)

IRepository<T>
├── GenericRepository<T> (泛型仓库实现)
    ├── ItemRepository (物品仓库)
    └── CharacterRepository (角色仓库)
```

## 高级功能

### 1. 自定义仓库项目

```csharp
// 创建自定义数据类型
[Serializable]
public class Quest : BaseRepositoryItem
{
    [SerializeField] private string description;
    [SerializeField] private bool isCompleted;
    [SerializeField] private int rewardXP;
    
    public Quest(string name) : base(name) { }
    
    public override IRepositoryItem Clone()
    {
        return new Quest(DisplayName)
        {
            description = this.description,
            isCompleted = this.isCompleted,
            rewardXP = this.rewardXP
        };
    }
}

// 创建自定义仓库
var questRepository = new GenericRepository<Quest>("QuestRepository");

// 注册到管理器
repositoryManager.RegisterRepository(questRepository);
```

### 2. 事件监听

```csharp
// 监听仓库事件
itemRepository.OnItemAdded += (item) => 
{
    Debug.Log($"添加了新物品: {item.DisplayName}");
};

itemRepository.OnItemRemoved += (item) => 
{
    Debug.Log($"移除了物品: {item.DisplayName}");
};

// 监听管理器事件
RepositoryManager.OnRepositorySaved += (repositoryName) => 
{
    Debug.Log($"仓库 {repositoryName} 已保存");
};
```

### 3. 条件查询

```csharp
// 复杂条件查询
var highLevelWarriors = characterRepository.FindWhere(character => 
    character.CharacterClass == CharacterClass.Warrior && 
    character.Stats.Level >= 20 &&
    character.Status == CharacterStatus.Active
);

var valuableItems = itemRepository.FindWhere(item => 
    item.Value > 1000 && 
    item.Rarity >= ItemRarity.Epic
);

// 排序查询
var topCharacters = characterRepository.GetSortedByCombatPower(true).Take(10);
var cheapestItems = itemRepository.GetSortedByValue(false).Take(5);
```

### 4. 批量操作

```csharp
// 给所有角色增加经验
var leveledUpCharacters = characterRepository.AddExperienceToAll(500);

// 治疗所有角色
characterRepository.HealAllCharacters();

// 批量状态更新
characterRepository.UpdateCharacterStatus(
    CharacterStatus.Injured, 
    CharacterStatus.Resting
);
```

## 最佳实践

### 1. 性能优化
- 使用 `FindWhere` 进行条件查询而不是手动遍历
- 及时调用 `CleanupInvalidItems/Characters` 清理无效数据
- 合理设置自动保存间隔，避免频繁IO操作

### 2. 数据安全
- 定期使用 `BackupAllRepositories` 进行数据备份
- 在应用退出前确保调用 `SaveAllRepositories`
- 使用事件监听机制处理异常情况

### 3. 扩展性
- 继承 `BaseRepositoryItem` 创建自定义数据类型
- 使用 `RegisterRepository` 注册自定义仓库
- 实现 `IRepositoryItem` 接口支持完全自定义的数据结构

## 示例场景

项目包含了完整的示例：

- **MainMenuExample**: 展示如何在主菜单中集成仓库系统
- **RepositoryTestScene**: 完整的功能测试和演示

## 文件结构

```
GenericRepositry/
├── IRepositoryItem.cs              # 基础接口
├── IRepository.cs                  # 仓库接口
├── BaseRepositoryItem.cs           # 抽象基类
├── GenericRepository.cs            # 泛型仓库实现
├── RepositoryManager.cs            # 仓库管理器
├── Items/                          # 物品系统
│   ├── ItemType.cs                 # 物品类型枚举
│   ├── GameItem.cs                 # 游戏物品类
│   └── ItemRepository.cs           # 物品仓库
├── Characters/                     # 角色系统
│   ├── CharacterClass.cs           # 角色职业枚举
│   ├── CharacterStats.cs           # 角色属性
│   ├── GameCharacter.cs            # 游戏角色类
│   └── CharacterRepository.cs      # 角色仓库
└── Examples/                       # 使用示例
    ├── MainMenuExample.cs          # 主菜单示例
    └── RepositoryTestScene.cs      # 测试场景
```

## 许可证

本项目采用 MIT 许可证。
