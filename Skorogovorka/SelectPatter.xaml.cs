using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Skorogovorka.MainPage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Skorogovorka
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class SelectPatter : Page
    {
        public List<Patter> patters;

        public SelectPatter()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                patters = (List<Patter>)e.Parameter;
                foreach(Patter p in patters)
                {
                    pattersList.Items.Add(p);
                }
            }
        }

        private void pattersList_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach(Patter p in patters)
            {
                if (e.ClickedItem.ToString().Equals(p.patter))
                    Frame.Navigate(typeof(Speaker), e.ClickedItem.ToString());
            }
        }
    }
}
