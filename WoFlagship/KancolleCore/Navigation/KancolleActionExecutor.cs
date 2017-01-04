using CefSharp;
using CefSharp.Wpf;
using System;
using System.Windows;


namespace WoFlagship.KancolleCore.Navigation
{
    class KancolleActionExecutor
    {
        const int MOUSEEVENTF_MOVE = 0x0001; //移动鼠标
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; //模拟鼠标左键按下
        const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下
        const int MOUSEEVENTF_RIGHTUP = 0x0010; //模拟鼠标右键抬起
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下
        const int MOUSEEVENTF_MIDDLEUP = 0x0040; //模拟鼠标中键抬起
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;// 标示是否采用绝对坐标

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dwFlags">鼠标事件常数</param>
        /// <param name="dx">指定x，y方向的绝对位置或相对位置 </param>
        /// <param name="dy">指定x，y方向的绝对位置或相对位置 </param>
        /// <param name="cButtons">没有使用</param>
        /// <param name="dwExtraInfo">没有使用</param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private ChromiumWebBrowser webBrowser;

        public KancolleActionExecutor(ChromiumWebBrowser webBrowser)
        {
            this.webBrowser = webBrowser;
        }

        /// <summary>
        /// 执行edge中的action
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool Execute(KancolleActionEdge edge)
        {
            return Execute(edge.Action);
        }

        /// <summary>
        /// 执行action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool Execute(KancolleAction action)
        {

            Application.Current.Dispatcher.Invoke(new Action(() => {
                var host = webBrowser.GetBrowser().GetHost();
                switch (action.ActionType)
                {
                    case ActionTypes.Click:
                        //host.SendMouseMoveEvent((int)action.ActionPosition.X, (int)action.ActionPosition.Y, false, CefEventFlags.None);
                        host.SendMouseClickEvent((int)action.ActionPosition.X, (int)action.ActionPosition.Y, MouseButtonType.Left, false, 1, CefEventFlags.None);
                        host.SendMouseClickEvent((int)action.ActionPosition.X, (int)action.ActionPosition.Y, MouseButtonType.Left, true, 1, CefEventFlags.None);
                        break;
                    case ActionTypes.Move:
                        host.SendMouseMoveEvent((int)action.ActionPosition.X, (int)action.ActionPosition.Y, false, CefEventFlags.None);
                        break;
                }
            }));

            return true;
        }

        /// <summary>
        /// 点击clickPosition位置
        /// </summary>
        /// <param name="clickPosition"></param>
        /// <returns></returns>
        public bool Execute(Point clickPosition)
        {
            return Execute(new KancolleAction(clickPosition));
        }

        private void Move(double x, double y)
        {
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, (int)(x/SystemParameters.PrimaryScreenWidth*65535), (int)(y / SystemParameters.PrimaryScreenHeight * 65535), 0, 0);
        }

        private void Click()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0,0,0,0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0,0, 0, 0);
        }
        
        private void Click(double x, double y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE, (int)(x / SystemParameters.PrimaryScreenWidth * 65535), (int)(y / SystemParameters.PrimaryScreenHeight * 65535), 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE, (int)(x / SystemParameters.PrimaryScreenWidth * 65535), (int)(y / SystemParameters.PrimaryScreenHeight * 65535), 0, 0);
        }
    }
}
