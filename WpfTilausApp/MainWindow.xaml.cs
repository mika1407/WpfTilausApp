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

namespace WpfTilausApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HaeAsiakkaat(); //Täytetään Asiakas - comboboxin sisältö
            HaeTuotteet();
        }

        private void HaeAsiakkaat()
        {
            List<cbPairAsiakas> cbpairAsiakkaat = new List<cbPairAsiakas>();
            TilausDBEntities entities = new TilausDBEntities();

            var asiakkaat = from a in entities.Asiakkaat
                            select a;

            foreach (var asiakas in asiakkaat)
            {
                cbpairAsiakkaat.Add(new cbPairAsiakas(asiakas.Nimi, asiakas.AsiakasID));
            }
            cbAsiakas.DisplayMemberPath = "asiakasNimi";
            cbAsiakas.SelectedValuePath = "asiakasNro";
            cbAsiakas.ItemsSource = cbpairAsiakkaat;
        }
        private void HaeTuotteet()
        {
            List<cbPairTuote> cbpairTuotteet = new List<cbPairTuote>();
            TilausDBEntities entities = new TilausDBEntities();

            var tuotteet = from a in entities.Tuotteet
                           select a;

            foreach (var tuote in tuotteet)
            {
                cbpairTuotteet.Add(new cbPairTuote(tuote.Nimi, tuote.TuoteID));
            }
            cbTuote.DisplayMemberPath = "tuoteNimi";
            cbTuote.SelectedValuePath = "tuoteNro";
            cbTuote.ItemsSource = cbpairTuotteet;
        }

        private void cbAsiakas_DropDownClosed(object sender, EventArgs e)
        {
            cbPairAsiakas cbp = (cbPairAsiakas)cbAsiakas.SelectedItem;
            string AsiakasNimi = cbp.asiakasNimi;
            int AsiakasNro = cbp.asiakasNro;
            txtAsiakasNumero.Text = AsiakasNro.ToString();
        }

        private void btnTallenna_Click(object sender, RoutedEventArgs e)
        {
            TilausOtsikko Tilaus = new TilausOtsikko();
            Tilaus.AsiakasNumero = int.Parse(txtAsiakasNumero.Text);
            Tilaus.ToimitusOsoite = txtToimitusOsoite.Text; //<-- Lisää toimitusosoite käyttöliittymään
            Tilaus.Postinumero = txtPostinumero.Text;
            //Tilaus.TilausPvm = DateTime.Today; //<-- korvaa tämä alla olevalla esimerkilla, kun olet lisännyt DatePickerin
            //Tilaus.ToimitusPvm = DateTime.Now.AddDays(2); //<-- korvaa tämä alla olevalla esimerkilla, kun olet lisännyt DatePickerin
            Tilaus.TilausPvm = dpTilausPvm.SelectedDate.Value; //<-- lisää DatePicker -komponentti käyttöliittymään
            Tilaus.ToimitusPvm = dpToimitusPvm.SelectedDate.Value; //<-- lisää DatePicker -komponentti käyttöliittymään

            txtToimitusAika.Text = Tilaus.LaskeToimitusAika();

            txtTilausNumero.Text = VieTilausKantaan(Tilaus);
        }

        private string VieTilausKantaan(TilausOtsikko Tilaus) //Huomaa, että rutiini palauttaa stringin (uuden tilauksen numeron)
        {
            try
            {
                TilausDBEntities entities = new TilausDBEntities();
                Tilaukset dbItem = new Tilaukset()
                {
                    AsiakasID = Tilaus.AsiakasNumero, //Tilaus on tilausotisikko-tyyppinen olio, jonka tämä rutiini saa parametrinä kutsuvasta ohjelmasta
                    Toimitusosoite = Tilaus.ToimitusOsoite,
                    Postinumero = Tilaus.Postinumero,
                    Tilauspvm = Tilaus.TilausPvm,
                    Toimituspvm = Tilaus.ToimitusPvm
                };

                entities.Tilaukset.Add(dbItem);
                entities.SaveChanges();

                int id = dbItem.TilausID; //Haetaan juuri lisätyn rivin IDENTITEETTIsarakkeen arvo (eli PK)
                return id.ToString(); //Palautetaan onnistuneen lisäyksen merkiksi uuden tilauksen numero
            }
            catch (Exception)
            {
                return "0"; //Jos tallennus tietokantaan epäonnistuu, tämä rutiini palauttaa nollan
            }
        }
    }
}
