namespace AMI_WebAPI.Models.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string DisplayName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        //public string PasswordHash { get; set; } = null!;

        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; }

        //public virtual ICollection<Consumer> ConsumerCreatedByNavigations { get; set; } = new List<Consumer>();

       // public virtual ICollection<Consumer> ConsumerUpdatedByNavigations { get; set; } = new List<Consumer>();
    }
}
