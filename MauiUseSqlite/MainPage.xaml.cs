using MauiUseSqlite.Dialog;
using MauiUseSqlite.Helper;
using MauiUseSqlite.Models;
using System.Collections.ObjectModel;

namespace MauiUseSqlite
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<User> _users = new ObservableCollection<User>();
        private User _selectedUser = new User();
        public MainPage()
        {
            InitializeComponent();
            InitData();
        }

        //private List<Student> SingleStudent = new List<Student>();
        private void InitData()
        {
            //建库
            SqlSugarHelper.Db.DbMaintenance.CreateDatabase();//达梦和Oracle不支持建库


            //// 初始化一些示例数据
            //_users.Add(new User { Id = 1, UserName = "Admin", Password = "admin123", Role = "Administrator" });
            //_users.Add(new User { Id = 2, UserName = "User1", Password = "user1123", Role = "User" });

            //usersCollectionView.ItemsSource = _users;


            //建表（看文档迁移）
            SqlSugarHelper.Db.CodeFirst.InitTables<User>(); //所有库都支持   
            UpdateFormFields();
            ////插入
            //if (true)
            //{
            //    var UserList = new List<Student>();
            //    UserList.Add(new Student() { SchoolId = 1, Name = "jack" });
            //    UserList.Add(new Student() { SchoolId = 2, Name = "小明" });
            //    UserList.Add(new Student() { SchoolId = 3, Name = "小红" });
            //    SqlSugarHelper.Db.Insertable(UserList).ExecuteCommand();
            //}

            ////更新
            //if (true)
            //    SqlSugarHelper.Db.Updateable(new Student() { Id = 1, SchoolId = 2, Name = "jack2" }).ExecuteCommand();

            ////删除
            //if (true)
            //    SqlSugarHelper.Db.Deleteable<Student>().Where(it => it.Id == 1).ExecuteCommand();

            ////查询
            //if (true)
            //    SingleStudent = SqlSugarHelper.Db.Queryable<Student>().Where(it => it.Id == 1).ToList();
            ////查询表的所有
            //var list = SqlSugarHelper.Db.Queryable<Student>().ToList();
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchClicked(object sender, EventArgs e)
        {
            string searchTerm = searchEntry.Text?.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                //DisplayAlert("提示", "请输入搜索内容", "确定");
                _users = new ObservableCollection<User>(SqlSugarHelper.Db.Queryable<User>().ToList());
                usersCollectionView.ItemsSource = _users;
                return;
            }

            var results = _users.Where(u => u.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (results.Any())
            {
                usersCollectionView.ItemsSource = new ObservableCollection<User>(results);
                DisplayAlert("搜索结果", $"找到 {results.Count} 个匹配的用户", "确定");
            }
            else
            {
                DisplayAlert("搜索结果", "没有找到匹配的用户", "确定");
            }
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCreateClicked(object sender, EventArgs e)
        {
            // 清空输入框，准备创建新用户
            _selectedUser = new User();
            UpdateFormFields();
        }

        private void OnSaveOrUpdateClicked(object sender, EventArgs e)
        {
            // 从输入框获取数据
            _selectedUser.UserName = userNameEntry.Text;
            _selectedUser.Password = passwordEntry.Text;
            _selectedUser.Role = roleEntry.Text;

            // 简单验证
            if (string.IsNullOrWhiteSpace(_selectedUser.UserName))
            {
                DisplayAlert("错误", "用户名不能为空", "确定");
                return;
            }

            if (string.IsNullOrWhiteSpace(_selectedUser.Password) || _selectedUser.Password.Length < 6)
            {
                DisplayAlert("错误", "密码不能少于6个字符", "确定");
                return;
            }

            if (_selectedUser.Id == 0) // 新增
            {
                var result = SqlSugarHelper.Db.Queryable<User>().Where(it => it.UserName == _selectedUser.UserName).ToList().Count;
                if (result > 0)
                {
                    DisplayAlert("错误", "用户名已存在", "确定");
                    return;
                }
                _selectedUser.Id = _users.Count + 1;
                _users.Add(_selectedUser);
                SqlSugarHelper.Db.Insertable(_selectedUser).ExecuteCommand();
                DisplayAlert("成功", $"用户 {_selectedUser.UserName} 已创建", "确定");
            }
            else // 更新
            {
                var existingUser = _users.FirstOrDefault(u => u.Id == _selectedUser.Id);
                if (existingUser != null)
                {
                    existingUser.UserName = _selectedUser.UserName;
                    existingUser.Password = _selectedUser.Password;
                    existingUser.Role = _selectedUser.Role;

                    SqlSugarHelper.Db.Updateable(existingUser).ExecuteCommand();
                    DisplayAlert("成功", $"用户 {_selectedUser.UserName} 已更新", "确定");
                }
            }

            //// 刷新列表
            //usersCollectionView.ItemsSource = null;
            //usersCollectionView.ItemsSource = _users;

            // 清空表单
            _selectedUser = new User();
            UpdateFormFields();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_selectedUser == null || _selectedUser.Id == 0)
            {
                DisplayAlert("错误", "请先选择要删除的用户", "确定");
                return;
            }

            bool answer = await DisplayAlert("确认", $"确定要删除用户 {_selectedUser.UserName} 吗?", "是", "否");
            if (answer)
            {
                var userToDelete = _users.FirstOrDefault(u => u.Id == _selectedUser.Id);
                if (userToDelete != null)
                {
                    _users.Remove(userToDelete);
                    SqlSugarHelper.Db.Deleteable<User>().Where(it => it.UserName == userToDelete.UserName).ExecuteCommand();
                    DisplayAlert("成功", $"用户 {userToDelete.UserName} 已删除", "确定");

                    // 刷新列表
                    usersCollectionView.ItemsSource = null;
                    usersCollectionView.ItemsSource = _users;

                    // 清空表单
                    _selectedUser = new User();
                    UpdateFormFields();
                }
            }
        }

        private void UpdateFormFields()
        {
            idLabel.Text = _selectedUser.Id.ToString();
            userNameEntry.Text = _selectedUser.UserName;
            passwordEntry.Text = _selectedUser.Password;
            roleEntry.Text = _selectedUser.Role;
            //var list = SqlSugarHelper.Db.Queryable<User>().ToList();
            //usersCollectionView.ItemsSource=list;
            _users = new ObservableCollection<User>(SqlSugarHelper.Db.Queryable<User>().ToList());
            usersCollectionView.ItemsSource = _users;
        }

        // 当用户从列表中选择一项时
        private void UsersCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is User selectedUser)
            {
                _selectedUser = selectedUser;
                //UpdateFormFields();
            }
        }

        private async void AddUserClicked(object sender, EventArgs e)
        {
            var dialog = new UserInputDialog();
            await Navigation.PushModalAsync(new NavigationPage(dialog));

            var result = await dialog.WaitForResultAsync();

            if (result != null)
            {
                // 检查用户名是否已存在
                var exists = SqlSugarHelper.Db.Queryable<User>()
                    .Where(u => u.UserName == dialog.NewUser.UserName)
                    .Count() > 0;

                if (exists)
                {
                    await DisplayAlert("错误", "用户名已存在", "确定");
                    return;
                }

                // 设置ID并保存
                dialog.NewUser.Id = _users.Count + 1;
                SqlSugarHelper.Db.Insertable(dialog.NewUser).ExecuteCommand();
                _users.Add(dialog.NewUser);
                UpdateFormFields();
                await DisplayAlert("成功", $"用户 {dialog.NewUser.UserName} 已创建", "确定");

            }
        }

        private async void UpdateUserClicked(object sender, EventArgs e)
        {
            if (_selectedUser == null || _selectedUser.Id == 0)
            {
                await DisplayAlert("提示", "请先选择要修改的用户", "确定");
                return;
            }

            // 创建编辑对话框并传入当前选中的用户
            var dialog = new UserEditDialog(new User
            {
                Id = _selectedUser.Id,
                UserName = _selectedUser.UserName,
                Password = _selectedUser.Password,
                Role = _selectedUser.Role
            });

            await Navigation.PushModalAsync(new NavigationPage(dialog));

            // 等待对话框返回结果
            var result = await dialog.WaitForResultAsync();

            if (result != null)
            {
                // 更新数据库
                await SqlSugarHelper.Db.Updateable(result).ExecuteCommandAsync();

                // 更新内存中的集合
                var index = _users.IndexOf(_users.FirstOrDefault(u => u.Id == result.Id));
                if (index >= 0)
                {
                    _users[index] = result;
                }
                UpdateFormFields();
                await DisplayAlert("成功", $"{_selectedUser.UserName}用户信息已更新", "确定");
            }
        }
    }

}
