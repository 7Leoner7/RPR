using RPR.Model;
using RPR.Shapes;
using RPR.ViewModel;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RPR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected GameView game { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += View_GoToTheBegin;
        }

        private void MainView_Initialized(object sender, EventArgs e)
        {

        }

        async private void MainView_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void MainView_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            game = new GameView(ref MainView, "World0");

            var rand = new Random();
            var Width = 100;
            var Height = 100;
            var vec = new Vector(0.4, 0.5);
            var speed = 5 * 7;
            vec.X *= speed;
            vec.Y *= speed;

            var ellipse = new SmartShape(new Ellipse()
            {
                Width = Width,
                Height = Height,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))),
                Margin = new Thickness()
                {
                    Bottom = 0,
                    Left = rand.Next(Width, (int)game.View.ActualWidth - Width),
                    Right = 0,
                    Top = rand.Next(Height, (int)game.View.ActualHeight - Height)
                },
            });
            ellipse.Tag = "View_Shape";

            var trail = new SmartShape(new Ellipse());

            trail.StrokeThickness = 5;
            trail.Fill = new SolidColorBrush(System.Windows.Media.Color.FromScRgb(0.99f, 255, 255, 255));
            trail.Width = 100;
            trail.Height = 100;
            trail.Tag = "View_Shape";
            trail.Possition = new System.Windows.Point(ellipse.Possition.X - vec.X, ellipse.Possition.Y - vec.Y);

            game.ManagerShapes.Add(trail);
            game.ManagerShapes.Add(ellipse);

            MainView.Dispatcher.InvokeAsync(() =>
               game.Init());
        }

        private void View_GoToTheBegin(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                game.Camera.GoToTheBegin();
                game.Coords.ToBegin();
            }
            if(e.Key == Key.F2)
            {
                game.Save();
            }

        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var res = game.GetPointMouseInWordl(e.GetPosition(MainView));
            res.X /= game.Camera.Rate_Size;
            res.Y /= game.Camera.Rate_Size;
            res.X = Math.Round(res.X, 2);
            res.Y = Math.Round(res.Y, 2);
            Coords.Content = "Mouse position: " + res + " CameraPosition: " + game.Camera.WorldPosition;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255,255,255,255));
        }
    }
}
