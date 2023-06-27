using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace HyperPeer
{
    internal class Utils
    {

        private static string apiBase = "YOUR_API_BASE_URL_HERE";
        public static string SettingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HyperPeer");

        public class LoginResponseData
        {
            public LoginAccountData data {  get; set; }
        }

        public class PublicProfileResponseData
        {
            public LoginAccountData data { get; set; }
        }

        public class GuildPublicReponseData
        {
            public GuildPublicData data { get; set; }
        }

        public class ChannelReponseData
        {
            public ChannelData data { get; set; }
        }

        public class MessageResponseData
        {
            public MessageData data { get; set; }
        }

        public class LoginAccountData
        {
            public string _id {  get; set; }
            public string discriminator {  get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public string token {  get; set; }
            public string avatarURL { get; set; }
            public Timestamps timestamps {  get; set; }
            public List<string> guilds {  get; set; }
        }

        public class GuildPublicData
        {
            public string _id { get; set; }
            public string name { get; set; }
            public string ownerId { get; set; }
            public string iconURL { get; set; }
            public List<string> members {  get; set; }
            public List<string> channels {  get; set; }
            public Timestamps timestamps { get; set; }
        }

        public class ChannelData
        {
            public string _id { get; set; }
            public string name { get; set; }
            public string guildId { get; set; }
            public string type { get; set; }
            public List<string> messages { get; set; }
            public List<string> connectedUsers { get; set; }
            public Timestamps timestamps { get; set; }
        }

        public class MessageData
        {
            public string _id { get; set; }
            public string content { get; set; }
            public string authorId { get; set; }
            public string channelId { get; set; }
            public DateTime createdAt { get; set; }
        }

        public class Timestamps
        {
            public DateTime createdAt {  get; set; }
            public DateTime updatedAt {  get; set; }
        }

        public class Account
        {
            public string username { get; set; }
            public string discriminator { get; set; }
            public string token { get; set; }
            public string email { get; set; }
            public string accountId { get; set; }
            public string avatarURL { get; set; }
            public List<Guild> guilds {  get; set; }
            public Timestamps timestamps { get; set; }
        }

        public class Guild
        {
            public string id { get; set; }
            public string name {  get; set; }
            public string ownerId { get; set; }
            public string iconURL { get; set; }
            public Timestamps timestamps { get; set; }
            public List<string> members {  get; set; }
            public List<Channel> channels { get; set; }
        }

        public class Message
        {
            public string id {  get; set; }
            public string content {  get; set; }
            public string authorId {  get; set; }
            public string channelId {  get; set; }
            public DateTime createdAt {  get; set; }
        }

        public class Channel
        {
            public string id { get; set; }
            public string name {  get; set; }
            public string type {  get; set; }
            public Timestamps timestamps {  get; set; }
            public List<Message> messages {  get; set; }
            public List<string> connectedUsers { get; set; }
        }

        public class Config
        {
            public Account account {  get; set; }

        }
        
        public static Account parseAccountFromConfigString(string plainConfig)
        {
            return JsonConvert.DeserializeObject<Config>(plainConfig).account;
        }

        public static Config parseConfig(string plainConfig)
        {
            return JsonConvert.DeserializeObject<Config>(plainConfig);
        }

        public static bool saveConfig(Account account)
        {
            Config config = new Config()
            {
                account = account
            };
            string json = JsonConvert.SerializeObject(config);

            File.WriteAllText(@System.IO.Path.Combine(SettingsDir, "config.json"), json);
            return true;
        }

        public static LoginResponseData parseLoginResponseData(string loginResponseData)
        {
            return JsonConvert.DeserializeObject<LoginResponseData>(loginResponseData);
        }

        public static PublicProfileResponseData parsePublicProfileData(string publicProfileReponse)
        {
            return JsonConvert.DeserializeObject<PublicProfileResponseData>(publicProfileReponse);
        }

        public static GuildPublicReponseData parsePublicGuildData(string publicGuildReponse)
        {
            return JsonConvert.DeserializeObject<GuildPublicReponseData>(publicGuildReponse);
        }

        public static ChannelReponseData parseChannelData(string channelResponse)
        {
            return JsonConvert.DeserializeObject<ChannelReponseData>(channelResponse);
        }

        public static MessageResponseData parseMessageData(string messageResponse)
        {
            return JsonConvert.DeserializeObject<MessageResponseData>(messageResponse);
        }

        public static bool isEmail(string email)
        {
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public static Object sendLoginRequest(string email, string pass)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiBase + "/login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            string json = new JavaScriptSerializer().Serialize(new
            {
                email = email,
                password = pass
            });
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                return result;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
                return "400";
            }
        }

        public static Object sendRegisterRequest(string username, string email, string pass)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiBase + "/register");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            //ServicePointManager.SecurityProtocol = null; //SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            string json = new JavaScriptSerializer().Serialize(new
            {
                username = username,
                email = email,
                password = pass
            });
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                return result;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
                return "400";
            }
        }

        public static Object sendProfileRequest(string uToken)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiBase + "/profile");
            httpWebRequest.Headers.Set("x-access-token", uToken);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                return result;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
                return "400";
            }
        }

        public static Object sendPublicProfileRequest(string uid)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiBase + "/profile/public/" + uid);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                return result;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
                return "400";
            }
        }

        public static Object sendPublicGuildDataRequest(string gid, string uToken)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiBase + "/guild/" + gid);
            httpWebRequest.Headers.Set("x-access-token", uToken);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                return result;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
                return "400";
            }
        }

        public static Object sendChannelDataRequest(string cid, string gid, string uToken)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiBase + "/channel/" + cid);
            httpWebRequest.Headers.Set("x-access-token", uToken);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            string json = new JavaScriptSerializer().Serialize(new
            {
                guildId = gid
            });
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                return result;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
                return "400";
            }
        }

        public static Object sendMessageDataRequest(string mid, string cid, string gid, string uToken)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiBase + "/message/" + mid);
            httpWebRequest.Headers.Set("x-access-token", uToken);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            string json = new JavaScriptSerializer().Serialize(new
            {
                guildId = gid,
                channelId = cid
            });
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                return result;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
                return "400";
            }
        }
    }
}
