using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DFASimulator3000
{
    public static class OmatKomennot
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
           (
                   "Exit",
                   "Exit",
                   typeof(OmatKomennot)
           );

        public static readonly RoutedUICommand NewGame = new RoutedUICommand
           (
                   "NewGame",
                   "NewGame",
                   typeof(OmatKomennot)
           );
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<DockPanel> dockPaneelit = new List<DockPanel>();
        List<CheckBox> hyvaksytytTilat = new List<CheckBox>();
        List<List<TextBox>> tekstboxienListat = new List<List<TextBox>>();
        List<TextBlock> tilaNumerot = new List<TextBlock>();
        List<char> inputs = new List<char>();
        private int laskuri = 1;
        private int alphaCalc = 2;
        public int AlphaCalc
        {
            get
            {
                return alphaCalc;
            }
            set
            {
                alphaCalc = value;
            }
        }

        private int step = 0;
        public int Step
        {
            get { return step; }
            
            set
            {
                step = value;
            }
        }

        private char[] aakkoset = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z', 'å', 'ä', 'ö' };
        private int tila = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Tarkistetaan koko syöte kerralla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TarkistaKokonaan(object sender, RoutedEventArgs e)
        {
            tila = 0;   
            Step = 0;   
            for (int i = 0; i < kayttajanTextBox.Text.Length; i++) // ei-tyhjä-merkkijono
            {
                if (tila > dockPaneelit.Count || tila < 0) break; // jos edellisen kierroksen tilaa ei ole olemassa, lopetaan
                tila = Tila(kayttajanTextBox.Text[i] + ""); // --> Lähdetaan katsotaan tila
            }
            AsetaHyvaksyttyLabel(); // asetetaan tieto käyttäjälle
        }

        /// <summary>
        /// Tarkistetaan syöte merkki kerrallaan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StepNappi_Click(object sender, RoutedEventArgs e)
        {
            if (kayttajanTextBox.Text.Contains(">>")) kayttajanTextBox.Text = kayttajanTextBox.Text.Remove(kayttajanTextBox.Text.IndexOf(">>"), 2); // poistetaan nuolet käyttäjänsyötteestä
            if (Step > 0 && Step <= kayttajanTextBox.Text.Length) // stepkerta ei saa olla eka (0) ja sen pitää olla enemmän kuin syötemerkkien pituus
            {
                if (tila < dockPaneelit.Count && tila >= 0)   // tilan piaa olla olemassa (sille pitää olla luotu dockpaneeli) ja tila ei saa olla jo hylätty eli (-1)
                {
                    tila = Tila(kayttajanTextBox.Text[Step - 1] + "");
                }
            }
                if (kayttajanTextBox.Text.Length > Step) // jos syöterkkejä on vielä tarkastettavana
                {
                    kayttajanTextBox.Text = kayttajanTextBox.Text.Insert(Step, ">>");
                    Step++;
                }
                else
                {
                    Step = 0;
                    tila = 0;
                } 
            AsetaHyvaksyttyLabel();
        }

        /// <summary>
        /// Katsotaan tila
        /// </summary>
        /// <param name="merkkijono"></param>
        /// <returns></returns>
        public int Tila(string merkkijono)
        {
            int[] intTilat = new int[tekstboxienListat.Count];
            for (int i = 0; i < tekstboxienListat.Count; i++)
            {
               // if (tila > dockPaneelit.Count) break; // jos käyttäjä on laittanut tilan, jota ei ole olemassa, lopetetaan
                if (!(int.TryParse(tekstboxienListat[i][tila].Text, out intTilat[i]))) intTilat[i] = -1;  // jos muutos ei onnistu, niin laitetaan -1. Oletuksena on jostain syystä 0, joka ei sovi
            }

            switch (merkkijono) 
            {
                case "a" : return intTilat[0];
             
            
            }

            for (int i = 0; i < aakkoset.Length; i++)
            {
                try { if (merkkijono.Equals(aakkoset[i].ToString())) return intTilat[i]; }
                catch (IndexOutOfRangeException e)
                {
                    break;
                }
            }
            return -1;
        }

        /// <summary>
        /// asetetaan tieto käyttäjälle näkyvään labeliin
        /// </summary>
        private void AsetaHyvaksyttyLabel()
        {
            if (tila < 0 || tila > dockPaneelit.Count - 1) HyvaksyvaInfoToKayttaja.Text = "Hylätty!";  // tila saattaa olla -1, jolloin se pitää katsoa ennen kuin katsotaan hyvaksytytTilat[tila], koska sitä ei silloin voi olla
            else if (hyvaksytytTilat[tila].IsChecked == true) HyvaksyvaInfoToKayttaja.Text = "Hyväksyvä!"; // jos lopputilan rivin checboks on valittu --> tila on hyväksytty
            else HyvaksyvaInfoToKayttaja.Text = "Hylätty!"; // tila > 0, joten se on olemassa, mutta checkboksia ei ole valittu, joten se on hylätty tila
        }

       /* ======================================================================================
        * NS KÄYTTÖLIITTYMÄJUTUT 
        * ==========================================================================================
        */

        /// <summary>
        /// Tyhjennetään kaikki syötekentät
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TyhjennaKentat(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dockPaneelit.Count; i++)
            {
                hyvaksytytTilat[i].IsChecked = false;
                for (int j = 1; j <= AlphaCalc; j++)
                {
                    DockPanel a = dockPaneelit[i];
                    TextBox ab = (TextBox)dockPaneelit[i].Children[j];
                    ab.Text = "";

                }
            }
            kayttajanTextBox.Text = "";
        }


        /// <summary>
        /// Poistaa viimeisimmän tilan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PoistaTila(object sender, RoutedEventArgs e)
        {
            if (laskuri > 1)
            {
                SisaltoStackPanel.Children.Remove(dockPaneelit.Last());

                dockPaneelit.RemoveAt(dockPaneelit.Count - 1);
                tilaNumerot.RemoveAt(tilaNumerot.Count - 1);
                hyvaksytytTilat.RemoveAt(hyvaksytytTilat.Count - 1);
                laskuri--;
            }
        }

        /// <summary>
        /// Lisää yhden tilan lisää
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LisaaTila(object sender, RoutedEventArgs e)
        {
            DockPanel a = new DockPanel();
            a.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            a.Margin = dockPaneelit[0].Margin;
            dockPaneelit.Add(a);
            SisaltoStackPanel.Children.Add(a);

            TextBlock tb = new TextBlock();
            a.Children.Add(tb);
            tb.FontSize = 20;
            string teksti = laskuri.ToString("00");
            tb.Text = String.Format("{0:00}", teksti);
            tilaNumerot.Add(tb);
            laskuri++;

            for (int i = 0; i < alphaCalc; i++)
            {
                List<TextBox> lista = tekstboxienListat[i];
                TextBox vasenBox = new TextBox();
                a.Children.Add(vasenBox);
                tekstboxienListat[i].Add(vasenBox);
                vasenBox.Width = 50;
            }
            CheckBox hyvaksyvaCheckBox = new CheckBox();
            a.Children.Add(hyvaksyvaCheckBox);
            hyvaksytytTilat.Add(hyvaksyvaCheckBox);
            hyvaksyvaCheckBox.Content = "Hyväksyvä tila";

        }

        /// <summary>
        /// Lisää yhden syöttömerkin lisää
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LisaaSyoteMerkki(object sender, RoutedEventArgs e)
        {
            if (AlphaCalc < aakkoset.Length)
            {
                List<TextBox> a = new List<TextBox>();
                tekstboxienListat.Add(a);

                for (int i = 0; i < dockPaneelit.Count; i++)
                {
                    DockPanel b = dockPaneelit[i];

                    TextBox oikeammaisinBox = new TextBox();
                    oikeammaisinBox.Width = 50;
                    a.Add(oikeammaisinBox);
                    b.Children.Add(oikeammaisinBox);


                    CheckBox cb = hyvaksytytTilat[i]; // otetaan talteen
                    dockPaneelit[i].Children.Remove(cb); // poistetaan keskeltä
                    dockPaneelit[i].Children.Add(cb); // lisätään loppuun 
                }

                TextBox justForShow = new TextBox();
                justForShow.Width = 50;
                justForShow.Text = aakkoset[alphaCalc].ToString();
                justforShowInputs.Children.Add(justForShow);
                justForShow.BorderBrush = Brushes.White;
                justForShow.TextAlignment = TextAlignment.Center;

                CheckBox justForSCb = justForShowCheckBox;
                justforShowInputs.Children.Remove(justForSCb);
                justforShowInputs.Children.Add(justForSCb);
                alphaCalc++;
            }
        }

        /*
         * MENUJUTUT
         * */
        /// <summary>
        /// Suljetaan ohjelma
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        void ExitExecuted(object target, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Avataan uusi ohjelma
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        public void NewGameCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            MainWindow uusIkkuna = new MainWindow();
            uusIkkuna.Show();
            this.Close();

        }

        /// <summary>
        /// Kun uusi checkbox syntyy, se lisätään listaan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void t0aHyvaksyva_Initialized(object sender, EventArgs e)
        {
            hyvaksytytTilat.Add((CheckBox)sender);
        }

        private void t0a_Initialized(object sender, EventArgs e)
        {
            List<TextBox> newTb = new List<TextBox>();
            TextBox a = (TextBox)sender;
            newTb.Add(a);
            tekstboxienListat.Add(newTb);

        }

        private void t0b_Initialized(object sender, EventArgs e)
        {
            List<TextBox> newTb = new List<TextBox>();
            TextBox a = (TextBox)sender;
            newTb.Add(a);
            tekstboxienListat.Add(newTb);
        }

        /// <summary>
        /// Kun uusi tila luodaan, lisätään se dockpaneelien listaan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockPanel_Initialized(object sender, EventArgs e)
        {
            dockPaneelit.Add((DockPanel)sender);
        }


        /// <summary>
        /// Poistetaan syötemerkki, jos niitä on > 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PoistaSyoteMerkki(object sender, RoutedEventArgs e)
        {

            if (alphaCalc > 2)
            {
                for (int i = 0; i < dockPaneelit.Count; i++)
                {
                    DockPanel b = dockPaneelit[i];
                    CheckBox ab = (CheckBox)b.Children[b.Children.Count - 1]; // talteen
                    b.Children.RemoveAt(b.Children.Count - 1); // poistaa checkboksin
                    b.Children.RemoveAt(b.Children.Count - 1); // poistaa vikan merkin

                    b.Children.Add(ab);
                }

                tekstboxienListat.RemoveAt(tekstboxienListat.Count - 1);
                justforShowInputs.Children.RemoveAt(justforShowInputs.Children.Count - 2);
                alphaCalc--;
            }
        }
    }
}