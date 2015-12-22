using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace CommonData
{
    [DataContract]
    public class VsoAccount
    {
        [DataMember(Name = "accountId")]
        public string Id { get; set; }
        [DataMember(Name = "accountUri")]
        public string Uri { get; set; }
        [DataMember(Name = "accountName")]
        public string Name { get; set; }
        [DataMember(Name = "organizationName")]
        public string OrganizationName { get; set; }
        [DataMember(Name = "accountType")]
        public string AccountType { get; set; }
        [DataMember(Name = "accountOwner")]
        public string Owner { get; set; }
        [DataMember(Name = "createdBy")]
        public string CreatedBy { get; set; }
        [DataMember(Name = "createdDate")]
        public string CreatedDate { get; set; }
        [DataMember(Name = "accountStatus")]
        public string Status { get; set; }
        [DataMember(Name = "lastUpdatedBy")]
        public string LastUpdatedBy { get; set; }
        [DataMember(Name = "properties")]
        public string[] Properties { get; set; }
        [DataMember(Name = "lastUpdatedDate")]
        public string LastUpdatedDate { get; set; }
    }

    [CollectionDataContract(ItemName = "Account")]
    public class VsoAccounts : ObservableCollection<VsoAccount>
    {
    }

    [DataContract]
    public class VsoApiAccounts
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "value")]
        public VsoAccounts Accounts { get; set; } = new VsoAccounts();
    }

    [DataContract]
    public class VsoMyProfile
    {
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }
        [DataMember(Name = "publicAlias")]
        public string PublicAlias { get; set; }
        [DataMember(Name = "emailAddress")]
        public string EmailAddress { get; set; }
        [DataMember(Name = "coreRevision")]
        public int CoreRevision { get; set; }
        [DataMember(Name = "timeStamp")]
        public string TimeStamp { get; set; }
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "revision")]
        public int Revision { get; set; }
    }
}
