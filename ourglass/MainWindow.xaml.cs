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


        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < presetTime.Length; i++) // 시간 콤보박스에 미리 설정된 시간 (분) 추가
            {
                cmbTime.Items.Add(presetTime[i]);
            }
            cmbTime.SelectedIndex = 2;
            DisplayTime();
            tbxTask.Text = TYPEHERE;

            timeTimer.Tick += new EventHandler(TimeTimer_Tick); // 타이머 초기 설정
            timeTimer.Interval = new TimeSpan(0, 0, 1);
            TimerTask curTask = new TimerTask();
            
        }

        private class TimerTask // 태스크 클래스
        {
            public int timerSet;
            public int timerElapsed;
            public double taskDone;
            public string taskName;
        }

        private void TimerStart(object sender, RoutedEventArgs e) // 타이머 시작
        {
            //MessageBox.Show("시작됨");
            
            timeTimer.Start();
            
            
        }
        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            
        }


            private void DisplayTime() // 입력된 분 값을, HH:MM:SS 형태로 변경하여 표시
        {
            try { lblTimeDisplay.Content = SecToHHMMSS(Convert.ToInt32(cmbTime.Text)); } catch { }
        }

        public string SecToHHMMSS(int sec) // int로 초를 넣으면 HH:MM:SS 형태의 스트링을 되돌려주는 함수를 여기에 만들꺼다.
        {
            TimeSpan time = TimeSpan.FromSeconds(MinToSec(sec));
            return time.ToString(@"hh\:mm\:ss");
        }

        public int MinToSec(int min) { return min * 60; }


        private void Button_Click(object sender, RoutedEventArgs e) // 테스트 용도로 쓰는 버튼
        {
            MessageBox.Show(cmbTime.Text);
        }




        // 아래는 UI 기믹이다.
        private void CmbTime_DropDownClosed(object sender, EventArgs e) { DisplayTime(); }
        private void CmbTime_LostFocus(object sender, RoutedEventArgs e) { DisplayTime(); }
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

        private void EnterKeyPressOnInputFields(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)

            {
                btnStart.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        } // 인풋필드에서 엔터키 입력시, 타이머 스타트
    }
}
