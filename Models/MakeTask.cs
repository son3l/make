namespace make.Models;
internal class MakeTask
{
    public string Name { get; set; } = string.Empty;
    public List<string> Dependencies { get; set; } = [];
    public List<string> Actions { get; set; } = [];
    public override string ToString()
    {
        return $"Name: {Name}; Depends: {string.Join(", ", Dependencies)}; Actions: {string.Join(", ", Actions)}";
    }
}
