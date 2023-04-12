using RPR.Model;
using RPR.Shapes;
using RPR.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
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
            InnerMenu.Visibility = Visibility.Hidden;
            MenuButton.Margin = new Thickness(0, MenuButton.Margin.Top, 0, 0);
            MenuButton.Content = "<";
        }

        private void View_GoToTheBegin(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                game.World.Camera.GoToTheBegin();
                game?.Coords?.ToBegin();
            }
            if (e.Key == Key.F2)
            {
                game.Save();
            }

        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //var res = game.GetPointMouseInWordl(e.GetPosition(MainView));
            //res.X = Math.Round(res.X, 2);
            //res.Y = Math.Round(res.Y, 2);
            // Coords.Content = "Mouse position: " + res; //+ " CameraPosition: " + game.Camera.WorldPosition;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Content.Equals(">"))
            {
                button.Content = "<";
                InnerMenu.Visibility = Visibility.Hidden;
                button.Margin = new Thickness(0, button.Margin.Top, 0, 0);
            }
            else
            {
                InnerMenu.Visibility = Visibility.Visible;
                button.Content = ">";
                button.Margin = new Thickness(0, button.Margin.Top, InnerMenu.ActualWidth, 0);
            }
        }

        private void StopResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (World.TimeStoped)
            {
                World.TimeStoped = false;
                StopResumeButton.Background = new SolidColorBrush(Colors.Green);
                StopResumeButton.Dispatcher.Invoke(() =>
                {
                    game.World.TimeLoop();
                });
            }
            else
            {
                World.TimeStoped = true;
                StopResumeButton.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private Shape? target;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            target = null;
            var tag = (string)((Button)sender).Tag;
            switch (tag)
            {
                case "Ellipse":
                    target = new Ellipse()
                    {
                        Width = 100,
                        Height = 100,
                        Fill = new SolidColorBrush(Colors.White),
                        Tag = "View_Shape",
                    };
                    break;
                case "Rectangle":
                    target = new Rectangle()
                    {
                        Width = 100,
                        Height = 100,
                        Fill = new SolidColorBrush(Colors.White),
                        Tag = "View_Shape",
                    };
                    break;
                case "Line":
                    target = new Line()
                    {
                        StrokeThickness = 2,
                        Stroke = new SolidColorBrush(Colors.White),
                        Tag = "View_Shape",
                        X1 = 0,
                        Y1 = 0,
                        X2 = 50,
                        Y2 = 50,
                    };
                    break;
            }

            MainView.Cursor = Cursors.Pen;
        }

        private void MainView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (target == null) return;
            SmartShape shape = new SmartShape();
            shape.XmlShape = XamlWriter.Save(target);
            var res = e.GetPosition(MainView);
            res.X = Math.Round(res.X, 2);
            res.Y = Math.Round(res.Y, 2);
            shape.Possition = res;
            game.World.ManagerShapes.Add(shape);

            game.Update();
            target = null;
            MainView.Cursor = Cursors.Arrow;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if(SavedWorldName.Text == string.Empty) return;

            game = new GameView(ref MainView, SavedWorldName.Text);
            StartMenu.Visibility = Visibility.Hidden;
            MainView.Dispatcher.InvokeAsync(() =>
               game?.Init());
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorldName.Text == string.Empty) return;
            game = new GameView(ref MainView, WorldName.Text);
            game.World.MetaData.Description = DescriptionWorld.Text;

            //Test Init
            var rand = new Random();
            var Width = 100;
            var Height = 100;
            var vec = new Vector(0.4, 0.5);
            var speed = 5 * 7;
            vec.X *= speed;
            vec.Y *= speed;
            for (int i = 0; i < 2; i++)
            {
                var ellipse = new SmartShape(new Rectangle()
                {
                    Width = Width,
                    Height = Height,
                    Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))),
                    Margin = new Thickness()
                    {
                        Bottom = 0,
                        Left = rand.Next(Width, (int)game.View.ActualWidth - Width),
                        Right = 0,
                        Top = rand.Next(Height, (int)game.View.ActualHeight - Height)
                    },
                    StrokeThickness = 15,
                });
                ellipse.Velocity = new Vector(0, 0);
                ellipse.SetTag("View_Shape");

                game?.World?.ManagerShapes.Add(ellipse);
            }
            //

            StartMenu.Visibility = Visibility.Hidden;

            MainView.Dispatcher.InvokeAsync(() =>
               game?.Init());
        }
    }
}
