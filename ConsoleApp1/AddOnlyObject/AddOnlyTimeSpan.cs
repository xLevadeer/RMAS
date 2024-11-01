/// <summary>
/// Mutable TimeSpan variant of add-only class
/// </summary>
public class AddOnlyTimeSpan : AddOnlyObject<TimeSpan> {
    public AddOnlyTimeSpan() : base() {}
    public AddOnlyTimeSpan(TimeSpan value) : base(value) {}
    
    protected override void Initialize()
    {
        this.Value = new TimeSpan();
    }

    public override void Add(TimeSpan value)
    {
        this.Value += value;
    }

    /// <summary>
    /// Cast support for converting immutable to mutable
    /// </summary>
    public static explicit operator AddOnlyTimeSpan(ImmutableAddOnlyTimeSpan value) {
        return new AddOnlyTimeSpan(value.Value);
    }

    public static implicit operator double(AddOnlyTimeSpan value) {
        return value.Value.TotalMinutes;
    }

    public static implicit operator AddOnlyTimeSpan(TimeSpan value) {
        return new AddOnlyTimeSpan(value);
    }
}

/// <summary>
/// Immutable TimeSpan variant of add-only class
/// </summary>
public class ImmutableAddOnlyTimeSpan : ImmutableAddOnlyObject<TimeSpan> {
    public ImmutableAddOnlyTimeSpan() : base() {}
    public ImmutableAddOnlyTimeSpan(TimeSpan value) : base(value) {}
    
    protected override void Initialize()
    {
        this.Value = new TimeSpan();
    }

    public static implicit operator double(ImmutableAddOnlyTimeSpan value) {
        return value.Value.TotalMinutes;
    }

    /// <summary>
    /// Cast support for converting mutable to immutable
    /// </summary>
    public static explicit operator ImmutableAddOnlyTimeSpan(AddOnlyTimeSpan value) {
        return new ImmutableAddOnlyTimeSpan(value.Value);
    }

    public static implicit operator ImmutableAddOnlyTimeSpan(TimeSpan value) {
        return new ImmutableAddOnlyTimeSpan(value);
    }
}