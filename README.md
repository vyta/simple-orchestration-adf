# Simple orchestration with Azure Durable Functions

> Note: Both csproj and sln files are provided, remove whichever is unnecessary for your environment

This repo contains the following:

- Simple HTTP trigger to kickoff an orchestration
- Simple orchestration function that handles two activities
  - EngagementActivity: responsible for creating and deleting engagements
  - WorkbenchStorageActivity: responsible for creating and deleting storage

```mermaid
flowchart LR
    A[HttpTrigger] -->B(Start Orchestration)
    B -->C(CreateEngagement)
    C -->|succeed or fail| B(Start Orchestration)
    B -->|CreateEngagementSucceeds|D(WorkbenchStorageCreate)
    D -->|succeed or fail| B(Start Orchestration)
    B -->|WorkbenchStorageCreateFails|E(DeleteEngagement)
```
