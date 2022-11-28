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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Point pt;
        private Button btnEllipse;
        private SolidColorBrush bojaKonturneLinije;
        private SolidColorBrush bojaElipse;
        private Ellipse el;
        double dkl = 0;  //debljina konturne linije
        double v = 0;    //visina
        double s = 0;    //sirina
       

        private Ellipse ellipsePriv;

        public Window1(Button btnEllipse ,Point pozicija)
        {
            InitializeComponent();
            btnEllipse = btnEllipse;
            pt = pozicija;
        }

        public Window1(Ellipse ElipsaEdit)
        {
            InitializeComponent();
            el = ElipsaEdit;           

        }

        public void IzmeniElipsu()
        {
            TBvisina.Text = el.Height.ToString();
            TB_sirina.Text = el.Width.ToString();
            TB_debljinaKonturneLinije.Text = el.StrokeThickness.ToString();
            TBvisina.IsReadOnly = true;
            TB_sirina.IsReadOnly = true;
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

                el.StrokeThickness = dkl;
                el.Stroke = bojaKonturneLinije;
                el.Fill = bojaElipse;
                MainWindow.izmenjenaElipsa = el;
            }
            else
            {              
                double.TryParse(TBvisina.Text, out v);
                double.TryParse(TB_sirina.Text, out s);
                double.TryParse(TB_debljinaKonturneLinije.Text, out dkl);

                // tako da im je gornji levi ugao pozicija gde je pokazivačem miša kliknuto
                //da bi se inicirala akcija crtanja
                Ellipse nacrtanaElipsa = new Ellipse { Height = v, Width = s };
                double osaX = pt.X;   //(sirina/2) ako zelimo da tacka bude u centru
                double osaY = pt.Y;
                nacrtanaElipsa.Margin = new Thickness(osaX, osaY, 0, 0);

                nacrtanaElipsa.StrokeThickness = dkl;
                nacrtanaElipsa.Stroke = bojaKonturneLinije;
                nacrtanaElipsa.Fill = bojaElipse;

                MainWindow.elipsaObj = nacrtanaElipsa;
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
            bojaElipse = new SolidColorBrush(bojaOblika);
        }

        private void Colors_Loaded(object sender, RoutedEventArgs e)
        {
            CB_bojaKonturneLinije.ItemsSource = typeof(Colors).GetProperties();
            CB_bojaOblika.ItemsSource = typeof(Colors).GetProperties();
        }
        #endregion
    }

}
