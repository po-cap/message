using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Message.Infrastructure.Persistence;

public class DateTimeOffsetToUtcConverter : ValueConverter<DateTimeOffset,DateTimeOffset>
{
    public DateTimeOffsetToUtcConverter()
        : base(
            v => v.ToUniversalTime(), // 写入数据库时转为 UTC
            v => v // 从数据库读取时保持原样（EF Core 会自动处理时区）
        )
    { }
}