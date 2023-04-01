using RPR.ViewModel;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            game = new GameView(ref MainView);
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
