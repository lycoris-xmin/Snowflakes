### 雪花Id工具服务

### 安装方式
```shell
// net cli
dotnet add package Lycoris.Snowflakes
// package manager
Install-Package Lycoris.Snowflakes
```
 
## 单机服务

### 安装方式
```csharp
// 默认注册
// 注册为单例服务
builder.Services.AddSnowflake().AsService();
// 注册为静态实例
builder.Services.AddSnowflake().AsHelper();

// 详细配置(单例服务与静态实例配置相同，不做多其他实例)
builder.Services.AddSnowflake(opt=>
{
    // 工作机器ID，默认从1开始,用于防止时钟回拨导致的Id重复
    opt.WorkId = 1;
    // 工作机器id所占用的长度，最大10，默认10
    opt.WorkIdLength = 10;
    // 用于计算时间戳的开始时间，默认起始时间 UTC 2000-01-01
    // 设置为固定时间，请不要设置为DateTime.Now
    opt.StartTimeStamp = new DateTime(2022, 1, 1)
}).AsService();
```

### 使用方式
```csharp
// 单例服务
public class Demo
{
    private readonly ISnowflakeMaker _snowflakeMaker;

    public Demo(ISnowflakeMaker snowflakeMaker)
    {
        _snowflakeMaker = snowflakeMaker
    }

    public void Test()
    {
        var id = _snowflakeMaker.GetNextId();
    }

    public void TestAsync()
    {
        var id = await _snowflakeMaker.GetNextIdAsync();
    }
}

// 静态实例
public class Demo
{
    public void Test()
    {
        var id = SnowflakeHelper.GetNextId();
    }

    public void TestAsync()
    {
        var id = await SnowflakeHelper.GetNextIdAsync();
    }
}
```

---

## 分布式

**由于分布式需要同一个服务或集群内保证唯一Id，需要分配给不同实例分配对应的机器Id，故需要redis做辅助，此处推荐 `CSRedisCore` 做为redis服务端演示**

### 单例服务

#### 创建redis辅助服务

**创建redis辅助服务类 `DistributedSnowflakesRedis` 继承 `IDistributedSnowflakesRedis` 接口并实现对应功能，此处以  `CSRedisCore` 做为演示，其他redis相关服务请使用对应的方法，以下指令与redis cli指令名称相同**

```csharp
public class DistributedSnowflakesRedis : IDistributedSnowflakesRedis
{
    private readonly CSRedisClient client;

    public DistributedSnowflakesRedis()
    {
        client = new CSRedisClient("host:port,password=password,defaultDatabase=0");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expire"></param>
    /// <returns></returns>
    public Task<bool> ExpireAsync(string key, TimeSpan expire) => client.ExpireAsync(key, expire);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task<long> IncrByAsync(string key, long value) => client.IncrByAsync(key, value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="scoreMembers"></param>
    /// <returns></returns>
    public Task<long> ZAddAsync(string key, params (decimal, object)[] scoreMembers) => client.ZAddAsync(key, scoreMembers);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="count"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public Task<(string member, decimal score)[]> ZRangeByScoreWithScoresAsync(string key, decimal min, decimal max, long? count = null, long offset = 0) => client.ZRangeByScoreWithScoresAsync(key, min, max, count, offset);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public Task<long> ZRemAsync<T>(string key, params T[] member) => client.ZRemAsync(key, member);
}
```

#### 注册服务

```csharp
// 默认注册
builder.Services.AddDistributedSnowflake().AddSnowflakesRedisService<DistributedSnowflakesRedis>().AsService();

// 注册服务
builder.Services.AddDistributedSnowflake(opt => 
{
    // 工作机器ID，默认从1开始,用于防止时钟回拨导致的Id重复
    opt.WorkId = 1;
    // 工作机器id所占用的长度，最大10，默认10
    opt.WorkIdLength = 10;
    // 用于计算时间戳的开始时间，默认起始时间 UTC 2000-01-01
    // 设置为固定时间，请不要设置为DateTime.Now
    opt.StartTimeStamp = new DateTime(2022, 1, 1);
    // 分布式路由前缀(根据对应集群或者服务类别配置不同的前缀，如果未设置，则会随机生成guid)
    opt.RedisPrefix = "";
    // 分布式Id 刷新存活状态的间隔时间，默认1小时
    opt.RefreshAliveInterval = TimeSpan.FromHours(1);
})
// 添加redis辅助服务
.AddSnowflakesRedisService<DistributedSnowflakesRedis>()
.AsService();
```

