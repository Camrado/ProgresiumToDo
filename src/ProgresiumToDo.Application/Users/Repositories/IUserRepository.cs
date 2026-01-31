using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Users.Repositories;

public interface IUserRepository {
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task AcquireUserLockAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> IsEmailVerifiedAsync(Guid userId, CancellationToken cancellationToken = default);
    
    void Add(User user);

    void Delete(User user);
}