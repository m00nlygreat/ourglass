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
        System.Windows.Threading.DispatcherTimer timeTimer = new System.Windows.Threading.DispatcherTimer(); // 새 타이머 생성
        TimerTask curTask = new TimerTask();
        
        public MainWindow()
        {
            InitializeComponent();

            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea; // 윈도우 위치 설정
            this.Left = desktopWorkingArea.Left;
            this.Top = desktopWorkingArea.Bottom - this.Height;

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
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Paused;
            TaskbarItemInfo.ProgressValue = 1;
        }



        private class TimerTask // 태스크 클래스
        {
            public int timerSet;
            public int timerRemaining;
            public double taskDone;
            public string taskName;
        }

        private void TimerStart(object sender, RoutedEventArgs e) // 타이머 시작
        {
            curTask.timerSet = MinToSec(cmbTime.Text);
            curTask.timerRemaining = MinToSec(cmbTime.Text);
            curTask.taskDone = curTask.timerRemaining / curTask.timerSet;
            curTask.taskName = tbxTask.Text;

            timeTimer.Start();

            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
            TaskbarItemInfo.ProgressValue = 1;
            WindowState = WindowState.Minimized;
        }
        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            if (curTask.timerRemaining == 0) // 시간이 다 되면, 타이머를 종료하고, 프로그레스 바 색상을 노란색으로 변경
            {
                timeTimer.Stop();
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Paused; 
                TaskbarItemInfo.ProgressValue = 1;
            }
            else // 아니면, 남은 시간을 1씩 줄여나가고, 프로그레스 바를 갱신한다.
            {
                curTask.timerRemaining--;
                DisplayTime(curTask.timerRemaining);
                curTask.taskDone = (double)curTask.timerRemaining / curTask.timerSet;
                TaskbarItemInfo.ProgressValue = curTask.taskDone;
                winOurglass.Title = SecToHHMMSS(curTask.timerRemaining);
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
            return time.ToString(@"hh\:mm\:ss");
        }

        public int MinToSec(string min) { return Convert.ToInt16(min) * 60; } // 텍스트로 쓰여진 분 값을 초 단위의 int 값으로 돌려준다.

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(curTask.taskDone.ToString("P1", System.Globalization.CultureInfo.InvariantCulture));

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
                btnStart.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }


    }
}
