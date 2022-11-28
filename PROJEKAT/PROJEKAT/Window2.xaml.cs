using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PROJEKAT
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private List<Point> tacke;
        private Button btnPolygon;
        double dkl = 0;  //debljina konturne linije
        private SolidColorBrush bojaKonturneLinije;
        private SolidColorBrush bojaPoligona;
        private Polygon po;
     

        public Window2(Button btnPolygon, List<Point> tackePoligona)
        {
            InitializeComponent();
            btnPolygon = btnPolygon;
            tacke = tackePoligona;
        }
        public Window2(Polygon PolygonEdit)
        {
            InitializeComponent();
            po = PolygonEdit;

        }

        public void IzmeniPoligon()
        {            
            TB_debljinaKonturneLinije.Text = po.StrokeThickness.ToString();            
            btnNacrtaj.Content = "Izmeni";
        }

        private void Otkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Nacrtaj_Click(object sender, RoutedEventArgs e)
        {
            if (btnNacrtaj.Content == "Izmeni")
            {
                double.TryParse(TB_debljinaKonturneLinije.Text, out dkl);

                po.StrokeThickness = dkl;
                po.Stroke = bojaKonturneLinije;
                po.Fill = bojaPoligona;

                MainWindow.izmenjeniPoligon = po;

            }
            else
            {
                double.TryParse(TB_debljinaKonturneLinije.Text, out dkl);

                Polygon p = new Polygon();
                p.Points = new PointCollection(tacke);
                p.StrokeThickness = dkl;
                p.Stroke = bojaKonturneLinije;
                p.Fill = bojaPoligona;

                MainWindow.poligonObj = p;

            }           
            this.Close();
        }

        #region BOJENJE
        private void CBBojaKonturneLinije(object sender, SelectionChangedEventArgs e)
        {

            Color bcl = (Color)(CB_bojaKonturneLinije.SelectedItem as PropertyInfo).GetValue(1, null);
            bojaKonturneLinije = new SolidColorBrush(bcl);

        }

        private void CBBojaOblika(object sender, SelectionChangedEventArgs e)
        {

            Color bojaOblika = (Color)(CB_bojaOblika.SelectedItem as PropertyInfo).GetValue(1, null);
            bojaPoligona = new SolidColorBrush(bojaOblika);
        }

        private void Colors_Loaded(object sender, RoutedEventArgs e)
        {
            CB_bojaKonturneLinije.ItemsSource = typeof(Colors).GetProperties();
            CB_bojaOblika.ItemsSource = typeof(Colors).GetProperties();
        }
        #endregion

    }
}
