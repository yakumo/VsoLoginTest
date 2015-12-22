using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    [DataContract]
    public class VsoPreference : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChange(string propertyName, Action propertyChangeAction)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            propertyChangeAction.Invoke();
            IsDirty = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void OnPropertyChange(string[] propertyName, Action propertyChangeAction)
        {
            foreach (string pn in propertyName)
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(pn));
            }
            propertyChangeAction.Invoke();
            IsDirty = true;
            foreach (string pn in propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pn));
            }
        }

        VsoToken _token = null;
        [DataMember]
        public VsoToken Token
        {
            get { return _token; }
            set
            {
                OnPropertyChange("Token", () => { _token = value; });
            }
        }

        VsoAccounts _accounts = new VsoAccounts();
        [DataMember]
        public VsoAccounts Accounts
        {
            get { return _accounts; }
            set
            {
                OnPropertyChange("Accounts", () => { _accounts = value; });
            }
        }

        string _selectedAccountId = null;
        [DataMember]
        public string SelectedAccountId
        {
            get { return _selectedAccountId; }
            set
            {
                OnPropertyChange(new string[] { "SelectedAccountId", "SelectedAccount" }, () => { _selectedAccountId = value; });
            }
        }

        public VsoAccount SelectedAccount
        {
            get
            {
                return (from a in Accounts where a.Id == _selectedAccountId select a).First();
            }
            set
            {
                SelectedAccountId = value.Id;
            }
        }

        public bool IsLoaded { get; set; } = false;
        public bool IsDirty { get; protected set; } = false;
        public void CleanDirty()
        {
            IsDirty = false;
        }

        public void CopyTo(VsoPreference other)
        {
            other.Token = Token;
            other.Accounts = Accounts;
            other.IsLoaded = IsLoaded;
            other.CleanDirty();
        }
    }

    public interface IVsoPreferenceStore
    {
        VsoPreference Preference { get; }

        Task<bool> Load();
        Task<bool> Save();
    }
}
