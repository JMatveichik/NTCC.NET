using NTCC.NET.Commands;
using NTCC.NET.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NTCC.NET.Pages
{
  /// <summary>
  /// Interaction logic for MessagesPage.xaml
  /// </summary>
  public partial class MessagesPage : UserControl
  {
    public MessagesPage()
    {
      InitializeComponent();
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.MarkAllMessagesAsReaded, MarkAllMessagesAsReadedExecuted, MarkAllMessagesAsReadedCanExecute));
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.ClearMessageList, ClearMessageListExecuted, ClearMessageListCanExecute));

    }

    //TODO : вызвать отписку от получения сообщений для модели данных
    

    private void ClearMessageListCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      MessagesViewModel messagesViewModel = DataContext as MessagesViewModel;
      if (messagesViewModel != null)
        e.CanExecute = messagesViewModel.MessagesList.Count > 0 ? true : false;
      else
        e.CanExecute = false;
    }

    private void ClearMessageListExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      MessagesViewModel messagesViewModel = DataContext as MessagesViewModel;
      if (messagesViewModel != null)
      {
        messagesViewModel.Clear();
      }
    }

    private void MarkAllMessagesAsReadedCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      MessagesViewModel messagesViewModel = DataContext as MessagesViewModel;
      if (messagesViewModel != null)
        e.CanExecute = messagesViewModel.BadgeValue > 0 ? true : false;
      else
        e.CanExecute = false;
      
    }

    private void MarkAllMessagesAsReadedExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      MessagesViewModel messagesViewModel = DataContext as MessagesViewModel;
      if (messagesViewModel != null)
      {
        messagesViewModel.MarkMessageAsReaded(null);
      }
    }

    private void MessagesDataGridMouseMove(object sender, MouseEventArgs e)
    {
      Point mousePosition = e.GetPosition(messagesDataGrid);
      IInputElement elementUnderMouse = messagesDataGrid.InputHitTest(mousePosition);

      var cell = elementUnderMouse as DataGridCell;
      if (cell != null)
      {
        var row = DataGridRow.GetRowContainingElement(cell);
        var item = row.Item;  // Это ваш элемент в списке данных
        if (item != null)
        {
          // Здесь можно что-то делать с элементом
          MessagesViewModel viewModel = (MessagesViewModel)DataContext;
          viewModel.MarkMessageAsReaded(item as FacilityMessage);
        }
      }
    }
  }
}
