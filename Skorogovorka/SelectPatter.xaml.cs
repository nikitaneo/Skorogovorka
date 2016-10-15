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
        public List<Patter> patters = new List<Patter>();

        public SelectPatter()
        {
            this.InitializeComponent();
            patters.Add(new Patter { id = 1, patter = "Peter Piper picked a peck of pickled peppers.\nA peck of pickled peppers Peter Piper picked.\nIf Peter Piper picked a peck of pickled peppers,\nWhere's the peck of pickled peppers Peter Piper picked?", confidence = 0.0 });
            patters.Add(new Patter { id = 2, patter = "Brothers Bean bought for their baby brother Bob’s birthday\nA big box of black bees, a blue box of brown beetles,\nAnd a big blue box of beautiful butterflies.\nBut which blue box is a bit bigger?", confidence = 0.0 });
            patters.Add(new Patter { id = 3, patter = "A funny puppy runs in front of a pub,\nA fluffy puppy runs in front of a club.\nIf the funny puppy didn’t run in front of the pub,\nWould the fluffy puppy run in front of the club?", confidence = 0.0 });
            patters.Add(new Patter { id = 4, patter = "Rock concerts shock pop icons,\nPop concerts shock rock icons.\nIf rock concerts didn’t shock pop icons,\nWould pop concerts shock rock icons?", confidence = 0.0 });
            patters.Add(new Patter { id = 5, patter = "Dolly wants to watch novels on TV\nPolly wants to watch horrors on TV.\nIf Dolly didn’t want to watch novels on TV,\nWould Polly want to watch horrors on TV?", confidence = 0.0 });
            patters.Add(new Patter { id = 5, patter = "Denise sees the fleece,\nDenise sees the fleas.\nAt least Denise could sneeze\nand feed and freeze the fleas.", confidence = 0.0 });

            foreach (Patter p in patters)
            {
                pattersList.Items.Add(p);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {

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

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
