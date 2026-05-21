namespace make.Models;
internal class MakeTask
{
    public string Name { get; set; } = string.Empty;
    public List<MakeTask> Dependencies { get; set; } = [];
    public List<string> Actions { get; set; } = [];
    public override string ToString()
    {
        return $"Name: {Name}; Depends: {string.Join(", ", Dependencies.Select(dep => dep.Name))}; Actions: {string.Join(", ", Actions)}";
    }
    public override bool Equals(object? obj)
    {
        if (obj is MakeTask other)
        {
            return Name == other.Name;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
