namespace SafouaneAntoineService.DAL.IDAL
{
    public interface IServiceCategoryDAL
    {
        public List<(int id, string name)> GetCategories();
    }
}
