using System.Collections.Generic;
using Live.Web.Models;

namespace Live.Web.Store
{
    public class UserStore
    {
        public List<TestUser> Users => new List<TestUser>
        {
            new TestUser(){Id = 1, Name = "卡萨丁"},
            new TestUser(){Id = 2, Name = "希维尔"},
            new TestUser(){Id = 3, Name = "茂凯"},
            new TestUser(){Id = 4, Name = "潘森"},
            new TestUser(){Id = 5, Name = "波比"},
            new TestUser(){Id = 6, Name = "拉克丝"},
            new TestUser(){Id = 7, Name = "索拉卡"},
            new TestUser(){Id = 8, Name = "娑娜"},
            new TestUser(){Id = 9, Name = "伊泽瑞尔"},
            new TestUser(){Id = 10, Name = "雷克顿"},
        };
    }
}
