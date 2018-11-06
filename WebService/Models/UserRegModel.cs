namespace WebService.Models {
    public class UserRegModel : UserLoginModel {
        public string Email { get; set; }
        public string Location { get; set; }

    }
}