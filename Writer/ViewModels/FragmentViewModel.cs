﻿using System;
using WriterCore.Model;

namespace Writer.ViewModels
{
    internal sealed class FragmentViewModel : NotifyPropertyChanged
    {
        private Fragment m_model;
        public FragmentViewModel(Fragment item)
        {
            m_model = item ?? new Fragment();
            m_model.CreateTime = DateTime.Now;
        }

        public string Title
        {
            get { return m_model.Title; }
            set { m_model.Title = value; OnPropertyChanged(); }
        }

        public string Summary
        {
            get { return m_model.Summary; }
            set { m_model.Summary = value; OnPropertyChanged(); }
        }
    }
}
