/*
 * Specification At: https://github.com/tursodatabase/libsql/blob/main/docs/HTTP_V2_SPEC.md
 */

using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HranaOverHttp.Sdk;

// Define the interface for the API
public interface IHranaApi
{
    [Get("/v2")]
    Task<ApiResponse<VersionInfo>> CheckApiVersion([Header("Authorization")] string authorization);

    [Post("/v2/pipeline")]
    Task<PipelineResponse> ExecutePipeline([Body] PipelineRequest request, [Header("Authorization")] string authorization);
}

// Models for the requests and responses
public class VersionInfo
{
    public string Version { get; set; }
}

public class PipelineRequest
{
    public string Baton { get; set; }
    public List<StreamRequest> Requests { get; set; }
}

public class PipelineResponse
{
    public string Baton { get; set; }
    public string BaseUrl { get; set; }
    public List<StreamResult> Results { get; set; }
}

public class StreamRequest
{
    public string Type { get; set; }
    public Stmt Stmt { get; set; }
    public Batch Batch { get; set; }
    public int? SqlId { get; set; }
}

public class Stmt
{
    public string Sql { get; set; }
}

public class Batch
{
    public List<Stmt> Statements { get; set; }
}

public class StreamResult
{
    public string Type { get; set; }
    public StreamResponse Response { get; set; }
    public Error Error { get; set; }
}

public class StreamResponse
{
    public string Type { get; set; }
    public StmtResult Result { get; set; }
    public DescribeResult DescribeResult { get; set; }
}

public class StmtResult
{
    public int AffectedRows { get; set; }
    public List<string> Data { get; set; }
}

public class DescribeResult
{
    public string Description { get; set; }
}

public class Error
{
    public string Message { get; set; }
}

/// <summary>
/// This constructs the turso database api name as by using database name and organisation name
/// </summary>
/// <param name="DatabaseName">The name of the database</param>
/// <param name="OrganisationName">Organisation name / username</param>
public record ConstructUrl(string DatabaseName, string OrganisationName)
{
    public string BaseUrl { get; private init; } = $"https://{DatabaseName}-{OrganisationName}.turso.io";
}

// Use the interface in your application logic
public class HranaService
{
    private readonly IHranaApi _api;
    private string _baseUrl;
    private string _bearerToken;

    public HranaService(ConstructUrl constructUrl, string bearerToken)
    {
        _baseUrl = constructUrl.BaseUrl;
        _bearerToken = bearerToken;
        _api = RestService.For<IHranaApi>(_baseUrl);
    }

    public async Task<PipelineResponse> ExecuteRequestsAsync(string baton, List<StreamRequest> requests)
    {
        var request = new PipelineRequest { Baton = baton, Requests = requests };
        string authorization = $"Bearer {_bearerToken}";
        return await _api.ExecutePipeline(request, authorization);
    }

    public void UpdateBearerToken(string newToken)
    {
        _bearerToken = newToken;
    }
}


