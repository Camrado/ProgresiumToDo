using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Infrastructure.Repositories.Tasks;

internal sealed class TaskItemRepository : Repository<TaskItem>, ITaskItemRepository
{
    public TaskItemRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
