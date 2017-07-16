using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System;
using Microsoft.Msagl.Drawing;
using viewmodel;
using System.Threading.Tasks;
using System.Threading;

namespace Visualizer.Panes
{
    public abstract class BasePane : ViewModelBase
    {
        protected SccmConnector _connector;
        protected int _progresscount = 0;
        protected bool _processing = false;

        protected Graph _graph;
        public Graph Graph { get { return this._graph; } }

        

        protected string _notificationtext;
        public string NotificationText
        {
            get { return this._notificationtext; }
            set
            {
                this._notificationtext = value;
                this.OnPropertyChanged(this, "NotificationText");
            }
        }

        protected string _header;
        public string Header
        {
            get { return this._header; }
            set
            {
                this._header = value;
                this.OnPropertyChanged(this, "Header");
            }
        }

        protected string _findlabeltext;
        public string FindLabelText
        {
            get { return this._findlabeltext; }
            set
            {
                this._findlabeltext = value;
                this.OnPropertyChanged(this, "FindLabelText");
            }
        }

        protected string _findtext;
        public string FindText
        {
            get { return this._findtext; }
            set
            {
                this._findtext = value;
                this.OnPropertyChanged(this, "FindText");
            }
        }

        protected bool _controlsenabled = true;
        public bool ControlsEnabled
        {
            get { return this._controlsenabled; }
            set
            {
                this._controlsenabled = value;
                this.OnPropertyChanged(this, "ControlsEnabled");
            }
        }

        protected string _searchtext;
        public string SearchText
        {
            get { return this._searchtext; }
            set
            {
                this._searchtext = value;
                this.OnPropertyChanged(this, "SearchText");
            }
        }

        // Constructor
        public BasePane(SccmConnector connector)
        {
            this._connector = connector;
            this.NotificationText = string.Empty;
        }

        protected void OnTextBoxFocused(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }

        

        /// <summary>
        /// Adds notification with dots slowly working. Adds the dots to the specified string
        /// </summary>
        /// <param name="basetext"></param>
        protected void NotifyProgress(string basetext)
        {
            int count = 0;
            this.NotificationText = basetext;

            while (this._processing == true)
            {
                if (count < 5)
                {
                    this.NotificationText = this.NotificationText + ".";
                    count++;
                }
                else
                {
                    this.NotificationText = basetext;
                    count = 0;
                }
                Thread.Sleep(500);
            }
            this.NotificationText = string.Empty;
        }

        protected abstract void BuildGraph();
        protected void OnBuildKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.BuildGraph();
            }
        }

        protected void OnBuildButtonPressed(object sender, RoutedEventArgs e)
        {
            this.BuildGraph();
        }

        protected void OnProgressUpdate(object sender, EventArgs e)
        {
            if (this._progresscount < 2)
            {
                this.NotificationText = this.NotificationText + ".";
                this._progresscount++;
            }
            else { this.NotificationText = null; }

        }

        protected void UpdateProgressMessage(string newmessage)
        {
            this.NotificationText = newmessage;
            this._progresscount = 0;
        }

        //protected void OnAbortButtonClick(object sender, RoutedEventArgs e)
        //{
        //    this._pane.gviewer.AbortAsyncLayout();
        //    this.NotificationText = "Build aborted";
        //    this._progresscount = 0;
        //    this.NotificationText = null;
        //}

        
    }
}
