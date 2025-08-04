using MauiUseSqlite.Models;

namespace MauiUseSqlite.Dialog;

public partial class UserInputDialog : ContentPage
{
    public User NewUser { get; private set; } = new User();
    private TaskCompletionSource<User> _tcs = new TaskCompletionSource<User>();

    public Task<User> WaitForResultAsync() => _tcs.Task;
    public UserInputDialog()
	{
		InitializeComponent();

    }

    private void OnConfirmClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserNameEntry.Text))
        {
            DisplayAlert("错误", "用户名不能为空", "确定");
            return;
        }



        NewUser.UserName = UserNameEntry.Text;
        NewUser.Password = PasswordEntry.Text;
        NewUser.Role = RoleEntry.Text;

        _tcs.TrySetResult(NewUser);
        Navigation.PopModalAsync();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        _tcs.TrySetResult(null);
        Navigation.PopModalAsync();
    }
}