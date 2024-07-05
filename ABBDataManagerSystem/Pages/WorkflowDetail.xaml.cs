using ABBDataManagerSystem.Bean.Base;
using ABBDataManagerSystem.Pages.Views;
using ABBDataManagerSystem.Tools;
using System.Windows;
using System.Windows.Controls;
using EventManager = ABBDataManagerSystem.Tools.EventManager;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// WorkflowDetail.xaml 的交互逻辑
    /// </summary>
    public partial class WorkflowDetail : UserControl, ICloseable
    {
        private Dictionary<string, LabelTextBox> FieldMaps = new Dictionary<string, LabelTextBox>();
        public WorkflowDetail()
        {
            InitializeComponent();
            InitField();
            UpdateFieldValue(Configs.Configs.WorkflowInfo);

            EventManager.Instance.Subscribe("WorkflowSelected", EventHandler);
        }

        private void InitField()
        {
            var fieldDefines = WorkflowInfo.FieldComments;
            foreach (var field in fieldDefines)
            {
                if (field.Key == "TappingVoltages")
                {
                    continue;
                }
                var fieldControl = new LabelTextBox()
                {
                    Label = field.Value,
                    Width = 400,
                    FontSize = 18,
                    Margin = new Thickness(10, 10, 40, 10)
                };
                FieldMaps.Add(field.Key, fieldControl);
                mainPanel.Children.Add(fieldControl);
            }
        }

        private void UpdateFieldValue(WorkflowInfo? workflow)
        {
            if (workflow == null)
            {
                if (Configs.Configs.WorkflowID.Length == 0)
                {
                    return;
                }
                Task.Run(() =>
                {
                    var workflows = WorkflowInfo.ReadFromDB(Configs.Configs.WorkflowID);
                    if (workflows == null || workflows.Count == 0)
                    {
                        return;
                    }
                    workflow = workflows[0];
                    if (workflow == null)
                    {
                        return;
                    }
                    Dispatcher.Invoke(() =>
                    {
                        UpdateFieldValue(workflow);
                    });
                });
                return;
            }

            var fieldDefines = WorkflowInfo.FieldComments;
            foreach (var field in fieldDefines)
            {
                if (field.Key == "TappingVoltages")
                {
                    continue;
                }
                var labelTextControl = FieldMaps[field.Key];
                var fieldValue = Utils.GetFieldValue(workflow, field.Key);
                if (fieldValue == null)
                {
                    labelTextControl.Text = string.Empty;
                    continue;
                }
                if (fieldValue.GetType() == typeof(int) || fieldValue.GetType() == typeof(int?))
                {
                    labelTextControl.Text = ((int)fieldValue).ToString();
                }
                else if (fieldValue.GetType() == typeof(float) || fieldValue.GetType() == typeof(float?))
                {
                    labelTextControl.Text = Utils.FloatFormat(((float)fieldValue));
                }
                else if (fieldValue.GetType() == typeof(string))
                {
                    labelTextControl.Text = (string)fieldValue;
                }
                else if (fieldValue.GetType() == typeof(bool))
                {
                    labelTextControl.Text = ((bool)fieldValue).ToString();
                }
            }
        }

        public void Close()
        {
            EventManager.Instance.Unsubscribe("WorkflowSelected", EventHandler);
        }

        private void EventHandler(object sender, TestEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateFieldValue(Configs.Configs.WorkflowInfo);
            });
        }
    }
}
