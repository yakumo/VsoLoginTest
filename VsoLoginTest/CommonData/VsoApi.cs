using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    public class VsoApi
    {
        static HttpClient MakeClient(string accessToken)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            return client;
        }

        public static Task<VsoToken> GetAccessToken(string assertionCode)
        {
            return Task.Factory.StartNew<VsoToken>(() =>
            {
                HttpClient client = new HttpClient();
                FormUrlEncodedContent context = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                    new KeyValuePair<string, string>("client_assertion", Constants.SecretId),
                    new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                    new KeyValuePair<string, string>("assertion", assertionCode),
                    new KeyValuePair<string, string>("redirect_uri", Constants.CallbackUrl),
                });
                Task<HttpResponseMessage> msgres = client.PostAsync("https://app.vssps.visualstudio.com/oauth2/token", context);
                msgres.Wait();
                HttpResponseMessage msg = msgres.Result;
                if (msg.IsSuccessStatusCode)
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(VsoToken));
                    Task<Stream> streamres = msg.Content.ReadAsStreamAsync();
                    streamres.Wait();
                    VsoToken token = (VsoToken)serializer.ReadObject(streamres.Result);
                    return token;
                }
                return null;
            });
        }

        public static Task<VsoToken> RefreshAccessToken(VsoToken sourceToken)
        {
            return Task.Factory.StartNew<VsoToken>(() =>
            {
                Debug.WriteLine("token refresh");
                return null;
            });
        }

        public static Task<VsoAccounts> GetAccounts(string acceptToken)
        {
            if (String.IsNullOrWhiteSpace(acceptToken))
            {
                throw new ArgumentException();
            }
            return Task.Factory.StartNew<VsoAccounts>(() =>
            {
                HttpClient client = MakeClient(acceptToken);
                Task<HttpResponseMessage> rest = client.GetAsync("https://app.vssps.visualstudio.com/_apis/profile/profiles/me?api-version=1.0");
                rest.Wait();
                if (rest.IsCompleted && rest.Result != null && rest.Result.IsSuccessStatusCode)
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(VsoMyProfile));
                    Task<Stream> streamres = rest.Result.Content.ReadAsStreamAsync();
                    streamres.Wait();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        streamres.Result.CopyTo(ms);
                        byte[] buf = ms.ToArray();
                        Debug.WriteLine("accounts:" + Encoding.UTF8.GetString(buf));
                        ms.Seek(0, SeekOrigin.Begin);
                        VsoMyProfile profile = (VsoMyProfile)serializer.ReadObject(ms);
                        if (profile != null)
                        {
                            HttpClient client2 = MakeClient(acceptToken);
                            Task<HttpResponseMessage> msgt = client2.GetAsync(String.Format("https://app.vssps.visualstudio.com/_apis/Accounts?memberId={0}&api-version=1.0", profile.Id));
                            msgt.Wait();
                            if (msgt.Result != null && msgt.Result.IsSuccessStatusCode)
                            {
                                DataContractJsonSerializer serializer2 = new DataContractJsonSerializer(typeof(VsoApiAccounts));
                                Task<Stream> streamt = msgt.Result.Content.ReadAsStreamAsync();
                                streamt.Wait();
                                VsoApiAccounts accounts = (VsoApiAccounts)serializer2.ReadObject(streamt.Result);
                                if (accounts?.Count > 0)
                                {
                                    return accounts.Accounts;
                                }
                            }
                        }
                    }
                }
                return null;
            });
        }
    }
}
