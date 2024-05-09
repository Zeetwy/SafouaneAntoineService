namespace SafouaneAntoineService.Models
{
    public class ServiceCategory
    {
        private string name;

        public string Name { get { return name; } }
        public ServiceCategory(string name)
        {
            this.name = name;
        }
    }
}
