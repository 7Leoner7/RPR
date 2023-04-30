using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;
using RPR.Model;
using RPR.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace RPR.Shapes
{
    public interface ISmartShape
    {
        public Guid ID { get; init; }

        public Geometry GetGeometry();

        public Brush GetFill();
        public void SetFill(Brush brush);

        public double GetWidth();
        public void SetWidth(double width);

        public double GetHeight();
        public void SetHeight(double height);

        public Brush GetStroke();
        public void SetStroke(Brush brush);

        public double GetStrokeThickness();
        public void SetStrokeThickness(double thickness);

        public object GetTag();
        public void SetTag(object tag);

        public Point Possition { get; set; }
    }

    public interface IMoveable
    {
        public Vector Velocity { get; set; }
    }

    public class SmartShape : ISmartShape, IMoveable, IRuleable
    {
        protected string assemblyPath = Directory.GetCurrentDirectory() + "\\Rules";

        private static string _sourceCode = @"
using System;
using System.Windows;
using RPR.Model;
using RPR.View;
using RPR.ViewModel;
using RPR.Shapes;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Runtime;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

public class Rule
{
    public static void Main(ArgsSmartShapes args)
    {
        !
    }
}";

        protected Shape Shape { get; set; }

        protected string _xmlShape;

        public Guid ID { get; init; }

        public Vector Velocity { get; set; }

        public Matrix Matrix { get; set; }

        public string XmlShape
        {
            get =>
                XamlWriter.Save(Shape);
            set
            {
                _xmlShape = value;
                InitShapeFromXml();
            }

        }

        public JObject? Data
        {
            get;
            set;
        }

        public Geometry GetGeometry() => Shape.RenderedGeometry.GetWidenedPathGeometry(new Pen());

        public Brush GetFill() => Shape.Fill;
        public void SetFill(Brush brush) => Shape.Fill = brush;

        public double GetWidth() => Shape.Width;
        public void SetWidth(double width) => Shape.Width = width;

        public double GetHeight() => Shape.Height;
        public void SetHeight(double height) => Shape.Height = height;

        public Brush? GetStroke() => Shape.Stroke;
        public void SetStroke(Brush brush) => Shape.Stroke = brush;

        public double GetStrokeThickness() => Shape.StrokeThickness;
        public void SetStrokeThickness(double thickness) => Shape.StrokeThickness = thickness;

        public object GetTag() => Shape.Tag;
        public void SetTag(object tag) => Shape.Tag = tag;

        protected Point possition;

        /// <summary>
        /// Possition relative center of shape bounds
        /// </summary>
        public Point PossitionRC
        {
            get
            {
                return new Point(possition.X - Shape.Width / 2, possition.Y - Shape.Height / 2);
            }
        }

        public Point Possition
        {
            get
            {
                return possition;
            }
            set
            {
                possition = value;
                if (GameView.World == null) return;

                var temp_point = GameView.World.Camera.GetMarginPointFromPossition(value);
                Shape.Margin = new Thickness()
                {
                    Bottom = 0,
                    Right = 0,
                    Left = temp_point.X,
                    Top = temp_point.Y,
                };
            }
        }

        public List<Rule> Rules { get; protected set; }

        public bool IsFollowInnerRules { get; set; }

        public bool IsFollowOuterRules { get; set; }

        public Shape GetShape() =>
            Shape;

        public ISmartShape GetInterface() =>
            this;

        private bool InRange(double number, double min, double max)=>
            number>=min && number<=max;

        public bool IsCollisionBounds(SmartShape sender)//???
        {
            if (Shape.RenderedGeometry == null) return false;
            if (sender.GetShape().RenderedGeometry == null) return false;

            //if(GetFill().ToString() == "#FFFFFFAA")
            //{

            //}

            var colllisX = InRange(Possition.X, sender.Possition.X, sender.Possition.X + sender.GetWidth())||
                InRange(Possition.X + GetWidth(), sender.Possition.X, sender.Possition.X + sender.GetWidth())||
                InRange(sender.Possition.X, Possition.X, Possition.X + GetWidth()) ||
                InRange(sender.Possition.X + sender.GetWidth(), Possition.X, Possition.X + GetWidth());

            var colllisY = InRange(Possition.Y, sender.Possition.Y - sender.GetHeight(), sender.Possition.Y) ||
                InRange(Possition.Y - GetHeight(), sender.Possition.Y - sender.GetHeight(), sender.Possition.Y) ||
                InRange(sender.Possition.Y, Possition.Y - GetHeight(), Possition.Y) ||
                InRange(sender.Possition.Y - sender.GetHeight(), Possition.Y - GetHeight(), Possition.Y);

            return colllisX && colllisY;
        }

        public bool FindCollissionBetweenShapes(SmartShape caller) //Non-implemented
        {
            var geometry_sender = Shape.RenderedGeometry.GetWidenedPathGeometry(new Pen()).Figures;
            var geometry_caller = caller.Shape.RenderedGeometry.GetWidenedPathGeometry(new Pen()).Figures;

            var vec = Possition - caller.Possition;
            
            return false;
        }

        public bool ProcessBegin { get; set; }

        public void StartFollowRulles()
        {
            if (Rules.Count == 0) return;

            if (Rules[0].GetInstruction() == null) InitShape();

            ProcessBegin = true;

            Shape.Dispatcher.Invoke(async () =>
            {
                while (ProcessBegin)
                {
                    if (ManagerShapes.ProcessIsStoped)
                    {
                        return;
                    }
                    if (World.TimeStoped)
                    {
                        await Task.Delay(100);
                        continue;
                    }
                    var args = ManagerShapes.GetArgsSmartShape(this);
                    if (args == null) continue;
                    Rules[0].GetInstruction()?.Invoke(args);
                    await Task.Delay(1);
                }
            });
        }

        private void MouseGetShape(object sender, MouseEventArgs e)
        {
            ShellMenu.TargetShape = this;
            ShellMenu.OpenMenu();
        }

        public void InitShape()
        {
            if (Shape == null) return;

            Shape.MouseDown += MouseGetShape;

            if (Rules.Count > 0)
            {
                CreateRulesOfAssembly(this.assemblyPath + "\\" + Rules[0].Name + ".dll", 0);
                return;
            }
            //  return;
            //var CollisionRule = @"var rand = new Random();
            //args.Sender.Possition += new Vector(Math.Cos(args.WorldArgs.CurrentTime/100d), Math.Sin(args.WorldArgs.CurrentTime/100d));
            //if(args.Bound_Collision){
            //    var vector = args.Sender.Possition - args.Called_Shape.Possition;
            //    args.Sender.Possition += vector;
            //    args.Called_Shape.Possition -= vector;
            //    args.Sender.SetFill(new SolidColorBrush(Color.FromArgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))));
            //    args.Sender.SetStroke(new SolidColorBrush(Color.FromArgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))));
            //}";
            Rule rule = new Rule();
            
            rule.TextRules.Add("");
            Rules.Add(rule);
            //DirectoryInfo info = new DirectoryInfo(assemblyPath);
            //if (!info.Exists)
            //    Directory.CreateDirectory(info.FullName);
            //CompileAllRules();
        }

        public bool CreateRulesOfAssembly(string assemblyPath, int num_rule)
        {
            try
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
                
                if (assembly == null) return false;
                Action<ArgsSmartShapes> action = (Action<ArgsSmartShapes>)Delegate.CreateDelegate(typeof(Action<ArgsSmartShapes>),
                    assembly.GetType("Rule").GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod));
               
                Rules[num_rule].SetInstrution(action);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CompileAllRules()
        {
            for (int i = 0; i < Rules.Count; i++)
            {
                CreateRulesOfAssembly(assemblyPath + "\\" + CompileRule(i) + ".dll", i);
            }
        }

        /// <summary>
        /// При удачной компиляции возвращает имя IRule
        /// </summary>
        /// <param name="number_rule"></param>
        /// <returns></returns>
        public string? CompileRule(int number_rule)
        {
            if (Rules.Count == 0) return null;
            if (Rules[number_rule].GetInstruction() != null) return Rules[number_rule].Name;

            var textRules = string.Join(null, Rules[number_rule].TextRules);

            CSharpParseOptions option = new CSharpParseOptions(LanguageVersion.CSharp8, preprocessorSymbols: new List<string>() { "Debug" }); // Определяем версию C # и объем переданной прекомпиляции
            var tree = CSharpSyntaxTree.ParseText(_sourceCode.Replace("!", textRules), option);
            Debug.Assert(tree.GetDiagnostics().ToList().Count == 0);

            var compileOption = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(
            "Rule",
            new List<SyntaxTree> { tree }, // Это синтаксическое дерево, созданное ранее
            new List<MetadataReference>() { MetadataReference.CreateFromFile(typeof(int).Assembly.Location) }, // Это хороший способ получить msconlib
            compileOption);
            var t = tree.GetDiagnostics().ToList().Count;
            Debug.Assert(t == 0); // Если раньше была пропущенная ссылка или неправильный метод вызова, это будет отражено здесь

            Rules[number_rule].Name = Rules[number_rule].Name ?? Guid.NewGuid().ToString();

            var assemblyPath = this.assemblyPath + "\\" + Rules[number_rule].Name + ".dll";
            //var pdbPath = @"C:\Roslyn\class1.pdb";

            MetadataReference[] metaDatareferences = {
                MetadataReference.CreateFromFile(typeof(ArgsSmartShapes).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Point).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Shape).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ValueType).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(JObject).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Brush).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile("C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\6.0.12\\System.Runtime.dll"),
            };
            var result = compilation.AddReferences(metaDatareferences).Emit(assemblyPath); // результат указывает, было ли выполнение успешным

            if (!result.Success)
            {
                var message = "";
                foreach (var item in result.Diagnostics)
                {
                    message += item.ToString() + "\n";
                }
                return null;
            }
            return Rules[number_rule].Name;
        }

        public void AddRule(Rule Rule)
        {
            Rules.Add(Rule);
        }

        public void UpdateRule(Rule Rule, int Position_Rule)
        {
            if (Rules.Count > Position_Rule)
            {
                Rules.RemoveAt(Position_Rule);
                Rules.Add(Rule);
            }
        }

        public void DeleteRule(int Position_Rule)
        {
            Rules.RemoveAt(Position_Rule);
        }

        public void DeleteRule(Rule Rule)
        {
            Rules.Remove(Rule);
        }

        public SmartShape(Shape base_shape)
        {
            Rules = new List<Rule>();
            Shape = base_shape;
            ID = Guid.NewGuid();
            InitShape();
        }

        public SmartShape(SmartShape smartShape)
        {
            Shape = smartShape.GetShape();
            Rules = smartShape.Rules;
            ID = smartShape.ID;
            Data = smartShape.Data;
            Possition = smartShape.Possition;
            XmlShape = smartShape.XmlShape;
        }

        public SmartShape()
        {
            Rules = new List<Rule>();
        }

        public void ReInitShape(Shape new_shape)
        {
            if (Shape == null)
                Shape = new_shape;
        }

        protected void InitShapeFromXml()
        {
            XmlReader reader = XmlReader.Create(new StringReader(_xmlShape));
            Shape = (Shape)XamlReader.Load(reader);
        }
    }

    public class ArgsSmartShapes
    {
        public WorldArgs WorldArgs { get; set; }
        public bool Bound_Collision { get; set; }
        public bool Inner_Collision { get; set; }
        public SmartShape? Called_Shape { get; set; }
        public SmartShape Sender { get; set; }
    }
}
