# MediatR Decision Matrix: Deep-Dive Technical Analysis

> Customized analysis for **ProgresiumToDo** — .NET 10, MediatR v14.0.0, Clean Architecture

## Your Current MediatR Surface Area

| Metric | Count |
|---|---|
| Handlers (Commands + Queries) | **35** |
| Pipeline Behaviors | **4** (Validation, Logging, UnitOfWork, Entitlement) |
| API Controllers using `IMediator` | **10** |
| Feature Areas | **9** (Auth, Billing, OAuth, Projects, Support, Tags, Tasks, Users, Waitlist) |
| Custom Abstractions | `ICommand`, `IQuery`, `ICommandHandler`, `IQueryHandler`, `IBaseCommand`, `IBaseQuery` |

---

## 1. Performance Metrics — Per-Request Dispatch Overhead

### Raw Benchmark Numbers (BenchmarkDotNet, .NET 8/9/10)

| Dispatch Method | Time per Request | Memory Allocated | Relative Cost |
|---|---|---|---|
| **Direct method call** | ~15–20 ns | ~144 B | 1× (baseline) |
| **Source-generated Mediator** | ~50–120 ns | ~88–200 B | 3–7× |
| **MediatR v14 (no behaviors)** | ~500–820 ns | ~1.2–1.4 KB | 30–50× |
| **MediatR v14 (4 behaviors)** | ~1.5–3.0 µs | ~3–5 KB | 100–180× |

> [!IMPORTANT]
> These numbers measure **only the dispatch overhead**, not your actual business logic. A typical HTTP request with EF Core + PostgreSQL costs **5–50 ms** (5,000,000–50,000,000 ns). The MediatR overhead of ~1.5–3 µs is **0.003–0.06%** of a real request.

### What This Means for ProgresiumToDo

Your 4 pipeline behaviors (Validation → Logging → UnitOfWork → Entitlement) each add their own `IPipelineBehavior` delegate invocation. The total per-request overhead from MediatR dispatch + behavior chain is roughly **1.5–3.0 µs**. For your SaaS app hitting PostgreSQL, this is functionally invisible.

**When it matters**: High-throughput event processing (>10,000 req/s), hot loops, or extremely latency-sensitive paths. None of these are typical for a task-management SaaS.

---

## 2. Memory & Startup Tax

### Assembly Scanning Cost

MediatR's `RegisterServicesFromAssembly()` in [DependencyInjection.cs](file:///d:/Work/Progresium/ProgresiumToDo/src/ProgresiumToDo.Application/DependencyInjection.cs) performs reflection-based scanning at startup:

| Metric | Your Project (35 handlers) | At 100 handlers | At 300 handlers |
|---|---|---|---|
| **Cold start scan time** | ~15–40 ms | ~50–120 ms | ~150–350 ms |
| **Memory for type metadata** | ~0.5 MB | ~1.2 MB | ~3–4 MB |
| **DI container registrations** | ~45 entries | ~120 entries | ~350 entries |

### Impact by Deployment Model

| Environment | Impact | Verdict |
|---|---|---|
| **Traditional server / K8s** | Negligible (cold start amortized) | ✅ No concern |
| **Azure Functions (Consumption)** | Adds 15-40ms to cold start | ⚠️ Noticeable but minor |
| **AWS Lambda (.NET 10)** | Adds to already-slow JIT startup | ⚠️ Consider source-gen |
| **Native AOT** | **MediatR does NOT support AOT** due to reflection | ❌ Blocker |
| **Trimmed publish** | MediatR breaks with aggressive trimming | ❌ Blocker |

> [!CAUTION]
> **If you ever plan to deploy as Native AOT or use trimming**, MediatR is a hard blocker. The source-generated Mediator library has full AOT support. Direct services also work perfectly with AOT.

### Source-Generated Mediator Startup

The source-generated Mediator emits all wiring at compile time. There is **zero runtime scanning**:

