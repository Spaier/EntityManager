 # EntityManager.Extensions

Extensions that allows you to find entity by primary key with AsNoTracking() or Include() query.

```
...
public class TodoController : Controller
{
    private AppDbContext _context;
    ....
    public Task<IActionResult> Get(long id) => Ok(await _context.Todos
        .AsNoTracking()
        .Include(todo => todo.IncludedProperty)
        .GetByPrimaryKeyAsync(_context, id));
}

```
