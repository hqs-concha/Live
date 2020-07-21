using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Live.Web.Models
{
    public class UserContext
    {
        private readonly TestUser _userInfo;

        public UserContext(IHttpContextAccessor accessor)
        {
            var value = accessor.HttpContext.Session.GetString("user");
            _userInfo = string.IsNullOrEmpty(value) ? new TestUser() : JsonConvert.DeserializeObject<TestUser>(value);
        }

        public int Id => _userInfo.Id;
        public string Name => _userInfo.Name;
    }
}
