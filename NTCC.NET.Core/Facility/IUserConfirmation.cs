namespace NTCC.NET.Core.Facility
{
  public interface IUserConfirmation
  {
    bool Confirm();
  }

  sealed public class UserConfirmTrue : IUserConfirmation
  {
    public bool Confirm()
    {
      return true;
    }
  }

  sealed public class UserConfirmFalse : IUserConfirmation
  {
    public bool Confirm()
    {
      return false;
    }
  }

}