using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Meridian.Infrastructure.DatabaseContext;

internal class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));