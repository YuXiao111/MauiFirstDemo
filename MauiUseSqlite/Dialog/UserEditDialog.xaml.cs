using MauiUseSqlite.Models;

namespace MauiUseSqlite.Dialog;

public partial class UserEditDialog : ContentPage
{
    public User EditingUser { get; private set; }
    private TaskCompletionSource<User> _tcs = new TaskCompletionSource<User>();

    public Task<User> WaitForResultAsync() => _tcs.Task;
    public UserEditDialog(User user)
	{
		InitializeComponent();
        EditingUser = user;
        BindingContext = EditingUser;
    }
    private void OnConfirmClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EditingUser.UserName))
        {
            DisplayAlert("错误", "用户名不能为空", "确定");
            return;
        }


        _tcs.TrySetResult(EditingUser);
        Navigation.PopModalAsync();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        _tcs.TrySetResult(null);
        Navigation.PopModalAsync();
    }
}