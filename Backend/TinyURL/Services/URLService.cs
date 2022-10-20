using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TinyURL.Models;

namespace TinyURL.Services
{
    public class URLService : IURLService
    {
        private readonly IMongoCollection<UrlDbModel> urlCollection;
        public URLService(IOptions<UrlDatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var documentStr = "{ createIndexes: 'UrlMapping', indexes: [ { key: { CodedUrl : 1 }, name: 'CodedUrl-uniq-1', unique: true } ] }";
            var document = BsonDocument.Parse(documentStr);
            _ = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName).RunCommand<BsonDocument>(document);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            urlCollection = mongoDatabase.GetCollection<UrlDbModel>(databaseSettings.Value.TinyUrlCollectionName);
        }

        public async Task<string> GetTinyUrl(URLInputModel model)
        {
            if (string.IsNullOrEmpty(model.Uri))
                return string.Empty;
            //string codedUrl = Sha256Encode(model?.Uri ?? "");
            string codedUrl = Base62Encode(UrlToId(model?.Uri ?? ""));
            try
            {
                await urlCollection.InsertOneAsync(
                    new UrlDbModel
                    {
                        CodedUrl = codedUrl,
                        LongUrl = model?.Uri ?? "",
                    });
                return codedUrl;
            }catch (MongoWriteException)
            {
                return (await urlCollection.Find(x => string.Equals(x.LongUrl, model!.Uri)).FirstOrDefaultAsync())?.CodedUrl ?? "";
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static string Sha256Encode(string url)
        {
            byte[] data = Encoding.UTF8.GetBytes(url);
            var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(data);
            return ByteArrayToTinyUrl(hash);
        }

        private static string ByteArrayToTinyUrl(byte[] array)
        {
            string output = "";
            for (int i = 0; i < array.Length; i++)
            {
                output+=$"{array[i]:X2}";
                if ((i % 4) == 3) break;
            }
            return output;
        }

        public async Task<string> GetLongUrl(string codedUri)
        {
            if (string.IsNullOrEmpty(codedUri))
                return string.Empty;
            try
            {
                return (await urlCollection.Find(x => x.CodedUrl == codedUri).FirstOrDefaultAsync())?.LongUrl ?? "";
            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private string Base62Encode(ulong url)
        {
            string baseString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string encodedString = "";
            while(url > 0)
            {
                ulong base62 = url % 62;
                encodedString = baseString[(int)base62] + encodedString;
                url /= 62;
            }

            return encodedString;
        }

        
        private ulong UrlToId(string url)
        {
            ulong id = 0;
            for (int i = 0; i < url.Length; i++)
            {
                if ('a' <= url[i] && url[i] <= 'z')
                    id = id * 62 + url[i] - 'a';
                if ('A' <= url[i] && url[i] <= 'Z')
                    id = id * 62 + url[i] - 'A' + 26;
                if ('0' <= url[i] && url[i] <= '9')
                    id = id * 62 + url[i] - '0' + 52;
            }

            return id;
        }
    }
}

