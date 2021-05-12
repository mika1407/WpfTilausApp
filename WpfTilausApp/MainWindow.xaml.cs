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
        DateTime tänään = DateTime.Today;
        Decimal RivienSummaYht = 0;
        public MainWindow()
        {
            InitializeComponent();
            HaeAsiakkaat(); //Täytetään Asiakas - comboboxin sisältö
            HaeTuotteet();

            dpTilausPvm.SelectedDate = tänään;  //Datepickerin oletuspvm
            dpToimitusPvm.SelectedDate = tänään.AddDays(14);  //Datepickerin oletuspvm
            //Luodaan ikäänkuin ilmaan DataGridTextColumn -tyyppisiä olioita
            DataGridTextColumn colTilausRiviNumero = new DataGridTextColumn();
            DataGridTextColumn colTilausNumero = new DataGridTextColumn();
            DataGridTextColumn colTuoteNumero = new DataGridTextColumn();
            DataGridTextColumn colTuoteNimi = new DataGridTextColumn();
            DataGridTextColumn colMaara = new DataGridTextColumn();
            DataGridTextColumn colAHinta = new DataGridTextColumn();
            DataGridTextColumn colRivinSumma = new DataGridTextColumn();
            //Oliot sidotaan tietyn nimiseen sarakkeeseen < --kohdistuu myöhemmin olion ominaisuuksiin, kunhan olio on ensin viety listalle
            colTilausRiviNumero.Binding = new Binding("TilausRiviNumero");
            colTilausNumero.Binding = new Binding("TilausNumero");
            colTuoteNumero.Binding = new Binding("TuoteNumero");
            colTuoteNimi.Binding = new Binding("TuoteNimi");
            colMaara.Binding = new Binding("Maara");
            colAHinta.Binding = new Binding("AHinta");
            colRivinSumma.Binding = new Binding("Summa");
            //DataGridin otsikot 
            colTilausRiviNumero.Header = "Tilausrivin numero";
            colTilausNumero.Header = "Tilauksen numero";
            colTuoteNumero.Header = "Tuotenumero";
            colTuoteNimi.Header = "Tuotenimi";
            colMaara.Header = "Määrä";
            colAHinta.Header = "A-Hinta";
            colRivinSumma.Header = "Rivin summa";
            //Edellä "ilmaan" luotujen sarakkeiden sijoitus konkreettiseen DataGridiin, joka on luotu formille
            dgTilausRivit.Columns.Add(colTilausRiviNumero);
            dgTilausRivit.Columns.Add(colTilausNumero);
            dgTilausRivit.Columns.Add(colTuoteNumero);
            dgTilausRivit.Columns.Add(colTuoteNimi);
            dgTilausRivit.Columns.Add(colMaara);
            dgTilausRivit.Columns.Add(colAHinta);
            dgTilausRivit.Columns.Add(colRivinSumma);
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

        private void cbTuote_DropDownClosed(object sender, EventArgs e)
        {
            cbPairTuote cbp = (cbPairTuote)cbTuote.SelectedItem;
            string TuoteNimi = cbp.tuoteNimi;
            int TuoteNro = cbp.tuoteNro;
            txtTuoteNumero.Text = TuoteNro.ToString();
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

        private void btnLisaaRivi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TilausRivi tilausRivi = new TilausRivi();
                tilausRivi.TilausNumero = int.Parse(txtTilausNumero.Text);
                tilausRivi.TuoteNumero = int.Parse(txtTuoteNumero.Text);
                tilausRivi.TuoteNimi = cbTuote.Text;
                tilausRivi.Maara = int.Parse(txtMaara.Text);
                tilausRivi.AHinta = Convert.ToDecimal(txtAHinta.Text);

                tilausRivi.TilausRiviNumero = VieTilausRiviKantaan(tilausRivi);


                RivienSummaYht += tilausRivi.RiviSumma(); //Kuten tämä: RivinSummaYht = RivinSummaYht + TilausR.RiviSumma();
                txtRiviSumma.Text = RivienSummaYht.ToString();
                dgTilausRivit.Items.Add(tilausRivi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtTuoteNumero.Clear();
            txtMaara.Clear();
            txtAHinta.Clear();
        }
        private int VieTilausRiviKantaan(TilausRivi TilausR)
        {
            TilausDBEntities entities = new TilausDBEntities();

            Tilausrivit dbItem = new Tilausrivit()
            {
                TilausID = TilausR.TilausNumero,
                TuoteID = TilausR.TuoteNumero,
                Maara = TilausR.Maara,
                Ahinta = TilausR.AHinta
            };

            entities.Tilausrivit.Add(dbItem);
            entities.SaveChanges();

            int id = dbItem.TilausriviID; //Haetaan juuri lisätyn rivin IDENTITEETTIsarakkeen arvo (eli PK)
            return id; //Pa
        }

        private void BtnPostitoimiPaikat_Click(object sender, RoutedEventArgs e)
        {
            formPostitoimiPaikat frmPostmp = new formPostitoimiPaikat();
            frmPostmp.Show();
        }

        private void BtnTuotteet_Click(object sender, RoutedEventArgs e)
        {
            formTuotteet frmTuotteet = new formTuotteet();
            frmTuotteet.Show();
        }
    }
}
