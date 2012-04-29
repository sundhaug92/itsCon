using System;

namespace itsLib.Messaging
{
    public class Mail
    {
        public Mail(string Path)
        {
        }

        public Mail(uint Id, uint FolderId)
            : this("/Messages/View_Message.aspx?MessageFolderID=" + FolderId.ToString() + "&MessageID=" + Id.ToString())
        {
        }

        private string From
        {
            get { throw new NotImplementedException(); }
        }

        private string To
        {
            get { throw new NotImplementedException(); }
        }

        private string Contents
        {
            get { throw new NotImplementedException(); }
        }
    }
}