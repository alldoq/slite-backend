using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using SliteBackend.Configuration;
using SliteBackend.Data;
using SliteBackend.Services;
using SliteBackend.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SliteBackend;

public class Function
{
    private static readonly HttpClient client = new HttpClient();
    private readonly IServiceProvider _serviceProvider;

    public Function()
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddDatabase(configuration);
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IServiceService, ServiceService>();

        _serviceProvider = services.BuildServiceProvider();
        
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            DatabaseConfiguration.InitializeDatabaseAsync(dbContext).GetAwaiter().GetResult();
        }
    }

    private static async Task<string> GetCallingIP()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");
        var msg = await client.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext:false);

        return msg.Replace("\n","");
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var companyService = scope.ServiceProvider.GetRequiredService<ICompanyService>();
            var serviceService = scope.ServiceProvider.GetRequiredService<IServiceService>();

            var path = apigProxyEvent.Path;
            var method = apigProxyEvent.HttpMethod;

            return method.ToUpper() switch
            {
                "GET" when path == "/hello" => await HandleHelloAsync(),
                "GET" when path == "/users" => await HandleGetUsersAsync(userService),
                "POST" when path == "/users" => await HandleCreateUserAsync(userService, apigProxyEvent.Body),
                "GET" when path == "/companies" => await HandleGetCompaniesAsync(companyService),
                "POST" when path == "/companies" => await HandleCreateCompanyAsync(companyService, apigProxyEvent.Body),
                "GET" when path.StartsWith("/companies/") && path.EndsWith("/services") => await HandleGetServicesByCompanyAsync(serviceService, path),
                "GET" when path == "/services" => await HandleGetServicesAsync(serviceService),
                "POST" when path == "/services" => await HandleCreateServiceAsync(serviceService, apigProxyEvent.Body),
                _ => CreateResponse(404, new { message = "Not Found" })
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error: {ex.Message}");
            return CreateResponse(500, new { message = "Internal Server Error" });
        }
    }

    private async Task<APIGatewayProxyResponse> HandleHelloAsync()
    {
        var location = await GetCallingIP();
        var body = new Dictionary<string, string>
        {
            { "message", "hello world with database support" },
            { "location", location }
        };

        return CreateResponse(200, body);
    }

    private async Task<APIGatewayProxyResponse> HandleGetUsersAsync(IUserService userService)
    {
        var users = await userService.GetAllUsersAsync();
        return CreateResponse(200, users);
    }

    private async Task<APIGatewayProxyResponse> HandleCreateUserAsync(IUserService userService, string? requestBody)
    {
        if (string.IsNullOrEmpty(requestBody))
        {
            return CreateResponse(400, new { message = "Request body is required" });
        }

        try
        {
            var userRequest = JsonSerializer.Deserialize<User>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (userRequest == null || string.IsNullOrEmpty(userRequest.Name) || string.IsNullOrEmpty(userRequest.Email))
            {
                return CreateResponse(400, new { message = "Name and Email are required" });
            }

            var existingUser = await userService.GetUserByEmailAsync(userRequest.Email);
            if (existingUser != null)
            {
                return CreateResponse(400, new { message = "User with this email already exists" });
            }

            var createdUser = await userService.CreateUserAsync(userRequest);
            return CreateResponse(201, createdUser);
        }
        catch (JsonException)
        {
            return CreateResponse(400, new { message = "Invalid JSON format" });
        }
    }

    private async Task<APIGatewayProxyResponse> HandleGetCompaniesAsync(ICompanyService companyService)
    {
        var companies = await companyService.GetAllCompaniesAsync();
        return CreateResponse(200, companies);
    }

    private async Task<APIGatewayProxyResponse> HandleCreateCompanyAsync(ICompanyService companyService, string? requestBody)
    {
        if (string.IsNullOrEmpty(requestBody))
        {
            return CreateResponse(400, new { message = "Request body is required" });
        }

        try
        {
            var companyRequest = JsonSerializer.Deserialize<Company>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (companyRequest == null || string.IsNullOrEmpty(companyRequest.Name) || string.IsNullOrEmpty(companyRequest.Email))
            {
                return CreateResponse(400, new { message = "Name and Email are required" });
            }

            var existingCompany = await companyService.GetCompanyByEmailAsync(companyRequest.Email);
            if (existingCompany != null)
            {
                return CreateResponse(400, new { message = "Company with this email already exists" });
            }

            var createdCompany = await companyService.CreateCompanyAsync(companyRequest);
            return CreateResponse(201, createdCompany);
        }
        catch (JsonException)
        {
            return CreateResponse(400, new { message = "Invalid JSON format" });
        }
    }

    private async Task<APIGatewayProxyResponse> HandleGetServicesAsync(IServiceService serviceService)
    {
        var services = await serviceService.GetAllServicesAsync();
        return CreateResponse(200, services);
    }

    private async Task<APIGatewayProxyResponse> HandleGetServicesByCompanyAsync(IServiceService serviceService, string path)
    {
        var segments = path.Split('/');
        if (segments.Length >= 3 && int.TryParse(segments[2], out int companyId))
        {
            var services = await serviceService.GetServicesByCompanyIdAsync(companyId);
            return CreateResponse(200, services);
        }
        
        return CreateResponse(400, new { message = "Invalid company ID" });
    }

    private async Task<APIGatewayProxyResponse> HandleCreateServiceAsync(IServiceService serviceService, string? requestBody)
    {
        if (string.IsNullOrEmpty(requestBody))
        {
            return CreateResponse(400, new { message = "Request body is required" });
        }

        try
        {
            var serviceRequest = JsonSerializer.Deserialize<Service>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (serviceRequest == null || string.IsNullOrEmpty(serviceRequest.Name) || serviceRequest.CompanyId <= 0)
            {
                return CreateResponse(400, new { message = "Name and CompanyId are required" });
            }

            var createdService = await serviceService.CreateServiceAsync(serviceRequest);
            return CreateResponse(201, createdService);
        }
        catch (JsonException)
        {
            return CreateResponse(400, new { message = "Invalid JSON format" });
        }
    }

    private static APIGatewayProxyResponse CreateResponse(int statusCode, object body)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
            Body = JsonSerializer.Serialize(body),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}