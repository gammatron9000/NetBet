module DbUpgrader

open DbUp
open System
open System.Reflection

let upgrade (connectionString: string) =
    let ass = Assembly.GetExecutingAssembly()
    EnsureDatabase.For.SqlDatabase(connectionString) |> ignore
    let timeout = Nullable(TimeSpan(0L))
    let upgrader = 
        DeployChanges.To
                     .SqlDatabase(connectionString)
                     .WithScriptsAndCodeEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                     .WithTransaction()
                     .WithExecutionTimeout(timeout)
                     .LogToConsole()
                     .LogToTrace()
                     .Build()

    upgrader.PerformUpgrade()
