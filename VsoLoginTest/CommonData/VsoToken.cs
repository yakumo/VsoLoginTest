using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CommonData
{
    [DataContract]
    public class VsoToken
    {
        private DateTimeOffset _expireStart = DateTimeOffset.Now;
        private long _expireIn = 0;

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }
        [DataMember(Name = "expires_in")]
        public long ExpireIn { get { return _expireIn; } set { _expireIn = value; ExpireTarget = DateTimeOffset.Now; } }
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "expire_target")]
        public DateTimeOffset ExpireTarget { get { return _expireStart; } private set { _expireStart = value; } }
        public void StartExpire()
        {
            ExpireTarget = DateTimeOffset.Now;
        }

        public DateTimeOffset Expire { get { return ExpireTarget.AddSeconds(ExpireIn); } }
        public bool IsExpire
        {
            get
            {
                if (AccessToken == null || ExpireTarget == null || DateTimeOffset.Now.CompareTo(Expire) >= 0)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
