namespace URP.Domain.Common;

/// <summary>
/// All timestamps in the system are stored as Unix epoch seconds (BIGINT) in MySQL.
/// The frontend converts epoch → IST (Asia/Kolkata) for display.
/// </summary>
public static class EpochHelper
{
    public static long NowSeconds() =>
        DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static long ToEpoch(DateTime dt) =>
        new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Utc)).ToUnixTimeSeconds();

    public static DateTime FromEpoch(long epoch) =>
        DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime;

    public static DateTime? FromEpochNullable(long? epoch) =>
        epoch.HasValue ? FromEpoch(epoch.Value) : null;
}
