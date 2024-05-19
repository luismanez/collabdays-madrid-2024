using Microsoft.Graph.Beta;

namespace ExpertsIndexer;

public class UserResumeService(GraphServiceClient graphServiceClient)
{
    private readonly GraphServiceClient _graphServiceClient = graphServiceClient;

    public async Task<IEnumerable<ExpertResume>> GetUserResumes()
    {
        var users = await _graphServiceClient.Users.GetAsync(
            q => {
                q.QueryParameters.Filter = "Department eq 'Engineering'";
                q.QueryParameters.Select = [ "id", "displayName", "mail" ];
            });

        var userResumes = new List<ExpertResume>();
        foreach (var user in users!.Value!)
        {
            var skills = await _graphServiceClient.Users[user.Id].Profile.Skills.GetAsync();
            var projects = await _graphServiceClient.Users[user.Id].Profile.Projects.GetAsync();

            userResumes.Add(new ExpertResume
            {
                Id = user.Id!,
                Name = user.DisplayName!,
                Mail = user.Mail!,
                Skills = skills.Value.Select(s => new Skill { Id = s.Id, Name = s.DisplayName }),
                Projects = projects.Value.Select(p => new Project
                {
                    Id = p.Id,
                    Name = p.DisplayName,
                    Description = p.Detail.Description,
                    Summary = p.Detail.Summary
                })
            });
        }

        return userResumes;
    }
}
