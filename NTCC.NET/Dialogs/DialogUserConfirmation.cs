using NTCC.NET.Core.Facility;

namespace NTCC.NET.Dialogs
{
  public class DialogUserConfirmation : IUserConfirmation
  {

    public bool Confirm(string confirmationMessage)
    {

      bool? Result = null;

      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        CustomMessageBox dialog = new CustomMessageBox(confirmationMessage, MessageType.Confirmation, MessageButtons.YesNo);
        Result = dialog.ShowDialog();
      });

      if (!Result.Value)
      {
        return false;
      };

      return true;
    }
  }
}