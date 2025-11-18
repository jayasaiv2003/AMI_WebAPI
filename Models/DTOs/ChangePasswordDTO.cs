namespace AMI_WebAPI.Models.DTOs
{
    public class ChangePasswordDTO
    {
        public long ConsumerId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

}
