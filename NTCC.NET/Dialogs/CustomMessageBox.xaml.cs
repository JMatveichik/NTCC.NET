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
      txtMessage.Text = Message;

      switch (Type)
      {
        case MessageType.Info:
          txtTitle.Text = "Info";
          MessageIcon = new PackIconMaterial() { Kind = PackIconMaterialKind.Information, Width = 48, Height = 48, Foreground = Brushes.DarkBlue };
          break;

        case MessageType.Confirmation:
          txtTitle.Text = "Confirmation";
          MessageIcon = new PackIconMaterial() { Kind = PackIconMaterialKind.HelpCircleOutline, Width = 48, Height = 48, Foreground = Brushes.DarkBlue };
          break;

        case MessageType.Success:
          txtTitle.Text = "Success";
          MessageIcon = new PackIconMaterial() { Kind = PackIconMaterialKind.CheckboxMarkedCircleOutline, Width = 48, Height = 48, Foreground = Brushes.DarkBlue };
          break;

        case MessageType.Warning:
          txtTitle.Text = "Warning";
          MessageIcon = new PackIconMaterial() { Kind = PackIconMaterialKind.AlertOutline, Width = 48, Height = 48, Foreground = Brushes.DarkBlue };
          break;

        case MessageType.Error:
          txtTitle.Text = "Error";
          MessageIcon = new PackIconMaterial() { Kind = PackIconMaterialKind.CloseCircleOutline, Width = 48, Height = 48, Foreground = Brushes.DarkBlue };
          break;
      }

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
    public PackIconBase MessageIcon = null;

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
