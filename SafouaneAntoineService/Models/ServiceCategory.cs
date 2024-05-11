namespace SafouaneAntoineService.Models
{
    public class ServiceCategory
    {
        private int id;
        private string name;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public ServiceCategory(string name)
        {
            this.name = name;
        }
    }
}