| Metric | Source-Generated Mediator |
|---|---|
| **Startup scan cost** | 0 ms |
| **Compile-time cost** | +1–3s to build (one-time, incremental after) |
| **AOT compatible** | ✅ Full support |
| **Trimming compatible** | ✅ Full support |

---

## 3. The Debugging Tax

### Stack Trace Comparison

When you hit F5 and "Step Into" `Mediator.Send(command)`:

**MediatR stack (your current setup):**
```
MyController.CreateTask()
  → MediatR.Mediator.Send()
    → MediatR.Internal.RequestHandlerWrapperImpl.HandleAsync()
      → ValidationBehavior.Handle()
        → LoggingBehavior.Handle()
          → UnitOfWorkBehavior.Handle()
            → EntitlementBehavior.Handle()
              → CreateTaskCommandHandler.Handle() ← your code
```

**Direct service call:**
```
MyController.CreateTask()
  → TaskService.CreateAsync() ← your code
```

**Source-generated Mediator:**
```
MyController.CreateTask()
  → Mediator.Send()                             ← generated code (readable)
    → ValidationBehavior.Handle()
      → LoggingBehavior.Handle()
        → UnitOfWorkBehavior.Handle()
          → EntitlementBehavior.Handle()
            → CreateTaskCommandHandler.Handle()  ← your code
```

### Debugging Cost Matrix

| Aspect | MediatR | Source-Gen Mediator | Direct Service |
|---|---|---|---|
| **"Go to Definition"** on Send | Goes to interface | Goes to generated code | Goes to method |
| **"Find All References"** for a handler | ❌ Finds nothing (resolved at runtime) | ⚠️ Partial (via generated code) | ✅ Full IDE support |
| **Stack trace readability** | 4–6 framework frames | 2–3 generated frames | 0 extra frames |
| **Breakpoint on "who calls this handler?"** | Must know the Request type | Same | Standard breakpoint |
| **New developer onboarding** | Must learn MediatR pattern | Must learn Mediator + source gen | Standard .NET patterns |
| **Exception origin clarity** | Obscured by wrapping delegates | Slightly clearer | Fully transparent |

> [!NOTE]
> The debugging overhead is **mostly a developer-experience cost**, not a runtime cost. For a solo dev or small team (1-3), this friction is significant. For larger teams with established patterns, it becomes muscle memory.

---

## 4. Boilerplate vs. Decoupling Trade-off

### Files Per Feature

For a single action like "Create Task", here's the file count:

| Approach | Files | Lines of Boilerplate |
|---|---|---|
| **MediatR (your current)** | 3–4: Command, Handler, Validator, (Response DTO) | ~40–60 |
| **Source-gen Mediator** | 3–4: Same structure, different base types | ~35–55 |
| **Direct Service** | 1–2: Service method + (Validator) | ~15–25 |

### Your project: 35 handlers × boilerplate cost

| Metric | MediatR | Direct Service | Delta |
|---|---|---|---|
| **Total boilerplate files** | ~105–140 files | ~35–50 files | –70–90 files |
| **Total boilerplate lines** | ~1,400–2,100 LOC | ~525–875 LOC | –875–1,225 LOC |

### When Does Decoupling Pay Off?

```mermaid
graph LR
    A["1-15 handlers<br/><b>Direct services win</b><br/>Overhead not justified"] --> B["15-50 handlers<br/><b>Crossover zone</b><br/>Your project is HERE"]
    B --> C["50-150 handlers<br/><b>Mediator pattern wins</b><br/>Pipeline behaviors save time"]
    C --> D["150+ handlers<br/><b>Mediator essential</b><br/>Cross-cutting at scale"]
    style B fill:#f5c542,stroke:#333,color:#000
```

### Your Position: 35 Handlers — The Crossover Zone

At 35 handlers, you're in the **crossover zone** where both approaches are defensible. Key factors:

