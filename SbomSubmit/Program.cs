using CommandLine;
using DependencyTrack;
using System.Diagnostics;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(RunOptions)
    .WithNotParsed(HandleError);


void RunOptions(Options opts)
{
    if (string.IsNullOrEmpty(opts.projectName) || 
        string.IsNullOrEmpty(opts.projectVersion) || 
        string.IsNullOrEmpty(opts.bomFile) || 
        string.IsNullOrEmpty(opts.server) || 
        string.IsNullOrEmpty(opts.apiKey))
    {
        HandleError(new List<Error>());
        return;
    }

    Task signBom = SubmitSbomAndWait(opts.projectName, opts.projectVersion, opts.bomFile, opts.server, opts.apiKey);
    signBom.Wait();
}

void HandleError(IEnumerable<Error> errors)
{
    Console.WriteLine("Incorrect arguments, use --help");
}

async Task SubmitSbomAndWait(string projectName, string projectVersion, string bomFile, string server, string apiKey)
{
    HttpClient c = new HttpClient();
    c.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

    DependencyTrackClient dtc = new DependencyTrackClient(c);
    dtc.BaseUrl = server;

    // Get and display the version of DT we're talking to.
    var about = await dtc.GetVersionAsync();
    Console.WriteLine($"Sending SBOM to {about.Application} version {about.Version} on '{server}'");

    // Get the project Id
    var project = await dtc.GetProject1Async(projectName, projectVersion);
    
    var file = File.ReadAllBytes(bomFile);

    var processing = await dtc.UploadBomReturnAsyncProcessingToken(
        new BomSubmitRequest
        {
            Project = project.Uuid.ToString(),
            Bom = Convert.ToBase64String(file),
        });

    bool? result = true;
    var startedAt = DateTime.UtcNow;
    do
    {
        result = await dtc.IsTokenBeingProcessed2Async(processing.Token);
        if (result == null)
        {
            throw new InvalidOperationException("Unknown state");
        }

        if (result == false)
        {
            Console.WriteLine("Finished Processing");
        } 
        else
        {
            Trace.WriteLine("Still Processing");
        }

        await Task.Delay(TimeSpan.FromSeconds(5.0d));
    }
    while (result == true && DateTime.UtcNow - startedAt < TimeSpan.FromMinutes(5.0d));

    var vulnerabilities = await dtc.GetVulnerabilitiesByProjectAsync(project.Uuid.ToString(), false);

    foreach (var vuln in vulnerabilities)
    {
        Console.WriteLine($"{vuln.Source}::{vuln.CvssV2Vector}");
    }
}

public class Options
{
    [Option('p', "projectName", Required = true, HelpText = "Set project name.")]
    public string? projectName { get; set; }

    [Option('v', "projectVersion", Required = true, HelpText = "Set project version.")]
    public string? projectVersion { get; set; }

    [Option('b', "bom", Required = true, HelpText = "Set bom filename.")]
    public string ?bomFile { get; set; }

    [Option('s', "server", Required = true, HelpText = "server:port of SBOM server.")]
    public string ?server { get; set; }

    [Option('k', "key", Required = true, HelpText = "Set API Key for calling SBOM server.")]
    public string ?apiKey { get; set; }
}