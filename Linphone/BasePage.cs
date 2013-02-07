using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Linphone
{
    public class BasePage: PhoneApplicationPage
    {
        protected BasePage()
            : this(new BaseModel())
        {

        }

        protected BasePage(BaseModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.Page = this;
            this.Loaded += BasePage_Loaded;
        }

        protected readonly BaseModel ViewModel;

        protected virtual void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs nee)
        {
            base.OnNavigatedTo(nee);

            this.ViewModel.OnNavigatedTo(nee);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            base.OnNavigatedFrom(nee);

            this.ViewModel.OnNavigatedFrom(nee);
        } 
    }
}
