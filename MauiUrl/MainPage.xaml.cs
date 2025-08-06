namespace MauiUrl
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            OnOpenBrowser();
        }

        // 按钮点击事件
        private async void OnOpenBrowser()
        {
            string url = "https://jsj.top/f/lccN5S"; // 默认网址
            // 验证 URL 格式
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url; // 自动补全协议
            }
            try
            {
                // 方式1：用系统浏览器打开
                //await Launcher.Default.OpenAsync(url);
                // 方式2（可选）：在内嵌 WebView 中加载
                webView.Source = new UrlWebViewSource { Url = url };
            }
            catch (Exception ex)
            {
                await DisplayAlert("错误", $"无法打开链接: {ex.Message}", "确定");
            }
        }

        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Success)
            {
                // 等待页面加载完成
                await Task.Delay(500);

                // 通过JavaScript获取页面内容高度
                var result = await webView.EvaluateJavaScriptAsync(
                    "document.body.scrollHeight.toString()");

                if (double.TryParse(result, out double height))
                {
                    // 设置WebView高度，增加一些缓冲值
                    webView.HeightRequest = height + 50;

                    // 在Android上强制布局更新的正确方式
#if ANDROID
                    // 获取Android原生WebView
                    var androidWebView = webView.Handler.PlatformView as Android.Webkit.WebView;
                    if (androidWebView != null)
                    {
                        // 强制布局更新
                        androidWebView.RequestLayout();
                        // 通知父布局重新计算
                        (webView.Parent as IView)?.InvalidateMeasure();
                    }
#endif
                }
            }
            else
            {
                DisplayAlert("温馨提示", "请检查网络连接！", "确定");
            }
        }
    }
}
