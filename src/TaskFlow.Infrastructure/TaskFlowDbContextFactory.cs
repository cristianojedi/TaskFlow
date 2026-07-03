using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskFlow.Infrastructure;

public class TaskFlowDbContextFactory : IDesignTimeDbContextFactory<TaskFlowDbContext>
{
    public TaskFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TaskFlowDbContext>();
        optionsBuilder.UseSqlite("Data Source=taskflow.db");
        return new TaskFlowDbContext(optionsBuilder.Options);
    }
}
