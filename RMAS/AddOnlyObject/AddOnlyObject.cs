
/// <summary>
/// Mutable version of the add-only object base. Requires the Add method to be defined.
/// </summary>
/// <typeparam name="T"> Expected to be the type of object that will be add-only </typeparam>
public abstract class AddOnlyObject<T> : AddOnlyObjectBase<T> where T : new() {

    // --- CONSTRUCTOR ---

    public AddOnlyObject() : base() {}
    public AddOnlyObject(T value) : base(value) {}

    // --- METHODS ---

    /// <summary>
    /// Method for logic of how this type of object can be added
    /// </summary>
    /// <param name="value"> The input value that will be added </param>
    public abstract void Add(T value);
}

/// <summary>
/// Immutable version of the add-only object base.
/// </summary>
/// <typeparam name="T"> Expected to be the type of object that will be add-only </typeparam>
public abstract class ImmutableAddOnlyObject<T> : AddOnlyObjectBase<T> where T : new() {

    // --- CONSTRUCTOR ---

    public ImmutableAddOnlyObject() : base() {}
    public ImmutableAddOnlyObject(T value) : base(value) {}
}