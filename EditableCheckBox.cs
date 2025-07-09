using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ToDoNote;

public class EditableCheckBox : CheckBox
{
    private DispatcherTimer _clickTimer;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("textEditor") is TextBox textBox)
        {
            textBox.KeyDown += TextBox_KeyDown;
            textBox.LostFocus += TextBox_LostFocus; // Добавляем обработку потери фокуса
        }
    }

    public EditableCheckBox()
    {
        InitializeClickTimer();
        PreviewMouseLeftButtonDown += EditableCheckBox_PreviewMouseLeftButtonDown;
        LostFocus += EditableCheckBox_LostFocus; // Обработка потери фокуса контролом
    }

    private void InitializeClickTimer()
    {
        _clickTimer = new DispatcherTimer();
        _clickTimer.Interval = TimeSpan.FromMilliseconds(200);
        _clickTimer.Tick += ClickTimer_Tick;
    }

    private void EditableCheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsEditing())
            return;

        if (e.ClickCount == 2)
        {
            _clickTimer.Stop();
            EnterEditMode();
            e.Handled = true;
        }
        else if (e.ClickCount == 1)
        {
            _clickTimer.Tag = e; // Сохраняем событие для последующей обработки
            _clickTimer.Start();
            e.Handled = true; // Предотвращаем немедленную обработку клика
        }
    }

    private void ClickTimer_Tick(object sender, EventArgs e)
    {
        _clickTimer.Stop();
        IsChecked = !IsChecked; // Обрабатываем одиночный клик
    }

    private void EnterEditMode()
    {
        Tag = "Editing";

        Dispatcher.BeginInvoke(() =>
        {
            if (GetTemplateChild("textEditor") is TextBox textBox && IsEditing())                
            {
                textBox.Visibility = Visibility.Visible;
                textBox.Focus();
                textBox.SelectAll();
            }

            if (GetTemplateChild("contentPresenter") is ContentPresenter cp)
            {
                cp.Visibility = Visibility.Collapsed;
            }
        }, DispatcherPriority.Input);
    }

    private void ExitEditMode()
    {
        if (!IsEditing()) return;

        Tag = null;

        if (GetTemplateChild("textEditor") is TextBox textBox)
        {
            // Принудительно обновляем привязку
            var binding = textBox.GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
            
            textBox.Visibility = Visibility.Collapsed;
        }

        if (GetTemplateChild("contentPresenter") is ContentPresenter cp)
        {
            cp.Visibility = Visibility.Visible;
        }
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Escape)
        {
            ExitEditMode();
        }
    }

    // Новый метод для обработки потери фокуса
    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        ExitEditMode();
    }

    // Остановка таймера при потере фокуса контролом
    private void EditableCheckBox_LostFocus(object sender, RoutedEventArgs e)
    {
        _clickTimer.Stop();
    }

    // Вспомогательный метод для проверки состояния
    private bool IsEditing() => Tag?.ToString() == "Editing";
}