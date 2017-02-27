using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCore.Navigation
{
    public class KancolleTaskResult
    {
        public KancolleTask FinishedTask { get; private set; }

        public KancolleTaskResultType ResultType { get; private set; }

        /// <summary>
        /// 是否成功完成
        /// </summary>
        public bool IsSuccess { get { return ResultType == KancolleTaskResultType.Success; } }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// 如果错误，则表示错误代码
        /// 0表示无错误
        /// </summary>
        public int ErrorCode { get; private set; }

        public KancolleTaskResult(KancolleTask finishedTask, KancolleTaskResultType type, string message, int errorcode)
        {
            FinishedTask = finishedTask;
            ResultType = type;
            Message = message;
            ErrorCode = errorcode;
        }

        public override string ToString()
        {
            string str = "";
            if (IsSuccess)
                str += "成功";
            else
                str += "失败";
            str += "\t" + ErrorCode + "\t"+Message;

            return str;
        }
    }

    public class KancolleTaskResultErrors
    {
        public const int Success = 0;
        public const int UnknownTaskType = 1;
      

        #region ReachScene
        public const int NoNavigationPath = 0x0101;
        public const int UnexceptedScene = 0x0102;
        public const int UnexceptedState = 0x0103;
        #endregion


        #region LockNowAndWaitForResponse
        public const int NoResponse = 0x0201;
        #endregion

        #region Organize
        public const int IncorrectDeckIndex = 0x1001;
        public const int IllegalShipCount = 0x1002;
        public const int EmptyFirstShip = 0x1003;
        public const int EmptyInterveningShip = 0x1004;
        public const int UnfoundShipNo = 0x1005;
        #endregion

        #region Map
        public const int UnfoundMapId = 0x2001;
        #endregion

        #region Remodel
        public const int NoShipAtPosition = 0x3001;
        public const int UnfoundItemNo = 0x3002;
        #endregion
    }
}
