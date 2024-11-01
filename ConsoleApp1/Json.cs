using System.Reflection;
using Newtonsoft.Json;

/// <summary>
/// Class for handling Json writing, reading, storing and overwriting
/// </summary>
/// <typeparam name="T"> Expected to be the class that Json will be reading and writing  </typeparam>
sealed class Json { // T must be a class
    private static string CWD = Directory.GetCurrentDirectory();

    /// <summary>
    /// Verifies the path ends with .json
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="ArgumentException"></exception>
    private static void VerifyPathIsJson(List<string> path) {
        string last_variable_in_path = path[path.Count - 1];
        if (!last_variable_in_path.EndsWith(".json")) {
            throw new ArgumentException($"The following path does not end with a .json file (path ending) \"{last_variable_in_path}\"");
        }
    }

    /// <summary>
    /// Verifies the path is valid in the os as not a blank string and a root path
    /// </summary>
    /// <param name="path_string"></param>
    /// <param name="create_directories"></param>
    /// <exception cref="ArgumentException"></exception>
    private static void VerifyValidPath(string path_string) {
        // check if it's not a possible path
        if (string.IsNullOrWhiteSpace(path_string) || !Path.IsPathRooted(path_string)) { 
            throw new ArgumentException($"The following path is invalid: \"{path_string}\"");
        }
    }

    /// <summary>
    /// Writes an input class as a json file to the specific location
    /// </summary>
    /// <param name="path"> Path to write json data to. Path is automatically appended to the current working directory </param>
    /// <param name="content"> Class as content to be written to the Json file. </param>
    /// <param name="create_directories"> Whether or not non-existent directories can be written </param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static void Write<T>(List<string> path, T content, bool create_directories=true) where T : class {
        // check if the file is a json file or not
        VerifyPathIsJson(path);
        // getting the path into a string format
        string path_string = Path.Combine(path.Prepend(CWD).ToArray()); // prepends the CWD and combines it into a path
        // check if the path is a valid path that can be written/read from/to
        VerifyValidPath(path_string);

        // check directory (and write)
        string path_directory = Path.GetDirectoryName(path_string)!; // exclamation mark (!) forgives the possibility of the value being null
        if (!Directory.Exists(path_directory)) { // if directory doesnt exist
            if (create_directories == true) { // if directories can be created
                Directory.CreateDirectory(path_directory);
            } else {
                throw new DirectoryNotFoundException($"The following path is not an existing directory: \"{path_directory}\"");
            }
        }

        // write file
        string json_string = JsonConvert.SerializeObject(new List<T>{content});
        using (StreamWriter file = new StreamWriter(path_string)) {
            file.Write(json_string);
        }
    }

    /// <summary>
    /// Reads a whole json file
    /// </summary>
    /// <param name="path"> Path to read Json data from. Path is automatically appended to the current working directory </param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static T Read<T>(List<string> path) where T : class {
        // check if the file is a json file or not
        VerifyPathIsJson(path);
        // getting the path into a string format
        string path_string = Path.Combine(path.Prepend(CWD).ToArray()); // prepends the CWD and combines it into a path
        // check if the path is a valid path that can be written/read from/to
        VerifyValidPath(path_string);

        // check if the file exists or not to read
        if (!File.Exists(path_string)) {
            throw new FileNotFoundException($"The following path is not an existing file: {path_string}");
        }

        // read file
        using (StreamReader file = new StreamReader(path_string)) {
            string jsonString = file.ReadToEnd();
            var contents = JsonConvert.DeserializeObject<List<T>>(jsonString)!; // can throw errors, but I let it because I want the program to stop if one of them occurs
            return contents[0];
        }
    }

    /// <summary>
    /// Gets a value out of a class Json rather than reading the whole class Json
    /// </summary>
    /// <param name="path"> Path to read Json data from. Path is automatically appended to the current working directory </param>
    /// <param name="target"> The name of the variable (property/field) to target </param>
    /// <returns> Returns the value of either the field or property that was found </returns>
    /// <exception cref="ArgumentException"></exception>
    public static object? ReadFor<T>(List<string> path, string target) where T : class {
        T json = Read<T>(path); // read the json object

        // check for a property
        PropertyInfo? propertyInfo = typeof(T).GetProperty(target);
        if (propertyInfo == null) {
            // check for a field
            FieldInfo? fieldInfo = typeof(T).GetField(target);
            if (fieldInfo == null) { // runs if both field and property cannot be found
                throw new ArgumentException($"Could not find a property with the name: \"{target}\" of type: \"{typeof(T)}\"");
            }
            return fieldInfo.GetValue(json); // return field
        }
        return propertyInfo.GetValue(json); // return property
    }

    /// <summary>
    /// Deletes a Json file
    /// </summary>
    /// <remarks>
    /// The file must end with .json for it to be deleted this way
    /// </remarks>
    /// <param name="path"> The path to the file </param>
    /// <exception cref="FileNotFoundException"></exception>
    public static void Delete(List<string> path) {
        // check if the file is a json file or not
        VerifyPathIsJson(path);
        // getting the path into a string format
        string path_string = Path.Combine(path.Prepend(CWD).ToArray()); // prepends the CWD and combines it into a path
        // check if the path is a valid path that can be written/read from/to
        VerifyValidPath(path_string);

        // check if the file exists or not to delete
        if (!File.Exists(path_string)) {
            throw new FileNotFoundException($"The following path is not an existing file: {path_string}");
        } // if exists, delete
        File.Delete(path_string);
    }   
}