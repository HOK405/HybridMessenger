using AutoMapper;
using HybridMessenger.Domain.Services;
using System.Dynamic;
using System.Reflection;

namespace HybridMessenger.Infrastructure.Services
{
    public class DynamicProjectionService : IDynamicProjectionService
    {
        private readonly IMapper _mapper;

        public DynamicProjectionService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<object> ProjectToDynamic<T, TDto>(IEnumerable<T> entities, IEnumerable<string> fieldsToInclude)
        {
            return entities.Select(entity =>
            {
                IDictionary<string, object> expando = new ExpandoObject();
                foreach (var fieldName in fieldsToInclude)
                {
                    var propertyInfo = typeof(TDto).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo != null)
                    {
                        var value = propertyInfo.GetValue(_mapper.Map<TDto>(entity), null);
                        expando.Add(fieldName, value);
                    }
                }
                return (object)expando;
            });
        }
    }
}
