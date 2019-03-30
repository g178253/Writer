using System;
using Writer.ViewModels;

namespace Writer
{
    internal sealed class General
    {
        /// <summary>
        /// 当前处于编辑状态，则为 true；添加状态，则为 false。
        /// </summary>
        public bool InEdit { get; set; }

        /// <summary>
        /// 未编辑前的文本。
        /// </summary>
        public string LastText { get; private set; }

        public void AddOrEdit(Action Edit, Action Add)
        {
            if (InEdit) Edit?.Invoke();
            else          Add?.Invoke();
        }

        public void BeginEdit(WritableTextBlockViewModel tb)
        {
            tb.IsReadOnly = false;
            LastText = tb.Text;
        }

        public void EndEdit(WritableTextBlockViewModel tb)
        {
            tb.IsReadOnly = true;
            ClearError(tb);
        }

        public void CancelEditOrAdd(WritableTextBlockViewModel tb, Action cancelAdd)
        {
            if (InEdit)
            {
                EndEdit(tb);
            }
            else
            {
                cancelAdd?.Invoke();
            }
        }

        public void SetError(WritableTextBlockViewModel tb, string v)
        {
            tb.InError = true;
            tb.Error = v;
        }

        public void ClearError(WritableTextBlockViewModel tb)
        {
            tb.InError = true;
            tb.Error = null;
        }
    }
}