#### 使用方式
```csharp
// 单例服务
public class Demo
{
    private readonly ISnowflakeMaker _snowflakeMaker;

    public Demo(ISnowflakeMaker snowflakeMaker)
    {
        _snowflakeMaker = snowflakeMaker
    }

    public void Test()
    {
        var id = _snowflakeMaker.GetNextId();
    }

    public void TestAsync()
    {
        var id = await _snowflakeMaker.GetNextIdAsync();
    }
}
```

### 静态实例

#### 创建redis辅助服务
**创建redis辅助服务类 `DistributedSnowflakesRedis` 继承 `IDistributedSnowflakesRedis` 接口并实现对应功能，此处以  `CSRedisCore` 做为演示，其他redis相关服务请使用对应的方法，以下指令与redis cli指令名称相同**

```csharp
public class DistributedSnowflakesRedis : IDistributedSnowflakesRedis
{
    private readonly CSRedisClient client;

    public DistributedSnowflakesRedis()
    {
        client = new CSRedisClient("host:port,password=password,defaultDatabase=0");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expire"></param>
    /// <returns></returns>
    public Task<bool> ExpireAsync(string key, TimeSpan expire) => client.ExpireAsync(key, expire);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task<long> IncrByAsync(string key, long value) => client.IncrByAsync(key, value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="scoreMembers"></param>
    /// <returns></returns>
    public Task<long> ZAddAsync(string key, params (decimal, object)[] scoreMembers) => client.ZAddAsync(key, scoreMembers);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="count"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public Task<(string member, decimal score)[]> ZRangeByScoreWithScoresAsync(string key, decimal min, decimal max, long? count = null, long offset = 0) => client.ZRangeByScoreWithScoresAsync(key, min, max, count, offset);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public Task<long> ZRemAsync<T>(string key, params T[] member) => client.ZRemAsync(key, member);
}
```

#### 注册服务
```csharp
// 基础注册
// 这种注册方式，需要 DistributedSnowflakesRedis 没有构造函数，或 DistributedSnowflakesRedis 有构造函数但是必须为无参的构造函数
builder.Services.AddDistributedSnowflake().AddSnowflakesRedisHelper<DistributedSnowflakesRedis>().AsHelper();

// 有参构造函数请使用以下方式
// 入参仅为示例，具体请根据服务实际情况自行处理
var redisHelper = new DistributedSnowflakesRedis(redis,...);
builder.Services.AddDistributedSnowflake().AddSnowflakesRedisHelper(redisHelper).AsHelper();

// 注册服务
builder.Services.AddDistributedSnowflake(opt => 
{
    // 工作机器ID，默认从1开始,用于防止时钟回拨导致的Id重复
    opt.WorkId = 1;
    // 工作机器id所占用的长度，最大10，默认10
    opt.WorkIdLength = 10;
    // 用于计算时间戳的开始时间，默认起始时间 UTC 2000-01-01
    // 设置为固定时间，请不要设置为DateTime.Now
    opt.StartTimeStamp = new DateTime(2022, 1, 1);
    // 分布式路由前缀(根据对应集群或者服务类别配置不同的前缀，如果未设置，则会随机生成guid)
    opt.RedisPrefix = "";
    // 分布式Id 刷新存活状态的间隔时间，默认1小时
    opt.RefreshAliveInterval = TimeSpan.FromHours(1);
})
// 添加redis辅助服务
.AddSnowflakesRedisHelper<DistributedSnowflakesRedis>()
.AsHelper();
```

#### 使用方式
```csharp
// 静态实例
public class Demo
{
    public void Test()
    {
        var id = DistributedSnowflakeHelper.GetNextId();
    }

    public void TestAsync()
    {
        var id = await DistributedSnowflakeHelper.GetNextIdAsync();
    }
}
```