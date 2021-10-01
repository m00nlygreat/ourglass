using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ourglass
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly int[] presetTime = { 15, 20, 30, 45, 60, 90, 120, 150 }; // 시간 콤보박스에 사용하는, 상수 배열
        const string TYPEHERE = "Task명을 입력하세요."; // Task 텍스트 박스에 기본적으로 입력할 내용
        const string VERSION = "v1.6";
        System.Windows.Threading.DispatcherTimer timeTimer = new System.Windows.Threading.DispatcherTimer(); // 새 타이머 생성
        TimerTask curTask = new TimerTask();
        public static List<TimerTask> prevTasks = new List<TimerTask>();
        
        bool isTasksBeingShown = false;

        public MainWindow()
        {
            InitializeComponent();

            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea; // 윈도우 위치 설정
            this.Left = desktopWorkingArea.Left;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            winOurglass.Title += VERSION;

            for (int i = 0; i < presetTime.Length; i++) // 시간 콤보박스에 미리 설정된 시간 (분) 추가
            {
                cmbTime.Items.Add(presetTime[i]);
            }
            cmbTime.SelectedIndex = 2;
            DisplayTime(MinToSec(cmbTime.Text));
            tbxTask.Text = TYPEHERE;
            tbxTask.Focus();

            timeTimer.Tick += new EventHandler(TimeTimer_Tick); // 타이머 초기 설정
            timeTimer.Interval = new TimeSpan(0, 0, 1);

            TaskbarItemInfo = new TaskbarItemInfo();
            TaskbarItemInfo.ProgressValue = 1;
        }

        public class TimerTask// 태스크 클래스
        {
            public int timerSet;
            public int timerRemaining;
            public double taskDone;
            public string taskName;
            public bool taskBeingDone;
            public object Clone()
            {
                TimerTask copiedTask = new TimerTask();
                copiedTask.timerSet = this.timerSet;
                copiedTask.timerRemaining = this.timerRemaining;
                copiedTask.taskDone = this.taskDone;
                copiedTask.taskName = this.taskName;
                copiedTask.taskBeingDone = this.taskBeingDone;
                return copiedTask;
            }
        }

        private void StartTimer() // 타이머 시작
        {
            if (curTask.taskBeingDone == true) { StopTimer(); } // 기 실행중인 타이머가 있으면 일단 스톱
            curTask.timerSet = MinToSec(cmbTime.Text);
            curTask.timerRemaining = MinToSec(cmbTime.Text);
            curTask.taskDone = curTask.timerRemaining / curTask.timerSet;
            if (tbxTask.Text == TYPEHERE) { curTask.taskName = ""; } else { curTask.taskName = tbxTask.Text; } // Task가 입력되지 않으면, Null을 입력.

            timeTimer.Start();
            btnStart.Content = "Stop";
            curTask.taskBeingDone = true;

            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
            TaskbarItemInfo.ProgressValue = 1;
            WindowState = WindowState.Minimized;
        }
        private void StopTimer()
        {
            timeTimer.Stop();
            prevTasks.Add(curTask.Clone() as TimerTask);
            curTask.taskBeingDone = false;
            curTask.timerSet = MinToSec(cmbTime.Text);
            curTask.timerRemaining = MinToSec(cmbTime.Text);
            curTask.taskDone = curTask.timerRemaining / curTask.timerSet;
            if (tbxTask.Text == TYPEHERE) { curTask.taskName = ""; } else { curTask.taskName = tbxTask.Text; }
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            TaskbarItemInfo.ProgressValue = 1;
            winOurglass.Title = "타이머 종료!";
            btnStart.Content = "Start";
        }
        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            if (curTask.timerRemaining == 0) // 시간이 다 되면, 타이머를 종료하고, 프로그레스 바 색상을 노란색으로 변경
            {
                StopTimer();
            }
            else // 아니면, 남은 시간을 1씩 줄여나가고, 프로그레스 바를 갱신한다.
            {
                curTask.timerRemaining--;
                DisplayTime(curTask.timerRemaining);
                curTask.taskDone = (double)curTask.timerRemaining / curTask.timerSet;
                TaskbarItemInfo.ProgressValue = curTask.taskDone;
                if (string.IsNullOrEmpty(curTask.taskName))
                {
                    winOurglass.Title = SecToHHMMSS(curTask.timerRemaining);
                }
                else
                {
                    winOurglass.Title = curTask.taskName + " - " + SecToHHMMSS(curTask.timerRemaining);
                }
            }
        } // 타이머 틱시 일어나는 일들
        private void LblTimeDisplay_MouseDoubleClick(object sender, MouseButtonEventArgs e) // 타이머 스톱기능이 숨겨져 있읍니다.
        {
            if (timeTimer.IsEnabled == true)
            {
                timeTimer.Stop();
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            }
            else
            {
                if (TaskbarItemInfo.ProgressState == TaskbarItemProgressState.Indeterminate)
                {
                    timeTimer.Start();
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                    WindowState = WindowState.Minimized;
                }
            }
        }


        private void DisplayTime(int sec) // 입력된 초 값을, HH:MM:SS 형태로 변경하여 표시
        {
            try
            {
                lblTimeDisplay.Content = SecToHHMMSS(sec);
            } catch { }
        }

        public string SecToHHMMSS(int sec) // int로 초를 넣으면 HH:MM:SS 형태의 스트링을 되돌려준다.
        {
            TimeSpan time = TimeSpan.FromSeconds(sec);
            //return time.ToString(@"hh\:mm\:ss");
            return time.ToString(@"hh") + BlinkingColon() + time.ToString(@"mm") + BlinkingColon() + time.ToString(@"ss");
        }

        public int MinToSec(string min) { try { return Convert.ToInt16(min) * 60; } catch { return 1800; } } // 텍스트로 쓰여진 분 값을 초 단위의 int 값으로 돌려준다.

        public string BlinkingColon() { if (curTask.timerRemaining % 2 == 1) { return "."; } else { return ":"; } }  // 1초마다 깜빡이는 콜론을 만든다.

        private void BtnTasks_Click(object sender, RoutedEventArgs e) // Task 보여주기 버튼
        {
            if (isTasksBeingShown == false)
            {
                frmTasks.Navigate(new Uri("/Tasks.xaml", UriKind.Relative));
                isTasksBeingShown = true;
            } else
            {
                
                frmTasks.Content = null;
                frmTasks.NavigationService.RemoveBackEntry();

                isTasksBeingShown = false;
            }

            #region 메세지 박스로 보여주는 기존안
            /*            string message = "현재 Task:\n\n";
                        message += string.Format("{0}\t{1}", string.Format("{0:P1}", 1 - curTask.taskDone), (string.IsNullOrEmpty(curTask.taskName)) ? "이름없는 Task" : curTask.taskName);
                        if (prevTasks.Count() != 0)
                        {
                            message += "\n\n이전 Task:\n\n";
                            int i = 1;
                            foreach (TimerTask task in prevTasks)
                            {
                                message += string.Format("{1}\t{0}\n", (string.IsNullOrEmpty(task.taskName)) ? "이름없는 Task " + i++.ToString() : task.taskName, string.Format("{0:P1}", 1 - task.taskDone));
                            }
                        }
                        MessageBox.Show(message);
            */
            //try {MessageBox.Show(prevTasks[0].taskName +" " + prevTasks[1].taskName); } catch { MessageBox.Show("끝난 Task 없음!"); }
            #endregion
        }

        private void btnStart_Clicked(object sender, RoutedEventArgs e) // 타이머 상태에 따라, 버튼 기능 달리함.
        {
            if (curTask.taskBeingDone == true)
            {
                StopTimer();
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            }
            else
            {
                StartTimer();
            }
        }

        // 아래는 UI 기믹이다.
        private void CmbTime_DropDownClosed(object sender, EventArgs e) { if (timeTimer.IsEnabled) { } else { DisplayTime(MinToSec(cmbTime.Text)); } }
        private void CmbTime_LostFocus(object sender, RoutedEventArgs e) { if (timeTimer.IsEnabled) { } else { DisplayTime(MinToSec(cmbTime.Text)); } }
        private void TbxTask_GotFocus(object sender, RoutedEventArgs e) // 포커스시 가이드 문구 지우기
        {
            if (tbxTask.Text == TYPEHERE)
            {
                tbxTask.Text = "";
            }
        }

        private void TbxTask_LostFocus(object sender, RoutedEventArgs e) // 입력 않고 떠날 경우 가이드 문구를 다시 노출
        {
            if (tbxTask.Text == "")
            {
                tbxTask.Text = TYPEHERE;
            }
        }

        private void EnterKeyPressOnInputFields(object sender, KeyEventArgs e) // 인풋필드에서 엔터키 입력시, 타이머 스타트
        {
            if (e.Key == Key.Return)

            {
                StartTimer();
            }
            // 버튼을 누르는 것이 아님. 타이머를 시작함.
        }
        private void WinOurglass_Activated(object sender, EventArgs e) // 타이머가 진행 중이지 않을 경우, 창을 활성화 하면 프로그레스 바 상태를 원래대로
        {
            if (curTask.taskBeingDone == false)
            {
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            }
        }
    }

}
