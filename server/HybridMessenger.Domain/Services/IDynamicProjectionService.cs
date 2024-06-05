namespace HybridMessenger.Domain.Services
{
    public interface IDynamicProjectionService
    {
        IEnumerable<object> ProjectToDynamic<T, TDto>(IEnumerable<T> entities, IEnumerable<string> fieldsToInclude);
    }
}
