using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HelpingModel
{ 
    public class UserProfile
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int PortalId { get; set; }
        public UserProfile()
        {
            PortalId = 1000;
        }
    }

    public class SocailSignInRQ
    {
        public int AuthType { get; set; }
        public string SocialKey { get; set; }
    }
    public class SocailSignInRS
    {

        public int PortalId { get; set; }
        public string EmailId { get; set; }
        public string SocialId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public SocialMediaLogin Provider { get; set; }
    }
    public class FacebookUser
    {
        public string email { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string id { get; set; }
    }
    public class UserProfileResponse
    {
        public StatusUser Status { get; set; }
        public Result Result { get; set; }
    }
    public class StatusUser
    {
        public bool IsSuccess { get; set; }
        public Errors Error { get; set; }
    }
    public class Result
    {
        public string Message { get; set; }
        public int Points { get; set; }
        public Usercontext UserContext { get; set; }
        public Token Token { get; set; }
    }

    public class Rootobject
    {
        public Statususer Status { get; set; }
        public Result Result { get; set; }
    }

    public class Statususer
    {
        public bool IsSuccess { get; set; }
        public object Error { get; set; }
    }
    public class Usercontext
    {
        public Loginuserdetail LoginUserDetail { get; set; }
        public string CacheExpiryTime { get; set; }
        public object ForgotPasswordToken { get; set; }
        public object RegisterUserToken { get; set; }
    }

    public class Loginuserdetail
    {
        public int UserId { get; set; }
        public int PortalId { get; set; }
        public string EmailId { get; set; }
        public string ExpiredOn { get; set; }
    }

    public class Token
    {
        public string Key { get; set; }
        public string Expiry { get; set; }
    }
    public class Errors
    {
        public int Code { get; set; }
        public string Description { get; set; }
    }
}