| Factor | Favors Mediator | Favors Direct |
|---|---|---|
| You have 4 pipeline behaviors | ✅ Each behavior applies to all commands automatically | ❌ Must apply manually or via decorators |
| Entitlement behavior uses marker interfaces | ✅ Clean opt-in via `IEntitledRequest` | ⚠️ Requires custom attribute + filter |
| UnitOfWork behavior discriminates queries vs commands | ✅ Already solved cleanly | ⚠️ Must reimplement per service |
| Team size is small | ❌ Boilerplate friction is felt | ✅ Simpler mental model |
| You use `Result<T>` pattern throughout | Neutral | Neutral |

> [!IMPORTANT]
> **Your 4 pipeline behaviors are the strongest argument to keep a mediator pattern.** The `UnitOfWorkBehavior` alone — which auto-wraps commands in transactions and skips queries — would require significant refactoring to replicate without a mediator.

---

## 5. Refactoring Strategy: If You Decide to Leave MediatR

### Option A: Migrate to Source-Generated Mediator (Lowest Risk)

**Effort**: ~2–4 hours for your project size

| Step | Detail |
|---|---|
| 1. Swap NuGet | Replace `MediatR` with `Mediator` (Martin Othamar) |
| 2. Update abstractions | Change `IRequest<T>` → `IRequest<T>` (Mediator namespace), `IPipelineBehavior` → `IPipelineBehavior` |
| 3. Update DI registration | Replace `AddMediatR()` with `AddMediator()` — source gen handles scanning |
| 4. Return types | Change `Task<T>` → `ValueTask<T>` in handlers (perf benefit) |
| 5. Build — verify generated code | Check `obj/Generated/` for source output |
| 6. Run tests | All existing tests should pass with minimal changes |

**What you keep**: All 4 pipeline behaviors, same folder structure, same patterns.
**What you gain**: AOT support, faster dispatch, lower allocations, compile-time handler validation.

---

### Option B: Remove Mediator Entirely — "Service-per-Feature" (High Effort)

**Effort**: ~2–4 days for your project size

#### Step 1: Replace Pipeline Behaviors with Alternatives

