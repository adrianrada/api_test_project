### **Code/Problem-Solving Task for a QA Engineer Candidate**

#### **Task Overview:**
You are tasked with testing the backend of a simple API-driven web application. The application is used to manage a list of tasks (to-do list). You will need to write automated test cases for the API endpoints, using a .NET (C#) testing framework. You are also expected to create a test plan and suggest improvements for test coverage and API validation.

#### **Application Details:**
The API has the following endpoints:
1. **GET** `/tasks`: Returns a list of all tasks.
2. **POST** `/tasks`: Creates a new task. The task has the following properties:
    - `id`: Auto-generated unique identifier.
    - `name`: The name of the task (string, required, max length 100).
    - `completed`: Boolean flag indicating if the task is completed (defaults to false).
3. **PUT** `/tasks/{id}`: Updates the task’s `name` or `completed` status.
4. **DELETE** `/tasks/{id}`: Deletes a task by its `id`.

---

### **Part 1: Test Plan and Strategy**

Write a **test plan** that outlines:
- **Test Strategy:** Briefly explain the approach you would take for testing this API.
- **Test Scenarios:** List 5-10 test cases that cover both functional and non-functional aspects of the API (e.g., boundary value testing, input validation, response time).
- **Edge Cases:** Identify and explain at least two edge cases you would test for.

---

### **Part 2: Automated Testing Task**

Using .NET and any C# testing framework (such as xUnit, NUnit, or MSTest), write automated test cases for the following scenarios:

1. **Test the retrieval of tasks**:
    - Verify that calling `GET /tasks` returns the correct response structure (status code 200, JSON format).
    - Validate that it returns an empty list if there are no tasks.

2. **Test the creation of a new task**:
    - Write a test for the `POST /tasks` endpoint, ensuring that a task can be created successfully with valid data.
    - Verify that an invalid request (e.g., `name` is missing or exceeds 100 characters) returns a proper validation error (e.g., status code 400).

3. **Test task update**:
    - Test the `PUT /tasks/{id}` endpoint to verify that a task can be updated successfully.
    - Write a test to ensure that trying to update a task that doesn’t exist returns a proper error response (e.g., status code 404).

4. **Test task deletion**:
    - Write a test to verify that a task can be deleted using the `DELETE /tasks/{id}` endpoint.
    - Ensure that trying to delete a non-existent task returns a 404 error.

---

### **Part 3: Bonus - Test Framework Improvements**

As a QA Engineer, maintaining a test framework is critical. For **bonus points**, suggest improvements to the testing framework you would implement in the future to:
- Improve test performance (e.g., parallel execution).
- Ensure more detailed logging and reporting of test results.
- Handle complex API workflows, such as chaining multiple API requests.

---


