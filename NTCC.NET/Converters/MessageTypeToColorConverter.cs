using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace NTCC.NET.Converters
{
  internal class MessageTypeToColorConverter : IValueConverter
  {

    /// <summary>
    /// Кисть фона информационных сообщений
    /// </summary>
    public Brush InfoMessageBrush 
    { 
      get; 
      set; 
    } = new SolidColorBrush(Colors.Green);

    /// <summary>
    /// Кисть фона трассировочных сообщений
    /// </summary>
    public Brush TraceMessageBrush
    {
      get; 
      set; 
    } = new SolidColorBrush(Colors.LightGray);

    /// <summary>
    /// Кисть фона отладочных  сообщений
    /// </summary>
    public Brush DebugMessageBrush 
    { 
      get; 
      set; 
    } = new SolidColorBrush(Colors.DarkGray);

    /// <summary>
    ///Кисть фона  удачных дейтсвий
    /// </summary>
    public Brush SuccessMessageBrush 
    { 
      get; 
      set; 
    } = new SolidColorBrush(Colors.LightGreen);

    /// <summary>
    /// Кисть фона предупредительных сообщения
    /// </summary>
    public Brush WarningMessageBrush 
    { 
      get; 
      set; 
    } = new SolidColorBrush(Colors.DarkOrange);

    /// <summary>
    /// Кисть фона ошибок процесса
    /// </summary>
    public Brush ErrorMessageBrush 
    { 
      get; 
      set; 
    } = new SolidColorBrush(Colors.DarkRed);

    /// <summary>
    /// Кисть фона исключительных ситуаций
    /// </summary>
    public Brush ExceptionMessageBrush 
    { 
      get; 
      set; 
    } = new SolidColorBrush(Colors.DarkViolet);

    /// <summary>
    /// Кисть фона по умолчанию
    /// </summary>
    public Brush DefaultMessageBrush
    {
      get; 
      set; 
    } = new SolidColorBrush(Colors.Black);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is NTCC.NET.Core.Facility.MessageType messageType)
      {
        switch (messageType)
        {
          case NTCC.NET.Core.Facility.MessageType.Trace:
            return TraceMessageBrush;
           
          case NTCC.NET.Core.Facility.MessageType.Debug:
            return DebugMessageBrush;
            
          case NTCC.NET.Core.Facility.MessageType.Info:
            return InfoMessageBrush;
            
          case NTCC.NET.Core.Facility.MessageType.Success:
            return SuccessMessageBrush;
           
          case NTCC.NET.Core.Facility.MessageType.Warning:
            return WarningMessageBrush;
            
          case NTCC.NET.Core.Facility.MessageType.Error:
            return ErrorMessageBrush;
           
          case NTCC.NET.Core.Facility.MessageType.Exception:
            return ExceptionMessageBrush;
            
          default:
            return DefaultMessageBrush;
           
        }
      }
      else if (value is NTCC.NET.Dialogs.MessageType dialogType)
      {
        switch (dialogType)
        {
          case NTCC.NET.Dialogs.MessageType.Info:
            return InfoMessageBrush;
          
          case NTCC.NET.Dialogs.MessageType.Confirmation:
            return InfoMessageBrush;
          
          case NTCC.NET.Dialogs.MessageType.Success:
            return SuccessMessageBrush;

          case NTCC.NET.Dialogs.MessageType.Warning:
            return WarningMessageBrush;

          case NTCC.NET.Dialogs.MessageType.Error:
            return ErrorMessageBrush;

          default:
            return DefaultMessageBrush;
        }
      }

      return DefaultMessageBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
