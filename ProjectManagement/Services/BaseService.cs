using ProjectManagement.Api.Extensions;
using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace ProjectManagement.Api.Services
{
    public class BaseService<TService, TEntity, TDto, TResponse>(ILogger<TService> logger,
                            IRepository<TEntity> repository) 
        where TDto : class
        where TService : class
        where TEntity : BaseEntity, new()
        where TResponse : BaseResponse<TDto>, new()
    {
        private readonly ILogger<TService> _logger = logger;
        private readonly IRepository<TEntity> _repository = repository;

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

                request.GetType().GetProperties().ToList().ForEach(prop =>
                {
                    object? value = prop.GetValue(request);
                    PropertyInfo? property = entity
                                                .GetType()
                                                .GetProperty(prop.Name);

                    //if(property != null && property.PropertyType.Namespace != "System.Collections.Generic")
                        //property?.SetValue(entity, prop.GetValue(request));

                    if (IsPropertyValid(property, value))
                        property?.SetValue(entity, value);
                });

                await _repository.Update(entity);
                
                TDto? dto = (TDto?)Activator.CreateInstance(typeof(TDto), entity);

                ArgumentNullException.ThrowIfNull(dto);

                response.ListResponse.Add(dto);
                response.IsSuccess = true;

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

        private static bool IsPropertyValid(PropertyInfo? property, object? value)
        {
            if (property == null
            || property.PropertyType.IsGenericType
            || !property.PropertyType.IsValueType
            || value == null
            || value == default)
                return false;

            return true;
        }
    }
}
