namespace app.web.Models
{
    public class ClientAlertModel
    {
        public ClientAlertModel()
        {
        }

        public ClientAlertModel(AlertStatus s, string m)
        {
            this._status = s;
            this._message = m;
        }

        /// <summary>
        /// 1 - Success
        /// 2 - Error
        /// </summary>
        private AlertStatus _status;
        public AlertStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
    }
}
