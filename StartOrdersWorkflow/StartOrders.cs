using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using StartOrdersWorkflow.Models;
using StartOrdersWorkflow.Models.Responses;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace StartOrdersWorkflow;
public class StartOrders
{
    private const string roleArn = "aws-iam-role-arn-here";
    private readonly IAmazonStepFunctions _stepFunctionsClient;

    public StartOrders(IAmazonStepFunctions stepFunctionsClient)
    {
        _stepFunctionsClient = stepFunctionsClient;
    }

    [LambdaFunction(ResourceName = "StartWorkflowLambda", Timeout = 15, MemorySize = 128, Role = roleArn)]
    [HttpApi(LambdaHttpMethod.Post, "/start")]
    public async Task<IHttpResult> StartWorkflow([FromBody] LambdaPayload payload, ILambdaContext context)
    {
        var stepFunctionArn = Environment.GetEnvironmentVariable("STEP_FUNCTION_ARN");

        try
        {
            context.Logger.Log($"Executing Step Function with ARN - {stepFunctionArn}");

            var startExecutionRequest = new StartExecutionRequest
            {
                StateMachineArn = stepFunctionArn,
                Input = JsonSerializer.Serialize(payload)
            };

            var response = await _stepFunctionsClient.StartExecutionAsync(startExecutionRequest);

            context.Logger.LogInformation($"Step Function execution response status code - {JsonSerializer.Serialize(response?.HttpStatusCode)}");

            return HttpResults.Ok(
                new OkResponse
                {
                    Success = true,
                    ExecutionArn = response.ExecutionArn
                });
        }
        catch (Exception e)
        {
            context.Logger.LogError($"ERROR: Exception has occurred - {e.Message}");

            return HttpResults.InternalServerError(
                new ErrorResponse
                {
                    Success = false,
                    Message = e.Message
                });
        }
    }

    [LambdaFunction(ResourceName = "ApproveOrderLambda", Timeout = 30, MemorySize = 128, Role = roleArn)]
    [HttpApi(LambdaHttpMethod.Get, "/orders/approve/{orderId}")]
    public async Task<IHttpResult> ApproveOrder(string orderId, [FromQuery] string taskToken)
    {
        var request = new SendTaskSuccessRequest
        {
            Output = JsonSerializer.Serialize(new
            {
                OrderId = orderId,
                IsApproved = true
            }),
            TaskToken = taskToken
        };

        await _stepFunctionsClient.SendTaskSuccessAsync(request);

        return HttpResults.Ok($"Order approved! You can now close this window...");
    }

    [LambdaFunction(ResourceName = "RejectOrderLambda", Timeout = 30, MemorySize = 128, Role = roleArn)]
    [HttpApi(LambdaHttpMethod.Get, "/orders/reject/{orderId}")]
    public async Task<IHttpResult> RejectOrder(string orderId, [FromQuery] string taskToken)
    {
        var request = new SendTaskSuccessRequest
        {
            Output = JsonSerializer.Serialize(new
            {
                OrderId = orderId,
                IsApproved = false
            }),
            TaskToken = taskToken
        };

        await _stepFunctionsClient.SendTaskSuccessAsync(request);

        return HttpResults.Ok($"Order rejected! You can now close this window...");
    }
}