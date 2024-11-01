using Newtonsoft.Json;

/// <summary>
/// A base class for add-only objects. Is intherited to Mutable and Immutable versions
/// </summary>
/// <typeparam name="T"> Expected to be the type of object that will be add-only </typeparam>
public abstract class AddOnlyObjectBase <T> where T : new() { // "where T : new()" states that T must have a parameterless constructor
    // --- VARIABLES ---
    
    /// <summary>
    /// An instance of the object which should be set to add-only
    /// </summary>
    [JsonProperty("value")]
    public T Value { get; protected set; }

    // --- CONSTRUCTOR ---

    /// <summary>
    /// <strong>Do not override the constructor, instead override </strong><see cref="Initalize()"/><strong>.</strong>
    /// </summary>
    public AddOnlyObjectBase() {
        Initialize();

        // backup if Initalize doesn't set T
        Value ??= new T(); // works because of the "where T : new()" which is that T must have a parametereless constructor
    }

    /// <summary>
    /// Creates a copy of the class from a T value
    /// </summary>
    /// <remarks>
    /// <strong>Do not override the constructor, instead override </strong><see cref="Initalize()"/><strong>.</strong>
    /// </remarks>
    /// <param name="value"> a value to create the class from; copy Value from value </param>
    public AddOnlyObjectBase(T value) {
        Value = value;
    }

    /// <summary>
    /// <strong>Must set this.<paramref name="Value"/>.</strong> Works as the constructor for this class <see cref="AddOnlyObject"/>.
    /// </summary>
    protected abstract void Initialize();

    // --- CASTING ---

    public static implicit operator string(AddOnlyObjectBase<T> addOnlyObject) {
        if (addOnlyObject.Value == null) return ""; // if null return null
        return addOnlyObject.Value.ToString()!; // may tostring to null
    }

    public static implicit operator T(AddOnlyObjectBase<T> addOnlyObject) {
        return addOnlyObject.Value; 
    }

    /// --- OVERRIDES ---

    public override string ToString() => StringFormat.ToString(this);
}