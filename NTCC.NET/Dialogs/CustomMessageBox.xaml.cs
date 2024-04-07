using MahApps.Metro.IconPacks;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Media;

namespace NTCC.NET.Dialogs
{
  /// <summary>
  /// Interaction logic for CustomMessageBox.xaml
  /// </summary>
  public partial class CustomMessageBox : Window
  {
    public CustomMessageBox(string Message, MessageType Type, MessageButtons Buttons)
    {
      InitializeComponent();
      DataContext = this;

      txtMessage.Text = Message;
      MessageType = Type;

      switch (Type)
      {
        case MessageType.Info:
          txtTitle.Text = "ИНФОРМАЦИЯ";
          MessageIcon = new PackIcon() { Kind = PackIconKind.InfoOutline, Width= 32, Height=32};
          break;

        case MessageType.Confirmation:
          txtTitle.Text = "ПОДТВЕРЖДЕНИЕ";
          MessageIcon = new PackIcon() { Kind = PackIconKind.HelpOutline, Width = 32, Height = 32 };
          break;

        case MessageType.Success:
          txtTitle.Text = "УСПЕШНО";
          MessageIcon = new PackIcon() { Kind = PackIconKind.CheckboxMarkedCircleOutline, Width = 32, Height = 32 };
          break;

        case MessageType.Warning:
          txtTitle.Text = "ВНИМАНИЕ";
          MessageIcon = new PackIcon() { Kind = PackIconKind.WarningOutline, Width = 32, Height = 32 };
          break;

        case MessageType.Error:
          txtTitle.Text = "ОШИБКА";
          MessageIcon = new PackIcon() { Kind = PackIconKind.AlertCircleOutline, Width = 32, Height = 32 };
          break;
      }

      IconContent.Content = MessageIcon;

      switch (Buttons)
      {
        case MessageButtons.OkCancel:
          btnYes.Visibility = Visibility.Collapsed; 
          btnNo.Visibility = Visibility.Collapsed;
          break;
        case MessageButtons.YesNo:
          btnOk.Visibility = Visibility.Collapsed;
          btnCancel.Visibility = Visibility.Collapsed;
          break;
        case MessageButtons.Ok:
          btnOk.Visibility = Visibility.Visible;
          btnCancel.Visibility = Visibility.Collapsed;
          btnYes.Visibility = Visibility.Collapsed; 
          btnNo.Visibility = Visibility.Collapsed;
          break;
      }
    }
    public PackIcon MessageIcon = null;

    public MessageType MessageType 
    { 
      get; 
      set; 
    }

    private void btnYes_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

    private void btnOk_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void btnNo_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }

  

  public enum MessageType
  {
    Info,
    Confirmation,
    Success,
    Warning,
    Error,
  }
  public enum MessageButtons
  {
    OkCancel,
    YesNo,
    Ok,
  }
}
