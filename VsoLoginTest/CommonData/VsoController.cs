using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    public class VsoController
    {
        public event EventHandler NeedLoginEvent;
        public event EventHandler AccountSelectEvent;

        public static VsoController Controller { get; private set; } = new VsoController();

        public IVsoPreferenceStore PreferenceStore { get; private set; } = null;
        public VsoPreference Preference
        {
            get
            {
                return PreferenceStore?.Preference;
            }
        }

        public void Start(IVsoPreferenceStore preferenceStore)
        {
            PreferenceStore = preferenceStore;
            Preference.PropertyChanged += Preference_PropertyChanged;

            if (!Preference.IsLoaded)
            {
                PreferenceStore.Load().ContinueWith((res) =>
                {
                    if (res.IsCompleted)
                    {
                        if (res.Result)
                        {
                        }
                        else
                        {
                            NeedLoginEvent?.Invoke(this, new EventArgs());
                        }
                    }
                });
            }
        }

        private void Preference_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Token")
            {
                if (Preference?.Token != null && Preference.Token.IsExpire)
                {
                    Task<VsoToken> tokent = VsoApi.RefreshAccessToken(Preference.Token);
                    tokent.Wait();
                    if (tokent.Result != null)
                    {
                        Preference.Token = tokent.Result;
                    }
                }
                if (Preference?.Token == null)
                {
                    NeedLoginEvent?.Invoke(this, new EventArgs());
                }
                else if (String.IsNullOrEmpty(Preference?.SelectedAccountId))
                {
                    Task<VsoAccounts> accountst = VsoApi.GetAccounts(Preference.Token.AccessToken);
                    accountst.Wait();
                    if (accountst.Result != null)
                    {
                        Preference.Accounts = accountst.Result;
                        if (accountst.Result.Count > 1)
                        {
                            AccountSelectEvent?.Invoke(this, new EventArgs());
                        }
                    }
                }
            }
        }

        public void SetAssertionCode(string assertionCode)
        {
            VsoApi.GetAccessToken(assertionCode).ContinueWith((res) =>
            {
                if (res.IsCompleted)
                {
                    if (res.Result != null)
                    {
                        VsoController.Controller.Preference.Token = res.Result;
                    }
                }
            });
        }
    }
}
