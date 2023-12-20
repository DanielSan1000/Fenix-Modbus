using ProjectDataLib;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using io = System.IO;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for TreeViewManager.xaml
    /// </summary>
    public partial class TreeViewManager : UserControl
    {

        public TreeViewManager()
        {                    
            InitializeComponent();
            //Temp = new HierarchicalDataTemplate("Daniel");
            DataContext = this;
        }
    }

    class ImageConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {       

            if(value is Project)
                return new BitmapImage(new Uri("TreeImage/Project.png", UriKind.Relative));

            else if(value is WebServer)
                return new BitmapImage(new Uri("TreeImage/HttpServer.ico", UriKind.Relative));

            else if (value is CusFile)
            {
                CusFile file = (CusFile)value;
                if(file.IsFile)
                {
                    string ext = io.Path.GetExtension(file.FullName);

                    if (ext == ".html")
                        return new BitmapImage(new Uri("TreeImage/HtmlFile.ico", UriKind.Relative));

                    else if (ext == ".js")
                        return new BitmapImage(new Uri("TreeImage/JsFile.ico", UriKind.Relative));

                    else if (ext == ".ico")
                        return new BitmapImage(new Uri("TreeImage/IcoFile.ico", UriKind.Relative));

                    else if (ext == ".jpg")
                        return new BitmapImage(new Uri("TreeImage/JpgFile.ico", UriKind.Relative));

                    else
                        return new BitmapImage(new Uri("TreeImage/File.ico", UriKind.Relative));
                }
                else
                {
                    return new BitmapImage(new Uri("TreeImage/Folder.ico", UriKind.Relative));
                }
             
            }

            else if(value is DatabaseModel)
                return new BitmapImage(new Uri("TreeImage/Database.ico", UriKind.Relative));

            else if (value is ScriptsDriver)
                return new BitmapImage(new Uri("TreeImage/Scripts.png", UriKind.Relative));

            else if(value is ScriptFile)
                return new BitmapImage(new Uri("TreeImage/CsFile.ico", UriKind.Relative));

            else if(value is InternalTagsDriver)
                return new BitmapImage(new Uri("TreeImage/IntTagFol.png", UriKind.Relative));

            else if(value is InTag)
                return new BitmapImage(new Uri("TreeImage/IntTag.png", UriKind.Relative));

            else if (value is Connection)
                return new BitmapImage(new Uri("TreeImage/Connection.png", UriKind.Relative));

            else if (value is Device)
                return new BitmapImage(new Uri("TreeImage/Device.ico", UriKind.Relative));

            else if (value is Tag)
                return new BitmapImage(new Uri("TreeImage/Tag.ico", UriKind.Relative));

            return new BitmapImage(new Uri("TreeImage/File.ico", UriKind.Relative));
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    class StateRunConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if ((bool)values[1])
                return string.Empty;
            else
            {
                if ((bool)values[0])
                    return "[Running]";
                else
                    return string.Empty;
            }
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    class StateBlockConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((bool)value)
                return "[Blocked]";
            else
                return string.Empty;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    class BoolNegConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }
    }

    class TemplateItems : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    class Clr : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Drawing.Color cl = (System.Drawing.Color)value;
            return new SolidColorBrush( Color.FromArgb(cl.A,cl.R,cl.G,cl.B));
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
