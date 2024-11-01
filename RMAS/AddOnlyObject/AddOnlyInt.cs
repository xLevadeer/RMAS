/// <summary>
/// Mutable Int variant of the add-only class
/// </summary>
public class AddOnlyInt : AddOnlyObject<int> {
    public AddOnlyInt() : base() {}
    public AddOnlyInt(int value) : base(value) {}

    protected override void Initialize(){
        this.Value = 0;
    }

    public override void Add(int value){
        if (value < 0) throw new ArgumentOutOfRangeException("The value cannot be negative");
        this.Value += value;
    }

    /// <summary>
    /// Cast support for converting immutable to immutable
    /// </summary>
    public static explicit operator AddOnlyInt(ImmutableAddOnlyInt value) {
        return new AddOnlyInt(value.Value);
    }

    public static implicit operator AddOnlyInt(int value) {
        return new AddOnlyInt(value);
    }
}

/// <summary>
/// Immutable Int variant of the add-only class
/// </summary>
public class ImmutableAddOnlyInt : ImmutableAddOnlyObject<int> {
    public ImmutableAddOnlyInt() : base() {}
    public ImmutableAddOnlyInt(int value) : base(value) {}

    protected override void Initialize(){
        this.Value = 0;
    }

    /// <summary>
    /// Cast support for converting mutable to immutable
    /// </summary>
    public static explicit operator ImmutableAddOnlyInt(AddOnlyInt value) {
        return new ImmutableAddOnlyInt(value.Value);
    }

    public static implicit operator ImmutableAddOnlyInt(int value) {
        return new ImmutableAddOnlyInt(value);
    }
}