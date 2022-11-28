using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using PROJEKAT.Classes;
using PROJEKAT.Entities;

namespace PROJEKAT
{
    
    public partial class MainWindow : Window
    {
        #region Varijable Za Elipsu
        private SolidColorBrush bojaMenija;
        private static Point pozicija;
        public static Ellipse elipsaObj;
        public static Ellipse izmenjenaElipsa;
        #endregion

        #region Varijable za Poligon
        public static List<Point> tackePoligona;
        public static Polygon poligonObj;
        public static Polygon izmenjeniPoligon;
        #endregion

        #region undo,redo,clear

        public static List<UIElement> ObjektiNadKojimaJeUradjenUndo;
        public static List<UIElement> listaSvihNacrtanihObjekataNaKanvasu;
        public bool uradjenClear;

        #endregion

        #region MODEL
        public int brPxUGridicuPoVrsti;
        public int brPxUGridicuPoKoloni;
        public double brGridicaPoVisini;
        public double brGridicaPoSirini;
        Dictionary<double, PowerEntity> Entiteti = new Dictionary<double, PowerEntity>();
        Dictionary<long, LineEntity> EntitetiLinija = new Dictionary<long, LineEntity>();
        double minimumX, maximumX;
        double minimumY, maximumY;
        List<Row> redovi;
        Dictionary<long, LineEntity> ucitanaLinijaXML = new Dictionary<long, LineEntity>();
        List<LineEntity> nacrtaneLinije = new List<LineEntity>();                          //linije nacrtane na canvasu do sada
        Dictionary<long, List<Line>> sveLinijeXML = new Dictionary<long, List<Line>>();
        PowerEntity startNode, endNode;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            bojaMenija = new SolidColorBrush();
            bojaMenija.Color = Colors.Yellow;
            tackePoligona = new List<Point>();

            //undo,redo,clear
            ObjektiNadKojimaJeUradjenUndo = new List<UIElement>();
            listaSvihNacrtanihObjekataNaKanvasu = new List<UIElement>();
            uradjenClear = false;

            brPxUGridicuPoVrsti = 100;
            brPxUGridicuPoKoloni = 100;
          
        }

