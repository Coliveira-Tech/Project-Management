using Microsoft.Extensions.Primitives;
using ProjectManagement.Api.Extensions;
using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Models;
using System.Linq.Expressions;
using System.Reflection;
using Tasks = System.Threading.Tasks;

namespace ProjectManagement.Api.Services
{
    public class BaseService<TService, TEntity, TDto, TResponse>(ILogger<TService> logger,
                            IRepository<TEntity> repository,
                            IHttpContextAccessor httpContextAccessor) 
        where TDto : class
        where TService : class
        where TEntity : BaseEntity, new()
        where TResponse : BaseResponse<TDto>, new()
    {
        private readonly ILogger<TService> _logger = logger;
        private readonly IRepository<TEntity> _repository = repository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        protected virtual async Tasks.Task AfterUpdate(TEntity entity, List<Tuple<PropertyInfo?, object?, object?>> updatedProperties) => await Tasks.Task.CompletedTask;
        protected virtual async Tasks.Task AfterInsert(TEntity entity) => await Tasks.Task.CompletedTask;
        protected virtual async Tasks.Task AfterDelete(TEntity entity) => await Tasks.Task.CompletedTask;

        public virtual async Task<TResponse> GetById(Guid id)
        {
            return await GetBy(x => x.Id == id);
        }

        public virtual async Task<TResponse> GetBy(Expression<Func<TEntity, bool>> predicate)
        {
            TResponse response = new();
            
            try
            {
                IEnumerable<TEntity> result = await _repository
                                                        .Get(predicate);

                if (!result.Any())
                {
                    response.Message.Add($"{typeof(TEntity).Name} not found");
                    response.ErrorCode = StatusCodes.Status404NotFound;
                    return response;
                }

                response.ListResponse.AddRange(result.ToDtoList<TEntity, TDto>());
                response.IsSuccess = true;

                _logger.LogInformation("Successfully retrieve {0} | Method: {1}"
                                    , typeof(TEntity).Name
                                    , GetType().Name);
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to retrieve {typeof(TEntity).Name}");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

        public virtual async Task<TResponse> GetAll()
        {
            TResponse response = new();

            try
            {
                IEnumerable<TEntity> result = await _repository
                                                        .GetAll();

                if (!result.Any())
                {
                    response.Message.Add($"{typeof(TEntity).Name} not found");
                    response.ErrorCode = StatusCodes.Status404NotFound;
                    return response;
                }

                response.ListResponse.AddRange(result.ToDtoList<TEntity, TDto>());
                response.IsSuccess = true;

                _logger.LogInformation("Successfully retrieve {0} '{1}' | Method: {2}"
                                    , response.ListResponse.Count
                                    , typeof(TEntity).Name
                                    , GetType().Name);
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to retrieve {typeof(TEntity).Name}");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

        public virtual async Task<TResponse> Insert<TRequest>(TRequest request)
        {
            TResponse response = new();

            try
            {
                TEntity? entity = (TEntity?)Activator.CreateInstance(typeof(TEntity), request);

                ArgumentNullException.ThrowIfNull(entity);

                await _repository.Insert(entity);

                TDto? dto = (TDto?)Activator.CreateInstance(typeof(TDto), entity);
                
                ArgumentNullException.ThrowIfNull(dto);

                response.ListResponse.Add(dto);
                response.IsSuccess = true;

                await AfterInsert(entity);

                _logger.LogInformation("Successfully inserted {0} {1} | Method: {2}"
                                    , typeof(TEntity).Name
                                    , entity.Id
                                    , GetType().Name);
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to insert {typeof(TEntity).Name}"); 
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

        public virtual async Task<TResponse> InsertRange<TRequest>(List<TRequest> request)
        {
            TResponse response = new();

            try
            {
                List<TEntity> entities = [];

                request.ForEach(req =>
                {
                    TEntity? entity = (TEntity?)Activator.CreateInstance(typeof(TEntity), req);
                    ArgumentNullException.ThrowIfNull(entity);
                    entities.Add(entity);
                });

                ArgumentNullException.ThrowIfNull(entities);

                await _repository.InsertRange(entities);

                List<TDto> dtos = [];

                entities.ForEach(entity =>
                {
                    TDto? dto = (TDto?)Activator.CreateInstance(typeof(TDto), entity);
                    ArgumentNullException.ThrowIfNull(dto);
                    dtos.Add(dto);
                });

                ArgumentNullException.ThrowIfNull(dtos);

                response.ListResponse.AddRange(dtos);
                response.IsSuccess = true;

                _logger.LogInformation("Successfully inserted {0} {1} | Method: {2}"
                                    , entities.Count
                                    , typeof(TEntity).Name
                                    , GetType().Name);
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to insert {typeof(TEntity).Name}");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }
        
        public virtual async Task<TResponse> Update<TRequest>(Guid id, TRequest request)
        {
            TResponse response = new();
            try
            {
                ArgumentNullException.ThrowIfNull(request);

                TEntity? entity = (await _repository.Get(x => x.Id == id)).FirstOrDefault();

                if (entity == null)
                {
                    response.Message.Add($"{typeof(TEntity).Name} not found");
                    response.ErrorCode = StatusCodes.Status404NotFound;
                    return response;
                }

                List<Tuple<PropertyInfo?, object?, object?>> updatedProperties = [];

                request.GetType().GetProperties().ToList().ForEach(prop =>
                {
                    object? newValue = prop.GetValue(request);
                    PropertyInfo? property = entity
                                                .GetType()
                                                .GetProperty(prop.Name);
                    object? oldValue = property?.GetValue(entity);
                                        

                    if (CanSetValue(property, oldValue, newValue))
                    {
                        updatedProperties.Add(new(property, oldValue, newValue));
                        property?.SetValue(entity, newValue);
                    }
                });

                await _repository.Update(entity);
                
                TDto? dto = (TDto?)Activator.CreateInstance(typeof(TDto), entity);

                ArgumentNullException.ThrowIfNull(dto);

                response.ListResponse.Add(dto);
                response.IsSuccess = true;

                await AfterUpdate(entity, updatedProperties);

                _logger.LogInformation("Successfully inserted {0} {1} | Method: {2}"
                                    , typeof(TEntity).Name
                                    , entity.Id
                                    , GetType().Name);
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to update {typeof(TEntity).Name}");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

        public virtual async Task<TResponse> Delete(Guid Id)
        {
            TResponse response = new();

            try
            {
                TEntity? entity = (await _repository.Get(x => x.Id == Id))
                                    .FirstOrDefault();

                if (entity == null)
                {
                    response.Message.Add($"{typeof(TEntity).Name} not found");
                    response.ErrorCode = StatusCodes.Status404NotFound;
                    return response;
                }

                await _repository.Delete(entity);

                TDto? dto = (TDto?)Activator.CreateInstance(typeof(TDto), entity);

                ArgumentNullException.ThrowIfNull(dto);

                response.ListResponse.Add(dto);
                response.IsSuccess = true;

                await AfterDelete(entity);

                _logger.LogInformation("Successfully delete {0} {1} | Method: {2}"
                                    , typeof(TEntity).Name
                                    , entity.Id
                                    , GetType().Name);
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to delete {typeof(TEntity).Name}");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

        public virtual async Task<TResponse> Delete(TEntity? entity)
        {
            TResponse response = new();

            try
            {
                if (entity == null)
                {
                    response.Message.Add($"{typeof(TEntity).Name} not found");
                    response.ErrorCode = StatusCodes.Status404NotFound;
                    return response;
                }

                await _repository.Delete(entity);

                TDto? dto = (TDto?)Activator.CreateInstance(typeof(TDto), entity);

                ArgumentNullException.ThrowIfNull(dto);

                response.ListResponse.Add(dto);
                response.IsSuccess = true;

                await AfterDelete(entity);

                _logger.LogInformation("Successfully delete {0} {1} | Method: {2}"
                                    , typeof(TEntity).Name
                                    , entity.Id
                                    , GetType().Name);
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to delete {typeof(TEntity).Name}");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

        public virtual async Task<TResponse> DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            TResponse response = new();

            try
            {
                IEnumerable<TEntity>? entities = await _repository.Get(predicate);

                if (entities == null)
                {
                    response.Message.Add($"{typeof(TEntity).Name} not found");
                    response.ErrorCode = StatusCodes.Status404NotFound;
                    return response;
                }

                await _repository.DeleteRange(entities);

                foreach (var entity in entities)
                {
                    TDto? dto = (TDto?)Activator.CreateInstance(typeof(TDto), entities);
                    ArgumentNullException.ThrowIfNull(dto);
                    response.ListResponse.Add(dto);
                }
                
                response.IsSuccess = true;

                _logger.LogInformation("Successfully delete {0} {1} | Method: {2}"
                                    , entities.Count()
                                    , typeof(TEntity).Name
                                    , GetType().Name);
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to delete {typeof(TEntity).Name}");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

        private static bool CanSetValue(PropertyInfo? property, object? oldValue, object? newValue)
        {
            bool stringIsNullOrEmpty = false;
            bool guidIsNullOrDefault = false;
            bool datetimeIsNullOrDefault = false;
            bool EnumIsDefault = false;

            if(property == null || newValue == null)
                return false;

            if(oldValue?.ToString() == newValue.ToString())
                return false;

            string type = newValue.GetType().IsEnum 
                        ? "Enum" 
                        : newValue.GetType().Name;

            switch (type)
            {
                case "String":
                    stringIsNullOrEmpty = string.IsNullOrEmpty(newValue?.ToString());
                    break;
                case "Guid":
                    guidIsNullOrDefault = Guid.Empty == (Guid)newValue;
                    break;
                case "DateTime":
                    datetimeIsNullOrDefault = DateTime.MinValue == (DateTime)newValue;
                    break;
                case "Enum":
                    EnumIsDefault = newValue?.ToString() == "None";
                    break;
                default:
                    break;
            }

            if (stringIsNullOrEmpty 
             || guidIsNullOrDefault 
             || datetimeIsNullOrDefault
             || EnumIsDefault)
                return false;

            return true;
        }

        protected Guid GetLoggedUserId()
        {
            string? userId = GetHeaders("LoggedUserId");
            return string.IsNullOrEmpty(userId) ? Guid.Empty : Guid.Parse(userId);
        }

        protected UserRole GetLoggedUserRole()
        {
            string? roleId = GetHeaders("LoggedUserRole");
            return string.IsNullOrEmpty(roleId) ? UserRole.None : (UserRole)int.Parse(roleId);
        }

        private string? GetHeaders(string key)
        {
            try
            {
                StringValues headerValue = new();

                IHeaderDictionary? headers = _httpContextAccessor.HttpContext?.Request.Headers;
                headers?.TryGetValue(key, out headerValue);

                return headerValue.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
