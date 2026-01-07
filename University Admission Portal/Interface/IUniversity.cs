namespace University_Admission_Portal.Interface
{
    public interface IUniversity<TEntity, TKey>
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(TKey id);

       // Task<TEntity> GetByName(string name);

        Task<TEntity> Add(TEntity entity);

        Task Update(TKey id, TEntity entity);
        Task Delete(TKey id);


    }
}
