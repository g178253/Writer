namespace Writer.ViewModels
{
    internal class WritableTextBlockViewModel : NotifyPropertyChanged
    {
        private string m_text;
        public string Text
        {
            get { return m_text; }
            set { m_text = value; OnPropertyChanged(); }
        }

        private bool m_isReadOnly = true;
        public bool IsReadOnly
        {
            get { return m_isReadOnly; }
            set { m_isReadOnly = value; OnPropertyChanged(); }
        }

        private bool m_inError;
        public bool InError
        {
            get { return m_inError; }
            set { m_inError = value; OnPropertyChanged(); }
        }

        private string m_editError;
        public string Error
        {
            get { return m_editError; }
            set { m_editError = value; OnPropertyChanged(); }
        }
    }
}
