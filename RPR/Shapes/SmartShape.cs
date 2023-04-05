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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace RPR.Shapes
{
    public interface ISmartShape
    {
        public Guid ID { get; init; }

        public Brush Fill { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public Brush Stroke { get; set; }

        public double StrokeThickness { get; set; }

        public Point Possition { get; set; }

        public List<string> TextRules { get; }

        public Geometry Geometry { get; }
    }

    public interface IMoveable
    {
        public Vector Velocity { get; set; }
    }

    public class SmartShape : ISmartShape, IMoveable
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

        protected List<Action<ArgsSmartShapes>> Rules { get; set; }

        public Guid ID { get; init; }

        public Vector Velocity { get; set; }

        public string XmlShape
        {
            get;
            set;
        }

        public JObject? Data
        {
            get;
            set;
        }

        public Geometry Geometry
        {
            get =>
                Shape.RenderedGeometry.GetWidenedPathGeometry(new Pen());
        }

        public List<string> TextRules
        {
            get;
            set;
        }

        public Brush Fill
        {
            get => Shape.Fill;
            set => Shape.Fill = value;
        }

        public double Width
        {
            get => Shape.Width;
            set => Shape.Width = value;
        }

        public double Height
        {
            get => Shape.Height;
            set => Shape.Height = value;
        }

        public Brush Stroke
        {
            get => Shape.Stroke;
            set => Shape.Stroke = value;
        }

        public double StrokeThickness
        {
            get => Shape.StrokeThickness;
            set => Shape.StrokeThickness = value;
        }

        public object Tag
        {
            get =>
                Shape.Tag;
            set =>
                Shape.Tag = value;
        }

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

        public Shape GetShape() =>
            Shape;

        public List<Action<ArgsSmartShapes>> GetRules() =>
            Rules;

        public ISmartShape GetInterface() =>
            this;

        public bool IsCollisionBounds(Shape sender)
        {
            if (Shape.RenderedGeometry == null) return false;
            if (sender.RenderedGeometry == null) return false;
            return Shape.RenderedGeometry.Bounds.IntersectsWith(sender.RenderedGeometry.Bounds);
        }

        public bool FindCollissionBetweenShapes(SmartShape caller)
        {
            var geometry_sender = Shape.RenderedGeometry.GetWidenedPathGeometry(new Pen()).Figures;
            var geometry_caller = caller.Shape.RenderedGeometry.GetWidenedPathGeometry(new Pen()).Figures;

            var vec = Possition - caller.Possition;

            return false;
        }

        protected void InitShape()
        {
            AddRules(@"var rand = new Random(); 
            args.Sender.Fill = new SolidColorBrush(Color.FromArgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));");
            AddRules("args.Sender.Possition = new Point(args.Sender.Possition.X + 1, args.Sender.Possition.Y + 1);");
            DirectoryInfo info = new DirectoryInfo(assemblyPath);
            if (!info.Exists)
                Directory.CreateDirectory(info.FullName);
            if (!SetRules(assemblyPath + "\\" + ID + ".dll"))
                CompileRules();
        }

        public bool SetRules(string assemblyPath)
        {
            try
            {
                Assembly assembly = Assembly.LoadFile(assemblyPath);

                if (assembly == null) return false;
                Action<ArgsSmartShapes> action = (Action<ArgsSmartShapes>)Delegate.CreateDelegate(typeof(Action<ArgsSmartShapes>),
                    assembly.GetType("Rule").GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod));

                Rules.Add(action);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CompileRules()
        {
            if (TextRules.Count == 0) return;

            CSharpParseOptions option = new CSharpParseOptions(LanguageVersion.CSharp8, preprocessorSymbols: new List<string>() { "Debug" }); // Определяем версию C # и объем переданной прекомпиляции
            var tree = CSharpSyntaxTree.ParseText(_sourceCode.Replace("!", string.Join(null, TextRules)), option);
            Debug.Assert(tree.GetDiagnostics().ToList().Count == 0);

            var compileOption = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(
            "Rule",
            new List<SyntaxTree> { tree }, // Это синтаксическое дерево, созданное ранее
            new List<MetadataReference>() { MetadataReference.CreateFromFile(typeof(int).Assembly.Location) }, // Это хороший способ получить msconlib
            compileOption);
            var t = tree.GetDiagnostics().ToList().Count;
            Debug.Assert(t == 0); // Если раньше была пропущенная ссылка или неправильный метод вызова, это будет отражено здесь

            var assemblyPath = this.assemblyPath + "\\" + ID + ".dll";
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
                throw new Exception(message);
            }

            SetRules(assemblyPath);
        }

        public void AddRules(string Rule)
        {
            TextRules.Add(Rule);
        }

        public SmartShape(Shape base_shape)
        {
            Rules = new List<Action<ArgsSmartShapes>>();
            Shape = base_shape;
            TextRules = new List<string>();
            ID = Guid.NewGuid();
            Serialize();
            InitShape();
        }

        public SmartShape(SmartShape smartShape)
        {
            Shape = smartShape.GetShape();
            TextRules = smartShape.TextRules;
            ID = smartShape.ID;
            Fill = smartShape.Fill;
            Data = smartShape.Data;
            Width = smartShape.Width;
            Height = smartShape.Height;
            Stroke = smartShape.Stroke;
            StrokeThickness = smartShape.StrokeThickness;
            Possition = smartShape.Possition;
            Serialize();
        }

        /// <summary>
        /// Non-init
        /// </summary>
        public SmartShape()
        {
            Rules = new List<Action<ArgsSmartShapes>>();
            TextRules = new List<string>();
            Shape = new AnyShape();
        }

        public void InitShape(Shape new_shape)
        {
            Shape = new_shape;
        }

        public void Serialize()
        {
            XmlShape = XamlWriter.Save(Shape);
        }

        public void Deserialize()
        {
            XmlReader xmlReader = XmlReader.Create(new StringReader(XmlShape));
            var SaveShape = (Shape)XamlReader.Load(xmlReader);
            Shape TempShape = Shape;

            Shape = SaveShape;
            Fill = TempShape.Fill;
            Stroke = TempShape.Stroke;
            StrokeThickness = TempShape.StrokeThickness;
            Width = TempShape.Width;
            Height = TempShape.Height;
            Tag = TempShape.Tag;
            Possition = new SmartShape(TempShape).Possition;

            InitShape();
        }
    }

    public class ArgsSmartShapes
    {
        public bool Bound_Collision { get; set; }
        public bool Inner_Collision { get; set; }
        public SmartShape? Called_Shape { get; set; }
        public SmartShape Sender { get; set; }
    }
}
