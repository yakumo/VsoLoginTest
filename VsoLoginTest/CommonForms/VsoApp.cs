using CommonData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CommonForms
{
	public class VsoApp : Xamarin.Forms.Application
	{
        public VsoApp()
        {
            VsoController.Controller.NeedLoginEvent += Controller_NeedLoginEvent;
            VsoController.Controller.AccountSelectEvent += Controller_AccountSelectEvent;

            MainPage = new NavigationPage(new ContentPage()
            {
                Content = new Label()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Text = "Hello, world",
                    FontSize = 48,
                }
            });

            MainPage.Appearing += MainPage_Appearing;
		}

        private void MainPage_Appearing(object sender, EventArgs e)
        {
            MainPage.Appearing -= MainPage_Appearing;
            VsoController.Controller.Start(DependencyService.Get<IVsoPreferenceStore>());
        }

        private void Controller_AccountSelectEvent(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MainPage.Navigation.PushModalAsync(new AccountSelectForm(), true);
            });
        }

        private void Controller_NeedLoginEvent(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MainPage.Navigation.PushModalAsync(new LoginForm(), true);
            });
        }

        protected override void OnStart()
        {
            base.OnStart();

            Resume().ContinueWith((res) => { });
        }

        protected override void OnResume()
        {
            base.OnResume();

            Resume().ContinueWith((res) => { });
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            Sleep().ContinueWith((res) => { });
        }

        async Task Resume()
        {
            await VsoController.Controller.PreferenceStore.Load();
        }

        async Task Sleep()
        {
            await VsoController.Controller.PreferenceStore.Save();
        }
    }
}
