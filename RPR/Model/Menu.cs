using RPR.Model;
using RPR.Shapes;
using RPR.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPR
{
    static public class ShellMenu
    {
        static public SmartShape? TargetShape { get; set; }
        static public StackPanel MenuView { get; set; }

        static public void DeleteTargetShape()
        {
            if (TargetShape != null) 
            { 
                GameView.World.ManagerShapes.Remove(TargetShape);
            }
        }

        static public void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TargetShape == null) return;
            try
            {
                var box = (TextBox)sender;
                var Name = box.Name;
                switch (Name)
                {
                    case "TFill":
                        TargetShape.SetFill(new SolidColorBrush((Color)ColorConverter.ConvertFromString(box.Text)));
                        break;
                    case "TWidth":
                        TargetShape.SetWidth(double.Parse(box.Text));
                        break;
                    case "THeight":
                        TargetShape.SetHeight(double.Parse(box.Text));
                        break;
                    case "TStroke":
                        TargetShape.SetStroke(new SolidColorBrush((Color)ColorConverter.ConvertFromString(box.Text)));
                        break;
                    case "TStrokeThickness":
                        TargetShape.SetStrokeThickness(double.Parse(box.Text));
                        break;
                    case "TTag":
                        TargetShape.SetTag(box.Text);
                        break;
                    case "TPossition":
                        var temp = box.Text.Split(';');
                        TargetShape.Possition = new Point(double.Parse(temp[0]), double.Parse(temp[1]));
                        break;
                    case "TVelocity":
                        temp = box.Text.Split(';');
                        TargetShape.Velocity = new Vector(double.Parse(temp[0]), double.Parse(temp[1]));
                        break;
                    //case "TName_Rule":
                    //    TargetShape.Rules[0].Name = box.Text;
                    //    TargetShape.CompileAllRules();
                    //    break;
                }
            }
            catch (Exception ex)
            {

            }

        }

        static public void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (TargetShape == null) return;
            try
            {
                var panel = (StackPanel)MenuView.FindName("StackMenu1");
                var box = (TextBox)MenuView.FindName("TRule");
                var box_name = (TextBox)MenuView.FindName("TName_Rule");
                var rule = new Model.Rule();
                rule.Name = box_name.Text;
                rule.TextRules.Add(box.Text);
                TargetShape.UpdateRule(rule, 0);
                TargetShape.CompileAllRules();
                if(!TargetShape.ProcessBegin)TargetShape.StartFollowRulles();
            }
            catch(Exception ex)
            {

            }
        }

        static public void OpenMenu()
        {
            if (TargetShape == null)
            {
                var panel = (StackPanel)MenuView.FindName("StackMenu0");
                panel.Visibility = System.Windows.Visibility.Visible;

                panel = (StackPanel)MenuView.FindName("StackMenu1");
                panel.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                var panel = (StackPanel)MenuView.FindName("StackMenu0");
                panel.Visibility = System.Windows.Visibility.Hidden;

                panel = (StackPanel)MenuView.FindName("StackMenu1");
                panel.Visibility = System.Windows.Visibility.Visible;

                var temp = (TextBox)panel.FindName("TFill");
                (temp).Text = TargetShape.GetFill().ToString();
                temp = (TextBox)panel.FindName("TWidth");
                (temp).Text = TargetShape.GetWidth().ToString();
                temp = (TextBox)panel.FindName("THeight");
                (temp).Text = TargetShape.GetHeight().ToString();
                temp = (TextBox)panel.FindName("TStroke");
                (temp).Text = TargetShape?.GetStroke()?.ToString();
                temp = (TextBox)panel.FindName("TStrokeThickness");
                (temp).Text = TargetShape?.GetStrokeThickness().ToString();
                temp = (TextBox)panel.FindName("TPossition");
                (temp).Text = TargetShape.Possition.X.ToString() + ";" + TargetShape.Possition.Y.ToString();
                temp = (TextBox)panel.FindName("TVelocity");
                (temp).Text = TargetShape.Velocity.X.ToString() + ";" + TargetShape.Velocity.Y.ToString();
                temp = (TextBox)panel.FindName("TTag");
                (temp).Text = (string)TargetShape.GetTag();
                temp = (TextBox)panel.FindName("TRule");
                (temp).Text = TargetShape.Rules[0].TextRules[0];
                temp = (TextBox)panel.FindName("TName_Rule");
                (temp).Text = TargetShape.Rules[0].Name;
            }
        }
    }
}
