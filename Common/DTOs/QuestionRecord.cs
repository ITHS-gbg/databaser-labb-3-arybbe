using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Common.DTOs;

public class QuestionRecord(string id, string text, List<string> options, string correctOption)
    : INotifyPropertyChanged
{
    public string Id
    {
        get => id;
        set
        {
            if (id != value)
            {
                id = value;
                OnPropertyChanged();
            }
        }
    }

    public string Text
    {
        get => text;
        set
        {
            if (text != value)
            {
                text = value;
                OnPropertyChanged();
            }
        }
    }

    public List<string> Options
    {
        get => options;
        set
        {
            if (options != value)
            {
                options = value;
                OnPropertyChanged();
            }
        }
    }

    public string CorrectOption
    {
        get => correctOption;
        set
        {
            if (correctOption != value)
            {
                correctOption = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}