        private void Ellipse_Click(object sender, RoutedEventArgs e)
        {
            btnEllipse.Background = bojaMenija;
            btnPolygon.Background = null;
            btnUndo.Background = null;
            btnClear.Background = null;
            btnRedo.Background = null;
            if (btnEllipse  != sender)
            {
                btnEllipse.Background = null;
            }

        }
        private void Polygon_Click(object sender, RoutedEventArgs e)
        {
           
            btnPolygon.Background = bojaMenija;
            btnEllipse.Background = null;
            btnUndo.Background = null;
            btnClear.Background = null;
            btnRedo.Background = null;
            if (btnPolygon != sender)
            {
                btnPolygon.Background = null;
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //prozor za elipsu
            if (btnEllipse.Background == bojaMenija)
            {
                pozicija = Mouse.GetPosition(canvasDisplay);
                Window1 w1 = new Window1(btnEllipse,pozicija);
                w1.ShowDialog();

                //kada kliknemo na dugme "nacrtaj" dobijemo objekat koji treba dodati na canvas
                if (elipsaObj != null)      
                {                   
                    elipsaObj.MouseLeftButtonDown+=left_click_on_existed_ellipse;
                    canvasDisplay.Children.Add(elipsaObj);
                    
                    //undo,redo,clear
                    listaSvihNacrtanihObjekataNaKanvasu.Add(elipsaObj);
                    ObjektiNadKojimaJeUradjenUndo.Clear();
                }
                
                elipsaObj = null;
                btnEllipse.Background = null;
                
            }
            //prozor za poligon ce se otvoriti tek na levi klik kanvasa ,ovde samo "prikupljamo" tacke
            else if(btnPolygon.Background == bojaMenija)
            {
                pozicija = Mouse.GetPosition(canvasDisplay);
                tackePoligona.Add(pozicija);
            }

            
        }

        public  void left_click_on_existed_ellipse(object sender,MouseButtonEventArgs e)
        {               
             Window1 w1 = new Window1((Ellipse)sender);
             w1.IzmeniElipsu();
             w1.ShowDialog();
            //pronalazim element,pristupam njegovoj vrednosti i menjam ga
            for (int i = 0; i < canvasDisplay.Children.Count; i++)
            {
                if ((Ellipse)sender == canvasDisplay.Children[i])
                {
                    canvasDisplay.Children[i] = izmenjenaElipsa;
                }
            }
        }
   

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tackePoligona.Count > 0)
            {
                Window2 w2 = new Window2(btnPolygon, tackePoligona);
                w2.ShowDialog();
                if (poligonObj != null)
                {
                    poligonObj.MouseLeftButtonDown += left_click_on_existed_polygon;
                    canvasDisplay.Children.Add(poligonObj);

                    //undo,redo,clear
                    listaSvihNacrtanihObjekataNaKanvasu.Add(poligonObj);
                    ObjektiNadKojimaJeUradjenUndo.Clear();
                }
               
                tackePoligona.Clear();
                poligonObj = null;
                btnPolygon.Background = null;
               
            
            }
        }

        public void left_click_on_existed_polygon(object sender, MouseButtonEventArgs e)
        {
            Window2 w2 = new Window2((Polygon)sender);
            w2.IzmeniPoligon();
            w2.ShowDialog();
            //pronalazim element,pristupam njegovoj vrednosti i menjam ga
            for (int i = 0; i < canvasDisplay.Children.Count; i++)
            {
                if ((Polygon)sender == canvasDisplay.Children[i])
                {
                    canvasDisplay.Children[i] = izmenjeniPoligon;
                }
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            btnUndo.Background = bojaMenija;
            btnEllipse.Background = null;
            btnPolygon.Background = null;
            btnClear.Background = null;
            btnRedo.Background = null;
            if (btnUndo != sender)
            {
                btnUndo.Background = null;
            }

            if(canvasDisplay.Children.Count == 0) //uradjen clear
            {
                foreach(UIElement element in ObjektiNadKojimaJeUradjenUndo)
                {
                    canvasDisplay.Children.Add(element);        
                }
                ObjektiNadKojimaJeUradjenUndo.Clear();
                uradjenClear = false;
            }

           else if(canvasDisplay.Children.Count > 0) 
           {
              ObjektiNadKojimaJeUradjenUndo.Add(canvasDisplay.Children[canvasDisplay.Children.Count - 1]);
              canvasDisplay.Children.RemoveAt(canvasDisplay.Children.Count - 1);
           }
        
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            btnClear.Background = bojaMenija;
            btnEllipse.Background = null;
            btnPolygon.Background = null;
            btnUndo.Background = null;
            btnRedo.Background = null;
            if (btnUndo != sender)
            {
                btnUndo.Background = null;
            }
  
            foreach (UIElement element in listaSvihNacrtanihObjekataNaKanvasu)
            {    
                //dodajem nacrtan oblik u listu za undo da bih mogla kasnije iz nje da vratim
                ObjektiNadKojimaJeUradjenUndo.Add(element);   
                canvasDisplay.Children.Remove(element);              
            }
            uradjenClear = true;
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            btnRedo.Background = bojaMenija;
            btnEllipse.Background = null;
            btnPolygon.Background = null;
            btnUndo.Background = null;
            btnClear.Background = null;

            if (ObjektiNadKojimaJeUradjenUndo.Count > 0 && uradjenClear == false)
            {
                canvasDisplay.Children.Add(ObjektiNadKojimaJeUradjenUndo[ObjektiNadKojimaJeUradjenUndo.Count - 1]);
                ObjektiNadKojimaJeUradjenUndo.RemoveAt(ObjektiNadKojimaJeUradjenUndo.Count - 1);
            }
        }


        private void Model_Click(object sender, RoutedEventArgs e)
        {
            canvasDisplay.Children.Clear();

            InitGrid();
            UcitajEntiteteIzXML();
            NadjiIvice();
            NapraviGridice();
            UcitajEntiteteUGridice();
            UcitajLinijeUGridice();

        }

        // definise se zadat broj vrsta i kolona;
        private void InitGrid()
        {
            // broj kockica po x i y osi (2000/100) = 20 vrsta , (1000/100) = 10 kolona
            brGridicaPoVisini = ((MainWindow)App.Current.MainWindow).Height / brPxUGridicuPoVrsti;
            brGridicaPoSirini = ((MainWindow)App.Current.MainWindow).Width / brPxUGridicuPoKoloni;

            //inicijalizacija listi vrsta i kolona (lista vrsta,lista kolona)
            RowDefinition rowDefinition;
            ColumnDefinition columnDefinition;

            //cepkanje na gridice(prozor)
            for (int i = 0; i < brPxUGridicuPoVrsti; i++)
            {
                rowDefinition = new RowDefinition  //pravi element liste vrsti
                {
                    Height = new GridLength(brGridicaPoVisini)
                };

                GridPanel.RowDefinitions.Add(rowDefinition);
            }

            for (int i = 0; i < brPxUGridicuPoKoloni; i++)
            {
                columnDefinition = new ColumnDefinition  //pravi element liste kolona
                {
                    Width = new GridLength(brGridicaPoSirini)
                };
                GridPanel.ColumnDefinitions.Add(columnDefinition);
            }
           
            Grid.SetRowSpan(canvasDisplay, this.brPxUGridicuPoVrsti);
            Grid.SetColumnSpan(canvasDisplay, this.brPxUGridicuPoKoloni);
           
        }

        #region UCITAVANJE IZ XML
        private void UcitajEntiteteIzXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            XmlNodeList nodeList;

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            // parsiranje substitutuion entiteta
            List<SubstationEntity> substations = new List<SubstationEntity>();
            SubstationEntity substationEntity;
            double x, y;

            foreach (XmlNode node in nodeList)
            {
                substationEntity = new SubstationEntity
                {
                    Id = long.Parse(node.SelectSingleNode("Id").InnerText),
                    Name = node.SelectSingleNode("Name").InnerText
                };
                x = double.Parse(node.SelectSingleNode("X").InnerText) + 40;
                y = double.Parse(node.SelectSingleNode("Y").InnerText) + 80;
                ToLatLon(x, y, 34, out double newX, out double newY);
                substationEntity.X = newX;
                substationEntity.Y = newY;

                if (!Entiteti.ContainsKey(substationEntity.Id))
                    Entiteti.Add(substationEntity.Id, substationEntity);
            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            //  ParseNodeEntities(nodeList);
            List<NodeEntity> nodes = new List<NodeEntity>();
            NodeEntity nodeEntity;

            foreach (XmlNode node in nodeList)
            {
                nodeEntity = new NodeEntity
                {
                    Id = long.Parse(node.SelectSingleNode("Id").InnerText),
                    Name = node.SelectSingleNode("Name").InnerText
                };
                x = double.Parse(node.SelectSingleNode("X").InnerText) + 40;
                y = double.Parse(node.SelectSingleNode("Y").InnerText) + 80;
                ToLatLon(x, y, 34, out double newX, out double newY);
                nodeEntity.X = newX;
                nodeEntity.Y = newY;

                if (!Entiteti.ContainsKey(nodeEntity.Id))
                    Entiteti.Add(nodeEntity.Id, nodeEntity);
            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            //  ParseSwitchEntities(nodeList);
            List<SwitchEntity> switches = new List<SwitchEntity>();
            SwitchEntity switchEntity;

            foreach (XmlNode node in nodeList)
            {
                switchEntity = new SwitchEntity
                {
                    Id = long.Parse(node.SelectSingleNode("Id").InnerText),
                    Name = node.SelectSingleNode("Name").InnerText,
                    Status = node.SelectSingleNode("Status").InnerText
                };
                x = double.Parse(node.SelectSingleNode("X").InnerText) + 40;
                y = double.Parse(node.SelectSingleNode("Y").InnerText) + 80;
                ToLatLon(x, y, 34, out double newX, out double newY);
                switchEntity.X = newX;
                switchEntity.Y = newY;

                if (!Entiteti.ContainsKey(switchEntity.Id))
                    Entiteti.Add(switchEntity.Id, switchEntity);
            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            //  ParseLineEntities(nodeList);
            LineEntity line;

            foreach (XmlNode node in nodeList)
            {
                line = new LineEntity
                {
                    Id = long.Parse(node.SelectSingleNode("Id").InnerText),
                    Name = node.SelectSingleNode("Name").InnerText,
                    FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText),
                    SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText)
                };

                EntitetiLinija.Add(line.Id, line);
            }
        }
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }

        #endregion

        private void NadjiIvice()
        {
            minimumX = Entiteti.Values.Min(entity => entity.X);
            maximumX = Entiteti.Values.Max(entity => entity.X);

            minimumY = Entiteti.Values.Min(entity => entity.Y);
            maximumY = Entiteti.Values.Max(entity => entity.Y);
        }

        private void NapraviGridice()
        {
            redovi = new List<Row>(this.brPxUGridicuPoVrsti);

            //KOLIKO CE MI BITI VELIKI NOVI KOORD.SISTEM
            var differenceX = maximumX - minimumX;                  //SIRINA(novog koord.sistema)
            var rowHeight = differenceX / this.brPxUGridicuPoVrsti; //SIRINA JEDNE KOCKICE GRIDA(gridica)

            var differenceY = maximumY - minimumY;                      //VISINA
            var columnWidth = differenceY / this.brPxUGridicuPoKoloni; //VISINA JEDNE KOCKCIE GRIDA


            List<Column> columns = new List<Column>(this.brPxUGridicuPoKoloni);

            //ZA SVAKI ELEMENT KOLONE SE DODAJE NJEGOV INDEX I POMERAJ U ODNOSU NA ORGINALNI KOORD.SISTEM ==> U ODNOSU NA ORIGINALNU Y OSU 
            columns.Add(new Column(0, minimumY)); //nulti element
            for (int column = 1; column < this.brPxUGridicuPoKoloni - 1; column++) //pravi nove el. i dodaje ih u listu
            {
                columns.Add(new Column(column, minimumY + column * columnWidth)); //idx,pomeraj
            }
            columns.Add(new Column(this.brPxUGridicuPoKoloni - 1, maximumY));

            //prvi el vrste
            redovi.Add(new Row(0, maximumX, new List<Column>(columns.Count))); //svoj idx,pomeraj njegov,listu kolone ispod sebe
            columns.ForEach(x => redovi[0].Columns.Add((Column)x.Clone()));

            //ISTO SAMO ZA VRSTE POMERAJ
            for (int row = 1; row < this.brPxUGridicuPoVrsti - 1; row++)
            {
                //PRAVIMO LISTU LISTI U KOJOJ SVAKI ELEMENT VRSTE U SEBI SADRZI LISTU CELE KOLONE ISPOD SEBE
                redovi.Add(new Row(row, maximumX - row * rowHeight, new List<Column>(columns.Count)));
                columns.ForEach(x => redovi[row].Columns.Add((Column)x.Clone()));
            }

            //poslednji el vrste
            redovi.Add(new Row(this.brPxUGridicuPoVrsti - 1, minimumX, new List<Column>(columns.Count)));
            columns.ForEach(x => redovi[this.brPxUGridicuPoVrsti - 1].Columns.Add((Column)x.Clone()));

        }

        //DO SADA IMAMO GRIDOVE KOJI SU LISTA LISTI ALI SU PRAZNI JER NEMAJU U SEBI ENTITETE(NODE,SWITCH...)

        #region UCITAVANJE ENTITETA
        private void UcitajEntiteteUGridice()
        {
            Rectangle rectangle;     
            
            foreach (var entitet in Entiteti.Values)
            {
                //ZA SVAKI ENTITET PRAVOMO PRAVOUGAONIK
                rectangle = InitPravougaonik(entitet);


                Tuple<int, int> coordinates = PozicionirajEntitetUGridu(entitet); //VRATI JOJ KOORDINATE GDE DA UPISE U GRIDU (BR VRSTE I BR KOLONE)
                entitet.Row = coordinates.Item1;
                entitet.Column = coordinates.Item2;
                entitet.Rectangle = rectangle;
                                                                        //po x levo
                Canvas.SetTop(rectangle, coordinates.Item1 * brGridicaPoVisini - (rectangle.Height / 2)); //TRAZI GDE CRTA RECTANGLE U GRIDU (centar/donji desni?)
                Canvas.SetLeft(rectangle, coordinates.Item2 * brGridicaPoSirini - (rectangle.Width / 2));
                                                                                                    //po y gore(gornji je negativan)
                Canvas.SetZIndex(rectangle, 2);

                ((Canvas)GridPanel.Children[0]).Children.Add(rectangle);
            }
        }
        private Rectangle InitPravougaonik(PowerEntity entitet)
        {
            Rectangle rectangle = new Rectangle
            {
                Width = 6,
                Height = 6,
                ToolTip = entitet.ToString(),
            };

            //U ODNOSU NA TO KOJI JE ENITET BOJIM GA U RAZLICITU BOJU
            if (entitet.GetType().Name.Equals("SubstationEntity"))
                rectangle.Fill = Brushes.Red;
            else if (entitet.GetType().Name.Equals("NodeEntity"))
                rectangle.Fill = Brushes.Green;
            else if (entitet.GetType().Name.Equals("SwitchEntity"))
                rectangle.Fill = Brushes.Blue;

            return rectangle;
        }

        private Tuple<int, int> PozicionirajEntitetUGridu(PowerEntity powerEntity)
        {
            bool found = false;
            //SVAKOM ENTITETU TRAZI GRIDIC U KOJI CE GA STAVI
            //UPOREDJUJE X OD ORIGINALNOG KOORD.SISTEMA SA X OD ENITETA IZ XML FAJLA
            int row = redovi.First(x => x.Value <= powerEntity.X).Id;
            int column = redovi[row].Columns.First(x => x.Value >= powerEntity.Y).Id; //ISTO TO RADI PO Y

            if (redovi[row].Columns[column].Taken) //PITAM JE L TAJ GRIDIC ZAUZET 
            {
                int kruznica = 0; //koji smo krug

                while (!found)
                {
                    kruznica++;
                    //leva kolona
                    if (column - kruznica >= 0) //PROVERA DA LI POSTOJI KONECTRICNI KRUG REDNOG BROJA INDEX-A(nulti,prvi..)
                    {
                        List<Row> rows = new List<Row>();
                        // gornji deo reda
                        if (row - kruznica + 1 >= 0) //7
                        {

                            this.redovi
                                .FindAll(x => x.Id >= row - kruznica + 1 && x.Id <= row).OrderByDescending(x => x.Id).ToList().ForEach(x => rows.Add(x));
                        }
                        // donji deo reda
                        if (row + kruznica - 1 < this.brGridicaPoVisini) //1
                        {
                            this.redovi
                                .FindAll(x => x.Id <= row + kruznica - 1 && x.Id >= row)
                                .ForEach(x => rows.Add(x));
                        }

                        foreach (var rowEl in rows) //4
                        {
                            if (!rowEl.Columns[column - kruznica].Taken)
                            {
                                column -= kruznica;
                                row = rowEl.Id;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            break;
                    }
                    // desna kolona
                    if (column + kruznica < this.brGridicaPoSirini)
                    {
                        List<Row> rows = new List<Row>();
                        // gornji deo reda
                        if (row - kruznica + 1 >= 0) //9
                        {
                            this.redovi
                                .FindAll(x => x.Id >= row - kruznica + 1 && x.Id <= row)
                                .OrderByDescending(x => x.Id).ToList()
                                .ForEach(x => rows.Add(x));
                        }
                        // donji deo reda
                        if (row + kruznica - 1 < this.brGridicaPoVisini) //3
                        {
                            this.redovi
                                .FindAll(x => x.Id <= row + kruznica - 1 && x.Id >= row)
                                .ForEach(x => rows.Add(x));
                        }

                        foreach (var rowEl in rows) //6
                        {
                            if (!rowEl.Columns[column + kruznica].Taken)
                            {
                                column += kruznica;
                                row = rowEl.Id;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            break;
                    }

                    // gornji red
                    if (row - kruznica >= 0)
                    {
                        List<Column> columns = new List<Column>();
                        // levi deo kolone
                        if (column - kruznica > 0)//8
                            redovi[row - kruznica].Columns
                                .FindAll(x => x.Id <= column && x.Id >= column - kruznica)
                                .OrderByDescending(x => x.Id).ToList()
                                .ForEach(x => columns.Add(x));
                        // desni deo kolone
                        if (column + kruznica < this.brGridicaPoSirini)
                            redovi[row - kruznica].Columns
                                .FindAll(x => x.Id >= column && x.Id <= column + kruznica)
                                .ForEach(x => columns.Add(x));

                        foreach (var col in columns) //za drugi krug elementi kooji se nalaze iznad 8
                        {
                            if (!col.Taken)
                            {
                                column = col.Id;
                                row -= kruznica;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            break;
                    }
                    // donji red
                    if (row + kruznica < this.brGridicaPoVisini)
                    {
                        List<Column> columns = new List<Column>();
                        // levi deo kolone
                        if (column - kruznica > 0)
                            redovi[row + kruznica].Columns
                                .FindAll(x => x.Id <= column && x.Id >= column - kruznica)
                                .OrderByDescending(x => x.Id).ToList()
                                .ForEach(x => columns.Add(x));
                        // desni deo kolone
                        if (column + kruznica < this.brGridicaPoSirini)
                            redovi[row + kruznica].Columns
                                .FindAll(x => x.Id >= column && x.Id <= column + kruznica)
                                .ForEach(x => columns.Add(x));

                        foreach (var col in columns)
                        {
                            if (!col.Taken)
                            {
                                column = col.Id;
                                row += kruznica;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            break;
                    }
                }
            }
            else
                redovi[row].Columns[column].Taken = true;

            return new Tuple<int, int>(row, column);
        }
        #endregion

        #region UCITAVANJE LINIJA
        private void UcitajLinijeUGridice()
        {
            Line newLine;

            foreach (var line in EntitetiLinija.Values)
            {
                //da li postoje entieti koji imaju koordinate kao pocetna i krajnja tacka linije(first,second)
                if (Entiteti.TryGetValue(line.FirstEnd, out PowerEntity startNode) &&
                    Entiteti.TryGetValue(line.SecondEnd, out PowerEntity endNode) &&
                    //da li linija koju smo uzeli iz lineenteties nacrtana (da ne bi 2x crtali)
                    nacrtaneLinije.Find(x => x.FirstEnd == line.FirstEnd && x.SecondEnd == line.SecondEnd ||
                                          x.FirstEnd == line.SecondEnd && x.SecondEnd == line.FirstEnd) == null)
                {
                    //ako nije nacrtana doda je u pomocnu listu
                    sveLinijeXML.Add(line.Id, new List<Line>());

                    //lista gridica = rowcolumn
                    List<RowColumn> rowColumn = NadjiPutanju(startNode, endNode); 
                    for(int i = 0; i < (rowColumn.Count - 1 ); i++)
                    {
                        
                        newLine = CreateLine(line, rowColumn[i], rowColumn[i+1]);
                        Canvas.SetZIndex(newLine, 0);
                        ((Canvas)GridPanel.Children[0]).Children.Add(newLine);
                        nacrtaneLinije.Add(line);
                        sveLinijeXML[line.Id].Add(newLine);
                        
                    }
                }
            }
        }
        private Line CreateLine(LineEntity lineEntity, RowColumn rowColumn1, RowColumn rowColumn2)
        {
           Line line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                X1 = rowColumn1.Column * brGridicaPoSirini,
                Y1 = rowColumn1.Row * brGridicaPoVisini,
                X2 = rowColumn2.Column * brGridicaPoSirini,
                Y2 = rowColumn2.Row * brGridicaPoVisini,
                ToolTip = lineEntity.ToString()
            };

            return line;
        }
        private List<RowColumn> NadjiPutanju(PowerEntity startNode, PowerEntity endNode)
        {
            //lista gridica
            List<RowColumn> putanja = new List<RowColumn>(); //redja cvorove koji ce da cine konacnu putanju

            int rowFirst = startNode.Row;
            int columnFirst = startNode.Column;
            int rowSecond = endNode.Row;
            int columnSecond = endNode.Column;


            RowColumn rcStart = new RowColumn(rowFirst, columnFirst);
            RowColumn rcEnd = new RowColumn(rowSecond, columnSecond);

            if (rcStart != null) 
            {
                putanja.Add(rcStart); //DODAMO GA U PUTANJU

                if (rowFirst < rowSecond)
                {
                    int tempRow = rowFirst;
                    while (tempRow != rowSecond)
                    {
                        tempRow = tempRow + 1;
                        putanja.Add(new RowColumn(tempRow, rcStart.Column));
                    }
                }
                else if (rowFirst > rowSecond)
                {
                    int tempRow = rowFirst;
                    while (tempRow != rowSecond)
                    {
                        tempRow = tempRow - 1;
                        putanja.Add(new RowColumn(tempRow, rcStart.Column));
                    }
                }
                else
                {
                    //nista 
                }

                if (columnFirst < columnSecond)
                {
                    int tempCol = columnFirst;
                    while (tempCol != columnFirst)
                    {
                        tempCol = tempCol + 1;
                        putanja.Add(new RowColumn(rowSecond, tempCol));
                    }
                }
                else if (columnFirst > columnSecond)
                {
                    int tempCol = columnFirst;
                    while (tempCol != columnFirst)
                    {
                        tempCol = tempCol - 1;
                        putanja.Add(new RowColumn(rowSecond, tempCol));
                    }
                }
                else
                {
                    //nista 
                }

                putanja.Add(rcEnd);
            }
            

            return putanja;
        }
        #endregion
    }

}
