using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using model;

namespace CollectionViewer.Auth
{
    public class LoginViewModel: ViewModelBase
    {
        private string _username;
        private string _domain;
        private string _server;
        private bool _passthrough;
        private string _usernamelabel;
        private string _domainlabel;
        private string _serverlabel;
        private string _passthroughlabel;
        private string _okbuttonlabel;
        private string _sitelabel;
        private string _site;
        private string _pwlabel;
        private LoginWindow _loginwindow;

        public LoginViewModel()
        {
            this.SetDefaults();
            this._loginwindow = new LoginWindow();
            this._loginwindow.DataContext = this;
            this._loginwindow.ShowDialog();
        }

        public bool PassThrough
        {
            get { return this._passthrough; }
            set
            {
                this._passthrough = value;
                this.OnPropertyChanged(this, "PassThrough");
            }
        }
        public string PassThroughLabel
        {
            get { return this._passthroughlabel; }
            set
            {
                this._passthroughlabel = value;
                this.OnPropertyChanged(this, "PassThroughLabel");
            }
        }

        public string Username
        {
            get { return this._username; }
            set
            {
                this._username = value;
                this.OnPropertyChanged(this, "Username");
            }
        }

        public string UsernameLabel
        {
            get { return this._usernamelabel; }
            set
            {
                this._usernamelabel = value;
                this.OnPropertyChanged(this, "UsernameLabel");
            }
        }

        public string Domain
        {
            get { return this._domain; }
            set
            {
                this._domain = value;
                this.OnPropertyChanged(this, "Domain");
            }
        }
        public string DomainLabel
        {
            get { return this._domainlabel; }
            set
            {
                this._domainlabel = value;
                this.OnPropertyChanged(this, "DomainLabel");
            }
        }

        public string Server
        {
            get { return this._server; }
            set
            {
                this._server = value;
                this.OnPropertyChanged(this, "Server");
            }
        }
        public string ServerLabel
        {
            get { return this._serverlabel; }
            set
            {
                this._serverlabel = value;
                this.OnPropertyChanged(this, "ServerLabel");
            }
        }

        public string Site
        {
            get { return this._site; }
            set
            {
                this._site = value;
                this.OnPropertyChanged(this, "Site");
            }
        }

        public string SiteLabel
        {
            get { return this._sitelabel; }
            set
            {
                this._sitelabel = value;
                this.OnPropertyChanged(this, "SiteLabel");
            }
        }

        public string OkButtonText
        {
            get { return this._okbuttonlabel; }
            set
            {
                this._okbuttonlabel = value;
                this.OnPropertyChanged(this, "OkButtonText");
            }
        }

        public SecureString SecurePassword { get { return this._loginwindow.pwdbx.SecurePassword; } }
        public string Password { get { return this._loginwindow.pwdbx.Password; } }
        public string PasswordLabel
        {
            get { return this._pwlabel; }
            set
            {
                this._pwlabel = value;
                this.OnPropertyChanged(this, "PasswordLabel");
            }
        }
        private void SetDefaults()
        {
            this.Username = Environment.UserName;
            this.Domain = Environment.UserDomainName;
            this.DomainLabel = "Domain:";
            this.ServerLabel = "Server:";
            this.PasswordLabel = "Password:";
            this.UsernameLabel = "Username:";
            this.PassThroughLabel = "Use current user:";
            this.OkButtonText = "OK";
            this.SiteLabel = "Site:";
            this.PassThrough = true;
        }
    }
}
