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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Skorogovorka
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public class Patter
        {
            public int id { get; set; }
            public string patter { get; set; }
            public double confidence { get; set; }

            public override string ToString()
            {
                return patter;
            }
        }

        public List<Patter> patters = new List<Patter>();

        public MainPage()
        {
            this.InitializeComponent();
            patters.Add(new Patter { id = 1, patter = "Peter Piper picked a peck of pickled peppers.\nA peck of pickled peppers Peter Piper picked.\nIf Peter Piper picked a peck of pickled peppers,\nWhere's the peck of pickled peppers Peter Piper picked?\n", confidence = 0.0 });
            patters.Add(new Patter { id = 2, patter = "Brothers Bean bought for their baby brother Bob’s birthday\nA big box of black bees, a blue box of brown beetles,\nAnd a big blue box of beautiful butterflies.\nBut which blue box is a bit bigger?\n", confidence = 0.0 });
            patters.Add(new Patter {id = 3, patter = "A funny puppy runs in front of a pub,\nA fluffy puppy runs in front of a club.\nIf the funny puppy didn’t run in front of the pub,\nWould the fluffy puppy run in front of the club?\n", confidence = 0.0});
            patters.Add(new Patter {id = 4, patter = "Rock concerts shock pop icons,\nPop concerts shock rock icons.\nIf rock concerts didn’t shock pop icons,\nWould pop concerts shock rock icons?\n", confidence = 0.0});
            patters.Add(new Patter {id = 5, patter = "Dolly wants to watch novels on TV\nPolly wants to watch horrors on TV.\nIf Dolly didn’t want to watch novels on TV,\nWould Polly want to watch horrors on TV?\n", confidence = 0.0});
            patters.Add(new Patter { id = 5, patter = "Denise sees the fleece,\nDenise sees the fleas.\nAt least Denise could sneeze\nand feed and freeze the fleas.\n", confidence = 0.0});
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SelectPatter), patters);
        }
    }
}
