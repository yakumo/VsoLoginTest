using CommonData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;

namespace CommonForms
{
    public class LoginForm : ContentPage
    {
        WebView webView;
        ActivityIndicator indicator;

        public LoginForm()
        {
            Grid grid;
            BackgroundColor = Color.Black;
            indicator = new ActivityIndicator()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsRunning = true,
                IsVisible = false,
            };
            webView = new WebView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            webView.Navigating += WebView_Navigating;
            webView.Navigating += WebView_Navigating1;
            Content = grid = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = new GridLength(1,GridUnitType.Star), }
                },
                RowDefinitions =
                {
                    new RowDefinition() { Height = new GridLength(1,GridUnitType.Star), }
                },
            };
            grid.Children.Add(webView, 0, 0);
            grid.Children.Add(indicator, 0, 0);
            this.Appearing += (object sender, EventArgs e) =>
            {
                VsoController.Controller.Preference.PropertyChanged += Preference_PropertyChanged;

                string url = String.Format("https://app.vssps.visualstudio.com/oauth2/authorize?client_id={0}&response_type=Assertion&state=AppAuth&scope={2}&redirect_uri={1}", Constants.ClientId, Constants.CallbackUrl, WebUtility.UrlEncode(Constants.AuthorizeScope));
                Debug.WriteLine(url);
                webView.Source = url;
            };
            this.Disappearing += (object sender, EventArgs e) =>
            {
                VsoController.Controller.Preference.PropertyChanged -= Preference_PropertyChanged;
            };
        }

        private void Preference_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Token")
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    //indicator.IsVisible = false;
                    await this.Navigation.PopModalAsync(true);
                });
            }
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            //indicator.IsVisible = true;
            Debug.WriteLine("navigating, " + e.Url.ToString());
            if (e.Url.ToString().StartsWith(Constants.CallbackUrl))
            {
                e.Cancel = true;
                // Token受信タスク
                Task.Factory.StartNew(() =>
                {
                    // Code取得
                    string assertionCode = String.Empty;
                    string[] kep = e.Url.ToString().Split(new char[] { '?', '&' });
                    if (kep.Length > 1)
                    {
                        foreach (string k in kep)
                        {
                            string[] kv = k.Split(new char[] { '=' });
                            if (kv[0] == "code")
                            {
                                assertionCode = kv[1];
                                break;
                            }
                        }
                    }
                    VsoController.Controller.SetAssertionCode(assertionCode);
                });
            }
        }

        private void WebView_Navigating1(object sender, WebNavigatingEventArgs e)
        {
            //indicator.IsVisible = false;
            Debug.WriteLine("navigated, " + e.Url.ToString());
        }
    }
}
