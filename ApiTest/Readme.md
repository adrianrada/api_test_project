# **API Automated Testing Framework**

This solution contains 2 different components.

1. The source code for the unit test framework, created using a basic xUnit template.
2. The implementation of a simple HTTPClient type class that will handle the requests,
with some extra functionalities.

## **Installation**

Download the project or clone the github repository and import the solution to Visual Studio Community 2022.

## **Usage**

To run the tests, either execute them directly from the Visual Studio Test Explorer or from cmd using the below
command after having built the solution first.

```bash
dotnet test path\to\bin\Debug\net8.0\UnitTest.dll
```

## **Future Improvements**

1. **Optimize Data Handling**

For tests that require various data inputs, instead of adding InLineData for a Theory we could consider creating a
dedicated data class with all necessary information and added it as ClassData in the test or use MemberData attribute
to load data from a test class property.

See:<br />
[Creating parameterised tests in xUnit](https://andrewlock.net/creating-parameterised-tests-in-xunit-with-inlinedata-classdata-and-memberdata/)

2. **Adding Test Fixtures**

Adding setup/teardown methods at test class and/or test case level to ensure that some preconditions are met
(e.g. check the server is up and running, make sure that at least one task exists on the server) and to
perform a cleanup of the test environment after execution finishes (e.g. shut down the web server, dispose the
http client used during testing)

3. **Improving test execution performance**

By default, test cases are executed sequentially, which can bring significant performance issues if the number
and/or complexity of the test scenarios increases in time.

In order to mitigate this, we could use the following strategies:

    a) Parallel execution
    
    Group test suites into separate collections by creating different test classes that would run parallel
    against each other.

    On top of this, we can also increase the number of threads used for parallel execution.

    b) Distribute test executions to multiple machines

    Group test suites into different sets based on test domain or priority and use different hosts to execute
    the newly created scenarios (either from command line or from a CI/CD environment)

See:__
[xUnit - Running Tests in Parallel](https://xunit.net/docs/running-tests-in-parallel.html)__
[Selective Unit Tests](https://learn.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests?pivots=xunit#xunit-examples)__
[Speeding up Unit Test Execution in TFS](https://devblogs.microsoft.com/devops/speeding-up-unit-test-execution-in-tfs/)

4. **Logging and Reporting**

I've currently implemented two logger instances of the ILogger interface and used LoggerFactory to instantiate them
in the test class constructor.

The two newly created objects are used to log information during the execution of both the xUnit test cases and the
calls of ApiService methods for logging purposes.

In the future I could add more log events of different levels, for example when a test case fails or when an exception
is being thrown.

As for the test reporting, one could easily generate an html file if executing the test script from command prompt
by adding the --logger argument. The test results file is stored in under \TestResults\ in the root path where the
command is executed.

```bash
dotnet test ApiTest.dll --logger:html
```

5. **Combining Multiple API Requests**

For more complex scenarios we could try to chain together multiple API requests by using one of the following methods:

    a) Asynchronous calls
    
    Create methods with the async modifier and use await when parsing through an enumerable that contains different data
    that we want to send or query the server with.

    b) Running requests in parallel

    Using Task.WhenAll() method on tasks of the same type (can apply this method on batches of multiple requests for further
    optimization).

    c) Updating the API endpoint

    We could also take into account optimizations at server side and create an endpoint that could handle multiple 
    simultaneous requests.

    d) Use JSON batching

    This method allows us to enhance our application's performance by combining up to 20 requests in a single JSON object,
    with various modifiers to allow sequencing of requests.

See:__
[How To Send Many Requests In Parallel In ASP.NET CORE](https://www.michalbialecki.com/2018/04/19/how-to-send-many-requests-in-parallel-in-asp-net-core/)__
[JSON Batching](https://learn.microsoft.com/en-us/graph/json-batching)

6. **Create a CI/CD pipeline**

By integrating this project into github and with the usage of webhooks, we could define a CI/CD pipeline that should be
triggered when merging new changes into the main branch of the repository. The pipeline would include stages for building
the solution, starting the web server, initiating test execution, saving and sharing the test report and cleaning up the
test environment.

See:__
[Trigger Jenkins Build When Pull Request Is Merged In GitHub](https://stackoverflow.com/questions/64050510/trigger-jenkins-build-when-pull-request-is-merged-in-github)
