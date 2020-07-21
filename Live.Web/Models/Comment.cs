namespace Live.Web.Models
{
    public class Comment
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public string Description { get;set; }

        public Comment() { }

        public Comment(string group, string name, string desc)
        {
            Group = group;
            Name = name;
            Description = desc;
        }
    }
}
