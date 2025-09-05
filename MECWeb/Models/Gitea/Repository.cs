namespace MECWeb.Models.Gitea
{
    public class Repository
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Html_Url { get; set; } = string.Empty;
        public string Full_Name { get; set; } = string.Empty;
        public bool Private { get; set; }
        public Owner? Owner { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public string Clone_Url { get; set; } = string.Empty;
        public string Ssh_Url { get; set; } = string.Empty;
        public int Size { get; set; }
        public string Language { get; set; } = string.Empty;
        public bool Fork { get; set; }
        public int Stars_Count { get; set; }
        public int Forks_Count { get; set; }
        public string Default_Branch { get; set; } = string.Empty;

        // Weitere Properties die Gitea manchmal zurückgibt
        public bool Empty { get; set; }
        public bool Template { get; set; }
        public bool Archived { get; set; }
        public bool Mirror { get; set; }
        public int Open_Issues_Count { get; set; }
        public int Open_Pr_Counter { get; set; }
        public int Release_Counter { get; set; }
    }

    public class Owner
    {
        public string Login { get; set; } = string.Empty;
        public string Avatar_Url { get; set; } = string.Empty;
        public string Html_Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Full_Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}