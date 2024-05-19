using Microsoft.Extensions.Hosting;
using Microsoft.KernelMemory;

namespace ExpertsIndexer;

public class ExpertsIndexerHostedService(
    UserResumeService userResumeService,
    MemoryServerless memoryServerless) : IHostedService
{
    private readonly UserResumeService _userResumeService = userResumeService;
    private readonly MemoryServerless _memoryServerless = memoryServerless;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var userResumes = await _userResumeService.GetUserResumes();

        foreach (var userResume in userResumes)
        {
            //Console.WriteLine(userResume.AsMarkdown());
            Console.Write($"Indexing User: {userResume.Name} ({userResume.Mail}) ...");

            var document = new Document(userResume.Id)
                .AddStream(
                    fileName: $"{userResume.Id}__{userResume.Mail}__resume.md",
                    content: userResume.AsMarkdownStream());

            await _memoryServerless.ImportDocumentAsync(
                document,
                index: "CompanyExperts",
                cancellationToken: cancellationToken);

            Console.WriteLine($"DONE!");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("ExpertsIndexerHostedService is stopping.");
        return Task.CompletedTask;
    }
}
