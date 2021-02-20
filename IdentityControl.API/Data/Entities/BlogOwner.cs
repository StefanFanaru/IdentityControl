namespace IdentityControl.API.Data.Entities
{
    public class BlogOwner
    {
        public string BlogId { get; set; }
        public Blog Blog { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}