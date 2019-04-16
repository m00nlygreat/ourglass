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

namespace ourglass
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly int[] presetTime = { 15, 20, 30, 45, 60, 90, 120, 150 }; // 시간 콤보박스에 사용하는, 상수 배열
        const string TYPEHERE = "Task명을 입력하세요."; // Task 텍스트 박스에 기본적으로 입력할 내용
        public MainWindow()
        {
            InitializeComponent();
            for (int i=0; i < presetTime.Length; i++) // 시간 콤보박스에 미리 설정된 시간 (분) 추가
            {
                cmbTime.Items.Add(presetTime[i]);
            }
            cmbTime.SelectedIndex = 2;
            tbxTask.Text = TYPEHERE;
        }

        private void TbxTask_GotFocus(object sender, RoutedEventArgs e) // 포커스시 가이드 문구 지우기
        {
            if (tbxTask.Text == TYPEHERE)
            {
                tbxTask.Text = "";
            }
        }

        private void TbxTask_LostFocus(object sender, RoutedEventArgs e) // 입력 않고 떠날 경우 가이드 문구를 다시 노출
        {
            if (tbxTask.Text =="")
            {
                tbxTask.Text = TYPEHERE;
            }
        }
        public string SecToHHMMSS(int sec) // int로 초를 넣으면 HH:MM:SS 형태의 스트링을 되돌려주는 함수를 여기에 만들꺼다.
        {
            TimeSpan time = TimeSpan.FromSeconds(sec);

            //here backslash is must to tell that colon is
            //not the part of format, it just a character that we want in output
            return time.ToString(@"hh\:mm\:ss");
        }
    }
}
