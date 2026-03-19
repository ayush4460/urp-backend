namespace URP.Domain.Common;

/// <summary>Base class for all domain entities. TKey = primary key type.</summary>
public abstract class BaseEntity<TKey>
{
    public TKey Id        { get; protected set; } = default!;
    public long CreatedAt { get; private set; }   = EpochHelper.NowSeconds();
    public long UpdatedAt { get; private set; }   = EpochHelper.NowSeconds();

    protected void Touch() => UpdatedAt = EpochHelper.NowSeconds();
}

/// <summary>Entity that supports soft-deletion. DeletedAt == null means active.</summary>
public abstract class AuditableEntity<TKey> : BaseEntity<TKey>
{
    public long? DeletedAt { get; private set; }
    public bool  IsDeleted => DeletedAt.HasValue;

    public void SoftDelete() { DeletedAt = EpochHelper.NowSeconds(); Touch(); }
    public void Restore()    { DeletedAt = null; Touch(); }
}
