using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Freetime_Planner
{
    public class AccessTokens
    {
        public string _access_token { get; set; }
        public string _private_access_token { get; set; }

        public static string path = "Access_Tokens.json";

        public static void Upload()
        {
            var token_object = JsonConvert.DeserializeObject<AccessTokens>(File.ReadAllText(path));
            Bot._access_token = token_object._access_token;
            Bot._private_access_token = token_object._private_access_token;
        }
    }
}
