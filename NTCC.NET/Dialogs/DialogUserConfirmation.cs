using NTCC.NET.Core.Facility;

namespace NTCC.NET.Dialogs
{
  public class DialogUserConfirmation : IUserConfirmation
  {

    public DialogUserConfirmation(string confirmationMessage)
    {
      message = confirmationMessage;
    }

    private string message = null;

    public bool Confirm()
    {

      bool? Result = null;

      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        CustomMessageBox dialog = new CustomMessageBox(message, MessageType.Confirmation, MessageButtons.YesNo);
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