namespace NTCC.NET.Core.Facility
{
  public interface IUserConfirmation
  {
    bool Confirm(string confirmMessage);
  }
}