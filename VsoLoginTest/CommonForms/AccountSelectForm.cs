using CommonData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace CommonForms
{
    public class AccountSelectForm : ContentPage
    {
        ListView accountList;

        public AccountSelectForm()
        {
            accountList = new ListView()
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    ViewCell cell = new ViewCell();
                    Label nameLabel = new Label()
                    {
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                    };

                    cell.View = new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Children =
                        {
                            nameLabel,
                        },
                    };
                    nameLabel.SetBinding(Label.TextProperty, new Binding("Name"));

                    return cell;
                }),
            };
            Content = new StackLayout()
            {
                Children =
                {
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.Start,
                        FontSize = 28,
                        Text = "Select an Account",
                    },
                    accountList,
                },
            };

            accountList.SetBinding(ListView.ItemsSourceProperty, new Binding("Accounts"));

            BindingContext = VsoController.Controller.Preference;

            this.Appearing += AccountSelectForm_Appearing;
        }

        private void AccountSelectForm_Appearing(object sender, EventArgs e)
        {
            Debug.WriteLine("binding:" + BindingContext);
        }
    }
}
