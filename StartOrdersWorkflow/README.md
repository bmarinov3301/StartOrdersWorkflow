<h1>Project configuration steps</h1>
<h2>Setting up AWS access (can be skipped if already done)</h2>

1. Create an AWS root account.
2. Creat an IAM user with console and programmatic access via the AWS console with the root account. Make sure to **save** the access and secret keys for the created user.
3. Configure AWS credentials with the access and secret keys for the IAM user created above and a region set to **eu-central-1** using the command **aws configure --profile theProfileName**

<h2>AWS infrastructure and project configuration</h2>

1. Install Amazon.Lambda.Tools Global Tools if not already installed.

```
    dotnet tool install -g Amazon.Lambda.Tools
```
2. Install AWS Toolkit for Visual Studio - https://aws.amazon.com/visualstudio/
3. Open the **StartOrders.cs** file in Visual Studio and set a value to the *roleArn* private property. IAM Role ARN can be found via the AWS Management Console inside the **IAM** service under the **Roles** section inside the IAM role summary section
4. Open project root folder and set properties in **aws-lambda-tools-defaults.json**:
    - set **profile** to the profile name configured in **Setting up AWS access Step 3**
    - set **region** to **eu-central-1**
5. Build project
6. Right-click on project in Visual Studio and select **Publish to AWS Lambda...**
7. Select AWS Credentials and Region configured in **Setting up AWS access** section
8. Input a Stack Name and create a new S3 bucket and tick the **Save settings to aws-lambda-tools-defaults.json for future deployments.** option
9. Wait for deployment process to finish and copy the **AWS Serverless URL** needed for sending HTTP requests to start the orders workflow and approve or reject orders
10. Open AWS Management Console
11. Open each of the deployed Lambdas and set their **Execution role** to **dev-bg-lambda-role** in the **Configuration -> Permissions** tab. (refer to the *UpdateProductsLambda* project for creating the necessary IAM role in AWS)
12. Open the **StartWorkflowLambda** function and add an environment variable in the **Configuration -> Environment variables** tab with:
    - key: *STEP_FUNCTION_ARN*
    - value: *ARN of the step function you will create for the OrdersWorkflow project*. Step Function ARN can be found after creating the workflow via the AWS Management Console inside the **Step Functions** service inside the workflow details section

<h2>Test application</h2>

**Important: A Step Function workflow MUST be created in order to be able to use this application**

1. Send a request to the **AWS Serverless URL** copied in **AWS infrastructure and project configuration Step 8** with a JSON body
    - example URL: https://7l4ec3p0x2.execute-api.eu-central-1.amazonaws.com/start
    - example body: 
    ```
      {
        "ProductIds": [
          "p1",
          "p2",
          "p3"
        ]
      }
    ```
2. Open the *Step Functions* service in the AWS Management Console and confirm your workflow has started an execution