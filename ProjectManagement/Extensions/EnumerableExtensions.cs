namespace ProjectManagement.Api.Extensions
{
    public static class EnumerableExtensions
    {
        public static List<TDto> ToDtoList<TEntity, TDto>(this IEnumerable<TEntity> entityList)
        {
            List<TDto> newDtoList = [];

            entityList.ToList().ForEach(entity =>
            {
                TDto? dto = (TDto?)Activator.CreateInstance(typeof(TDto), entity);

                if (dto != null)
                    newDtoList.Add(dto);
            });

            return newDtoList;
        }
    }
}
