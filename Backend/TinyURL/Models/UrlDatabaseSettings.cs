using System;
namespace TinyURL.Models
{
    public class UrlDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string TinyUrlCollectionName { get; set; } = null!;
    }
}