| Current Behavior | Replacement Strategy |
|---|---|
| [ValidationBehavior](file:///d:/Work/Progresium/ProgresiumToDo/src/ProgresiumToDo.Application/Abstractions/Behaviors/ValidationBehavior.cs) | ASP.NET endpoint filter or `IValidationService` called at start of each service method |
| [LoggingBehavior](file:///d:/Work/Progresium/ProgresiumToDo/src/ProgresiumToDo.Application/Abstractions/Behaviors/LoggingBehavior.cs) | ASP.NET middleware (already have request logging), or decorator pattern |
| [UnitOfWorkBehavior](file:///d:/Work/Progresium/ProgresiumToDo/src/ProgresiumToDo.Application/Abstractions/Behaviors/UnitOfWorkBehavior.cs) | `IUnitOfWork` injected into services, explicit `SaveChanges + Commit` |
| [EntitlementBehavior](file:///d:/Work/Progresium/ProgresiumToDo/src/ProgresiumToDo.Application/Abstractions/Behaviors/EntitlementBehavior.cs) | `[Authorize(Policy = "...")]` attribute + custom `IAuthorizationHandler`, or `IEntitlementService` guard at top of service methods |

#### Step 2: Create Feature Services

```csharp
// Before: 3 files (CreateTaskCommand.cs, CreateTaskCommandHandler.cs, CreateTaskCommandValidator.cs)
// After: 1 service file + 1 validator (optional)

public class TaskService(
    ITaskRepository taskRepository,
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork,
    IEntitlementService entitlementService,
    IValidator<CreateTaskRequest> validator,
    ILogger<TaskService> logger)
{
    public async Task<Result<Guid>> CreateAsync(CreateTaskRequest request, CancellationToken ct)
    {
        // Validation (was: ValidationBehavior)
        await validator.ValidateAndThrowAsync(request, ct);

        // Entitlement (was: EntitlementBehavior)
        await entitlementService.EnsureAllowedAsync(userId, "TaskCreation", ct);

        // Business logic (was: Handler.Handle)
        var task = Task.Create(request.Title, ...);
        taskRepository.Add(task);

        // UoW (was: UnitOfWorkBehavior)
        await unitOfWork.SaveChangesAsync(ct);
        await unitOfWork.CommitAsync(ct);

        return Result.Success(task.Id);
    }
}
```

#### Step 3: Update Controllers

```diff
// Before
-public TaskController(IMediator mediator) : base(mediator) { }
-[HttpPost] public Task<IActionResult> Create(CreateTaskCommand cmd)
-    => HandleCommand(Mediator.Send(cmd));

// After
+public TaskController(TaskService taskService) { ... }
+[HttpPost] public Task<IActionResult> Create(CreateTaskRequest req)
+    => HandleResult(taskService.CreateAsync(req));
```

#### Step 4: Delete MediatR infrastructure
- Remove `ICommand`, `IQuery`, `ICommandHandler`, `IQueryHandler` abstractions
- Remove all 4 behavior files
- Remove `MediatR` NuGet package
- Remove `AddMediatR()` from DI

> [!WARNING]
> **The UnitOfWorkBehavior is the hardest to replace.** It currently auto-wraps all commands in a transaction and auto-skips queries. Without it, you must manually manage transactions in every command service, or build a custom decorator/interceptor — which is essentially rebuilding a simpler mediator.

---

## 6. Decision Matrix

### Primary Decision Axes

| | **Low Perf Requirements**<br/>(SaaS, CRUD, admin panels) | **Medium Perf Requirements**<br/>(API gateway, moderate throughput) | **High Perf Requirements**<br/>(real-time, event-driven, >10K req/s) |
|---|---|---|---|
| **Small** (1–15 handlers) | Direct services | Direct services | Direct services |
| **Medium** (15–50 handlers) | MediatR or Source-Gen ⚖️ | Source-Gen Mediator | Direct services or Source-Gen |
| **Large** (50–150 handlers) | MediatR | Source-Gen Mediator | Source-Gen Mediator |
| **Enterprise** (150+ handlers) | MediatR | Source-Gen Mediator | Source-Gen Mediator |

### Deployment-Specific Overrides

| Constraint | Forced Decision |
|---|---|
| **Native AOT required** | ❌ MediatR → Use Source-Gen or Direct |
| **Trimming required** | ❌ MediatR → Use Source-Gen or Direct |
| **Serverless (cold-start sensitive)** | ⚠️ Prefer Source-Gen over MediatR |
| **Must keep pipeline behaviors** | ⚠️ Stay with MediatR or Source-Gen (both support `IPipelineBehavior`) |
| **Team unfamiliar with mediator pattern** | Direct services |

### Recommendation for ProgresiumToDo

| Criteria | Your Situation | Verdict |
|---|---|---|
| Handler count | 35 (growing) | Mediator justified |
| Pipeline behaviors | 4 (heavy reliance on UoW + Entitlement) | **Strong case for mediator** |
| Performance needs | SaaS CRUD — not latency-critical | MediatR overhead is irrelevant |
| AOT / serverless | Not currently, but future-proofing | Source-Gen is safer |
| Team size | Small | Slight friction from mediator boilerplate |
| Existing code investment | 35 handlers + 4 behaviors already built | High switching cost |

> [!TIP]
> **Recommended path**: **Migrate from MediatR to Source-Generated Mediator (Option A)**. You keep all your pipeline behaviors, folder structure, and patterns — but gain AOT readiness, faster dispatch, compile-time handler validation, and lower memory allocations. The migration effort is ~2–4 hours with near-zero risk.
>
> Alternatively, if you're satisfied with MediatR's current behavior and have no AOT/serverless plans, **staying on MediatR v14 is perfectly fine** for your project scale.
>
> **Avoid Option B** (full removal) unless you have a strong philosophical preference against the mediator pattern. The refactoring cost (~2–4 days) isn't justified by the performance gains for a CRUD SaaS.
