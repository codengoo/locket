using System.ComponentModel.DataAnnotations;

namespace Locket.UserLocket.DTOs
{
    public class UserDTO
    {
        public class IGetUsersByIDs
        {
            [Required]
            public string[] Ids { get; set; }
        }

        public class IGetUsersById
        {
            public string ID { get; set; }
        }
    }
}
