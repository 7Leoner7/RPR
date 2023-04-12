using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;
using RPR.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
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
            set {
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

        public Brush GetStroke() => Shape.Stroke;
        public void SetStroke(Brush brush) => Shape.Stroke = brush;

        public double GetStrokeThickness() => Shape.StrokeThickness;
        public void SetStrokeThickness(double thickness) => Shape.StrokeThickness = thickness;
        
        public object GetTag() => Shape.Tag;
        public void SetTag(object tag) => Shape.Tag = tag;

        /// <summary>
        /// Possition relative center of shape bounds
        /// </summary>
        public Point Possition
        {
            get
            {
                return new Point(Shape.Margin.Left + Shape.ActualWidth / 2, Shape.Margin.Top + Shape.ActualHeight / 2);
            }
            set
            {
                Shape.Margin = new Thickness()
                {
                    Left = value.X - Shape.ActualWidth / 2,
                    Top = value.Y - Shape.ActualHeight / 2,
                    Bottom = 0,
                    Right = 0,
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

        protected bool ChisloInRage(double start, double end, double chislo)
        {
            return (start <= chislo) && (end >= chislo);
        }

        public bool IsCollisionBounds(SmartShape sender)
        {
            if (Shape.RenderedGeometry == null) return false;
            if (sender.GetShape().RenderedGeometry == null) return false;

            var ThisPointB = new Point(Possition.X - Shape.Width / 2, Possition.Y - Shape.Height / 2);
            var SenderPointB = new Point(sender.Possition.X - sender.Shape.Width / 2, sender.Possition.Y - sender.Shape.Height / 2);

            var ThisPointE = new Vector(Shape.Width, Shape.Height) + ThisPointB;
            var SenderPointE = new Vector(sender.Shape.Width, sender.Shape.Height) + SenderPointB;

            var collision = (ChisloInRage(ThisPointB.X, ThisPointE.X, SenderPointB.X) || ChisloInRage(ThisPointB.X, ThisPointE.X, SenderPointE.X)) &&
                (ChisloInRage(ThisPointB.Y, ThisPointE.Y, SenderPointB.Y) || ChisloInRage(ThisPointB.Y, ThisPointE.Y, SenderPointE.Y));

            return collision;
        }

        public bool FindCollissionBetweenShapes(SmartShape caller) //Non-implemented
        {
            var geometry_sender = Shape.RenderedGeometry.GetWidenedPathGeometry(new Pen()).Figures;
            var geometry_caller = caller.Shape.RenderedGeometry.GetWidenedPathGeometry(new Pen()).Figures;

            var vec = Possition - caller.Possition;

            return false;
        }

        public void StartFollowRulles()
        {
            if(Rules.Count== 0) return;

            if (Rules[0].GetInstruction() == null) InitShape();
            Shape.Dispatcher.Invoke(async () => {
                while (true)
                {
                    if (ManagerShapes.ProcessIsStoped)
                    {
                        return;
                    }
                    if (World.TimeStoped) { 
                        await Task.Delay(10); 
                        continue; 
                    }
                    var args = ManagerShapes.GetArgsSmartShape(this);
                    if (args == null) continue;
                    Rules[0].GetInstruction()?.Invoke(args);
                    await Task.Delay(1);
                }
            });
        }

        protected void InitShape()
        {
            if (Rules.Count > 0)
            {
                CreateRulesOfAssembly(this.assemblyPath + "\\" + Rules[0].Name + ".dll", 0);
                return;
            }
            //  return;
            var CollisionRule = @"
            if(args.Bound_Collision){
                var vector = args.Sender.Possition - args.Called_Shape.Possition;
                args.Sender.Possition += vector;
                args.Called_Shape.Possition -= vector;
                args.Sender.SetFill(new SolidColorBrush(Color.FromArgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))));
                args.Sender.SetStroke(new SolidColorBrush(Color.FromArgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))));
            }";
            Rule rule = new Rule();

            rule.TextRules.Add(@"var rand = new Random();");
            rule.TextRules.Add("args.Sender.Possition = new Point(args.Sender.Possition.X + Math.Cos(args.WorldArgs.CurrentTime/100d), args.Sender.Possition.Y + Math.Sin(args.WorldArgs.CurrentTime/100d));");
            rule.TextRules.Add(CollisionRule);
            Rules.Add(rule);
            DirectoryInfo info = new DirectoryInfo(assemblyPath);
            if (!info.Exists)
                Directory.CreateDirectory(info.FullName);
            CompileAllRules();
        }

        public bool CreateRulesOfAssembly(string assemblyPath, int num_rule)
        {
            try
            {
                Assembly assembly = Assembly.LoadFile(assemblyPath);

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
            if (Rules.Count < Position_Rule)
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
