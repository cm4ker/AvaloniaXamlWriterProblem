using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Context;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.PortableXaml;
using Avalonia.Markup.Xaml.Styling;
using Portable.Xaml;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var appBuilder = BuildAvaloniaApp().SetupWithoutStarting();

            Console.WriteLine("Hello World!");

            var str = Repro();
            Console.WriteLine(str);
            
            Window w = AvaloniaXamlLoader.Parse<Window>(str);
            w.DataContext = new { Person = "PersonName" };

            appBuilder.Instance.Run(w);

            w.ShowDialog();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();


        public static string Repro()
        {
            XamlSchemaContext _context = new XamlSchemaContext();
            XamlXmlWriter _xamlWriter;

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            using (XmlWriter xw = XmlWriter.Create(sw, settings))
            {
                using (_xamlWriter = new XamlXmlWriter(xw, _context, new XamlXmlWriterSettings()))
                {
                    XamlType windowType = _context.GetXamlType(typeof(Window));

                    _xamlWriter.WriteStartObject(windowType);
                    var contentProperty = windowType.GetMember("Content");

                    //Content
                    _xamlWriter.WriteStartMember(contentProperty);

                    //TextBox

                    XamlType textBoxType = _context.GetXamlType(typeof(TextBox));


                    var textProperty = textBoxType.GetMember("Text");

                    _xamlWriter.WriteStartObject(textBoxType);
                    
                    //Text
                    _xamlWriter.WriteStartMember(textProperty);
                    
                    XamlType bindingXaml = _context.GetXamlType(typeof(Binding));
                    XamlMember
                        pathProp = bindingXaml
                            .GetMember("Path"); 
                    _xamlWriter.WriteStartObject(bindingXaml);

                    _xamlWriter.WriteStartMember(pathProp);
                    _xamlWriter.WriteValue("Person");
                    _xamlWriter.WriteEndMember();

                    _xamlWriter.WriteEndObject();

                    _xamlWriter.WriteEndMember();

                    _xamlWriter.WriteEndObject();

                    _xamlWriter.WriteEndMember();

                    _xamlWriter.WriteEndObject();
                }
            }

            return sb.ToString();
        }


    }

}
