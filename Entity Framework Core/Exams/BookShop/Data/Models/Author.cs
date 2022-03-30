namespace BookShop.Data.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class Author
    {
        public Author()
        {
            this.AuthorsBooks = new HashSet<AuthorBook>();
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        [JsonIgnore]
        public ICollection<AuthorBook> AuthorsBooks { get; set; }   

    }
}
