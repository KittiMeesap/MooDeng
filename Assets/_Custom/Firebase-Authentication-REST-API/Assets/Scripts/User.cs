using System;

/// <summary>
/// The user class, which gets uploaded to the Firebase Database
/// </summary>

[Serializable] // This makes the class able to be serialized into a JSON
public class User
{
    public string name;

    public User(string name)
    {
        this.name = name;
    }
}
