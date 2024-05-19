using System.Text;
using System.Text.Json;

namespace ExpertsIndexer;

public class ExpertResume
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public IEnumerable<Skill> Skills { get; set; } = [];
    public IEnumerable<Project> Projects { get; set; } = [];

    public string AsMarkdown()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# {Name}");
        sb.AppendLine($"You can reach me at {Mail}");
        sb.AppendLine($"## Skills");
        sb.AppendLine("You can ask me about:");
        foreach (var skill in Skills)
        {
            sb.AppendLine($"- {skill.Name}");
        }
        sb.AppendLine($"## Projects");
        sb.AppendLine("I have been part of the following projects:");
        foreach (var project in Projects)
        {
            sb.AppendLine($"### {project.Name}");
            sb.AppendLine($"{project.Summary}");
            sb.AppendLine($"{project.Description}");
        }
        return sb.ToString();
    }

    public Stream AsMarkdownStream()
    {
        var markdown = AsMarkdown();
        var bytes = Encoding.UTF8.GetBytes(markdown);
        return new MemoryStream(bytes);
    }

    public Stream AsJsonStream()
    {
        var json = JsonSerializer.Serialize(this);
        var bytes = Encoding.UTF8.GetBytes(json);
        return new MemoryStream(bytes);
    }
}

public class Skill
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class Project
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}