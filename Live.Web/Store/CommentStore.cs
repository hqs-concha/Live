using System.Collections.Generic;
using Live.Web.Models;

namespace Live.Web.Store
{
    public class CommentStore
    {
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
